#include "databento_native.h"
#include <databento/historical.hpp>
#include <databento/record.hpp>
#include <databento/enums.hpp>
#include <databento/timeseries.hpp>
#include <databento/datetime.hpp>
#include <memory>
#include <string>
#include <vector>
#include <cstring>
#include <chrono>

namespace db = databento;

// ============================================================================
// Internal Wrapper Class
// ============================================================================
struct HistoricalClientWrapper {
    std::unique_ptr<db::Historical> client;
    std::string api_key;

    explicit HistoricalClientWrapper(const std::string& key)
        : api_key(key) {
        client = std::make_unique<db::Historical>(nullptr, key, db::HistoricalGateway::Bo1);
    }
};

struct MetadataWrapper {
    db::Metadata metadata;

    explicit MetadataWrapper(db::Metadata&& meta)
        : metadata(std::move(meta)) {}
};

// ============================================================================
// Helper Functions
// ============================================================================
static void SafeStrCopy(char* dest, size_t dest_size, const char* src) {
    if (dest && dest_size > 0 && src) {
        strncpy(dest, src, dest_size - 1);
        dest[dest_size - 1] = '\0';
    }
}

// Convert nanoseconds since epoch to UnixNanos
static db::UnixNanos NsToUnixNanos(int64_t ns) {
    return db::UnixNanos{std::chrono::duration<uint64_t, std::nano>{static_cast<uint64_t>(ns)}};
}

// ============================================================================
// C API Implementation
// ============================================================================

DATABENTO_API DbentoHistoricalClientHandle dbento_historical_create(
    const char* api_key,
    char* error_buffer,
    size_t error_buffer_size)
{
    try {
        if (!api_key) {
            SafeStrCopy(error_buffer, error_buffer_size, "API key cannot be null");
            return nullptr;
        }

        auto* wrapper = new HistoricalClientWrapper(api_key);
        return reinterpret_cast<DbentoHistoricalClientHandle>(wrapper);
    }
    catch (const std::exception& e) {
        SafeStrCopy(error_buffer, error_buffer_size, e.what());
        return nullptr;
    }
}

DATABENTO_API int dbento_historical_get_range(
    DbentoHistoricalClientHandle handle,
    const char* dataset,
    const char* schema,
    const char** symbols,
    size_t symbol_count,
    int64_t start_time_ns,
    int64_t end_time_ns,
    RecordCallback on_record,
    void* user_data,
    char* error_buffer,
    size_t error_buffer_size)
{
    try {
        auto* wrapper = reinterpret_cast<HistoricalClientWrapper*>(handle);
        if (!wrapper || !wrapper->client) {
            SafeStrCopy(error_buffer, error_buffer_size, "Invalid client handle");
            return -1;
        }

        if (!dataset || !schema || !on_record) {
            SafeStrCopy(error_buffer, error_buffer_size, "Invalid parameters");
            return -2;
        }

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

        // Convert timestamps
        auto start_unix = NsToUnixNanos(start_time_ns);
        auto end_unix = NsToUnixNanos(end_time_ns);
        db::DateTimeRange<db::UnixNanos> datetime_range{start_unix, end_unix};

        // Call timeseries API
        wrapper->client->TimeseriesGetRange(
            dataset,
            datetime_range,
            symbol_vec,
            schema_enum,
            [on_record, user_data](const db::Record& record) {
                // Get the actual RecordHeader pointer (not the Record wrapper)
                const auto& header = record.Header();
                const uint8_t* bytes = reinterpret_cast<const uint8_t*>(&header);
                size_t length = record.Size();
                uint8_t type = static_cast<uint8_t>(record.RType());

                on_record(bytes, length, type, user_data);
                return db::KeepGoing::Continue;
            }
        );

        return 0;
    }
    catch (const std::exception& e) {
        SafeStrCopy(error_buffer, error_buffer_size, e.what());
        return -1;
    }
}

DATABENTO_API DbentoMetadataHandle dbento_historical_get_metadata(
    DbentoHistoricalClientHandle handle,
    const char* dataset,
    const char* schema,
    int64_t start_time_ns,
    int64_t end_time_ns,
    char* error_buffer,
    size_t error_buffer_size)
{
    try {
        auto* wrapper = reinterpret_cast<HistoricalClientWrapper*>(handle);
        if (!wrapper || !wrapper->client) {
            SafeStrCopy(error_buffer, error_buffer_size, "Invalid client handle");
            return nullptr;
        }

        if (!dataset || !schema) {
            SafeStrCopy(error_buffer, error_buffer_size, "Dataset and schema cannot be null");
            return nullptr;
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
            return nullptr;
        }

        // Note: Getting metadata without full query is not directly supported
        // For now, return nullptr - this feature would need a different API approach
        SafeStrCopy(error_buffer, error_buffer_size, "Metadata-only query not implemented");
        return nullptr;
    }
    catch (const std::exception& e) {
        SafeStrCopy(error_buffer, error_buffer_size, e.what());
        return nullptr;
    }
}

DATABENTO_API void dbento_historical_destroy(DbentoHistoricalClientHandle handle)
{
    try {
        auto* wrapper = reinterpret_cast<HistoricalClientWrapper*>(handle);
        delete wrapper;
    }
    catch (...) {
        // Swallow exceptions in cleanup
    }
}

// ============================================================================
// Metadata API
// ============================================================================

DATABENTO_API int dbento_metadata_get_symbol_mapping(
    DbentoMetadataHandle handle,
    uint32_t instrument_id,
    char* symbol_buffer,
    size_t symbol_buffer_size)
{
    try {
        auto* wrapper = reinterpret_cast<MetadataWrapper*>(handle);
        if (!wrapper) {
            return -1;
        }

        // Get symbol mapping from metadata
        auto symbol_map = wrapper->metadata.CreateSymbolMap();

        // Look up the instrument ID - API may have different method name
        // For now, return not implemented
        return -2; // Not found/not implemented
    }
    catch (...) {
        return -1;
    }
}

DATABENTO_API void dbento_metadata_destroy(DbentoMetadataHandle handle)
{
    try {
        auto* wrapper = reinterpret_cast<MetadataWrapper*>(handle);
        delete wrapper;
    }
    catch (...) {
        // Swallow exceptions in cleanup
    }
}
