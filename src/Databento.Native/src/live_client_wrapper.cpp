#include "databento_native.h"
#include <databento/live_threaded.hpp>
#include <databento/live.hpp>
#include <databento/record.hpp>
#include <databento/enums.hpp>
#include <memory>
#include <string>
#include <vector>
#include <cstring>
#include <exception>

namespace db = databento;

// ============================================================================
// Internal Wrapper Class
// ============================================================================
struct LiveClientWrapper {
    std::unique_ptr<db::LiveThreaded> client;
    RecordCallback record_callback = nullptr;
    ErrorCallback error_callback = nullptr;
    void* user_data = nullptr;
    bool is_running = false;
    std::string dataset;
    std::string api_key;

    explicit LiveClientWrapper(const std::string& key)
        : api_key(key) {}

    ~LiveClientWrapper() {
        // LiveThreaded destructor handles cleanup
    }

    // Called by databento-cpp when a record is received
    db::KeepGoing OnRecord(const db::Record& record) {
        if (record_callback) {
            // Get the actual RecordHeader pointer (not the Record wrapper)
            const auto& header = record.Header();
            const uint8_t* bytes = reinterpret_cast<const uint8_t*>(&header);

            // Get record size based on its type
            size_t length = record.Size();

            // Get record type
            uint8_t type = static_cast<uint8_t>(record.RType());

            // Invoke callback
            record_callback(bytes, length, type, user_data);
        }
        return is_running ? db::KeepGoing::Continue : db::KeepGoing::Stop;
    }

    // Called when an error occurs
    void OnError(const std::exception& e) {
        if (error_callback) {
            error_callback(e.what(), -1, user_data);
        }
    }
};

// ============================================================================
// Helper Functions
// ============================================================================
static void SafeStrCopy(char* dest, size_t dest_size, const char* src) {
    if (dest && dest_size > 0) {
        strncpy(dest, src, dest_size - 1);
        dest[dest_size - 1] = '\0';
    }
}

// ============================================================================
// C API Implementation
// ============================================================================

DATABENTO_API DbentoLiveClientHandle dbento_live_create(
    const char* api_key,
    char* error_buffer,
    size_t error_buffer_size)
{
    try {
        if (!api_key) {
            SafeStrCopy(error_buffer, error_buffer_size, "API key cannot be null");
            return nullptr;
        }

        auto* wrapper = new LiveClientWrapper(api_key);
        return reinterpret_cast<DbentoLiveClientHandle>(wrapper);
    }
    catch (const std::exception& e) {
        SafeStrCopy(error_buffer, error_buffer_size, e.what());
        return nullptr;
    }
}

DATABENTO_API int dbento_live_subscribe(
    DbentoLiveClientHandle handle,
    const char* dataset,
    const char* schema,
    const char** symbols,
    size_t symbol_count,
    char* error_buffer,
    size_t error_buffer_size)
{
    try {
        auto* wrapper = reinterpret_cast<LiveClientWrapper*>(handle);
        if (!wrapper) {
            SafeStrCopy(error_buffer, error_buffer_size, "Invalid client handle");
            return -1;
        }

        if (!dataset || !schema) {
            SafeStrCopy(error_buffer, error_buffer_size, "Dataset and schema cannot be null");
            return -2;
        }

        // Store dataset for client creation
        wrapper->dataset = dataset;

        // Convert symbols to vector
        std::vector<std::string> symbol_vec;
        if (symbols && symbol_count > 0) {
            for (size_t i = 0; i < symbol_count; ++i) {
                if (symbols[i]) {
                    symbol_vec.emplace_back(symbols[i]);
                }
            }
        }

        // Parse schema from string to enum
        db::Schema schema_enum;
        std::string schema_str = schema;
        if (schema_str == "mbo") schema_enum = db::Schema::Mbo;
        else if (schema_str == "mbp-1") schema_enum = db::Schema::Mbp1;
        else if (schema_str == "mbp-10") schema_enum = db::Schema::Mbp10;
        else if (schema_str == "trades") schema_enum = db::Schema::Trades;
        else if (schema_str == "ohlcv-1s") schema_enum = db::Schema::Ohlcv1S;
        else if (schema_str == "ohlcv-1m") schema_enum = db::Schema::Ohlcv1M;
        else if (schema_str == "ohlcv-1h") schema_enum = db::Schema::Ohlcv1H;
        else if (schema_str == "ohlcv-1d") schema_enum = db::Schema::Ohlcv1D;
        else {
            SafeStrCopy(error_buffer, error_buffer_size, "Unknown schema type");
            return -3;
        }

        // Create client now that we have the dataset
        if (!wrapper->client) {
            wrapper->client = std::make_unique<db::LiveThreaded>(
                db::LiveThreaded::Builder()
                    .SetKey(wrapper->api_key)
                    .SetDataset(wrapper->dataset)
                    .BuildThreaded()
            );
        }

        // Subscribe using databento-cpp API (symbols, schema, stype)
        wrapper->client->Subscribe(symbol_vec, schema_enum, db::SType::RawSymbol);

        return 0;
    }
    catch (const std::exception& e) {
        SafeStrCopy(error_buffer, error_buffer_size, e.what());
        return -1;
    }
}

DATABENTO_API int dbento_live_start(
    DbentoLiveClientHandle handle,
    RecordCallback on_record,
    ErrorCallback on_error,
    void* user_data,
    char* error_buffer,
    size_t error_buffer_size)
{
    try {
        auto* wrapper = reinterpret_cast<LiveClientWrapper*>(handle);
        if (!wrapper || !wrapper->client) {
            SafeStrCopy(error_buffer, error_buffer_size, "Invalid client handle");
            return -1;
        }

        if (!on_record) {
            SafeStrCopy(error_buffer, error_buffer_size, "Record callback cannot be null");
            return -2;
        }

        // Store callbacks and user data
        wrapper->record_callback = on_record;
        wrapper->error_callback = on_error;
        wrapper->user_data = user_data;
        wrapper->is_running = true;

        // Start the client with a lambda that bridges to our callback
        wrapper->client->Start([wrapper](const db::Record& record) {
            return wrapper->OnRecord(record);
        });

        return 0;
    }
    catch (const std::exception& e) {
        SafeStrCopy(error_buffer, error_buffer_size, e.what());
        return -1;
    }
}

DATABENTO_API void dbento_live_stop(DbentoLiveClientHandle handle)
{
    try {
        auto* wrapper = reinterpret_cast<LiveClientWrapper*>(handle);
        if (wrapper && wrapper->is_running) {
            wrapper->is_running = false;
            // The callback will return KeepGoing::Stop on next iteration
        }
    }
    catch (...) {
        // Swallow exceptions in cleanup
    }
}

DATABENTO_API void dbento_live_destroy(DbentoLiveClientHandle handle)
{
    try {
        auto* wrapper = reinterpret_cast<LiveClientWrapper*>(handle);
        if (wrapper) {
            wrapper->is_running = false;
            delete wrapper;
        }
    }
    catch (...) {
        // Swallow exceptions in cleanup
    }
}
