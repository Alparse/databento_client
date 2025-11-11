#include "databento_native.h"
#include <databento/historical.hpp>
#include <databento/record.hpp>
#include <databento/enums.hpp>
#include <databento/timeseries.hpp>
#include <databento/datetime.hpp>
#include <databento/symbology.hpp>
#include <memory>
#include <string>
#include <vector>
#include <cstring>
#include <chrono>
#include <sstream>

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

DATABENTO_API int dbento_historical_get_range_to_file(
    DbentoHistoricalClientHandle handle,
    const char* file_path,
    const char* dataset,
    const char* schema,
    const char** symbols,
    size_t symbol_count,
    int64_t start_time_ns,
    int64_t end_time_ns,
    char* error_buffer,
    size_t error_buffer_size)
{
    try {
        auto* wrapper = reinterpret_cast<HistoricalClientWrapper*>(handle);
        if (!wrapper || !wrapper->client) {
            SafeStrCopy(error_buffer, error_buffer_size, "Invalid client handle");
            return -1;
        }

        if (!file_path || !dataset || !schema) {
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
        else if (schema_str == "ohlcv-eod") schema_enum = db::Schema::OhlcvEod;
        else if (schema_str == "definition") schema_enum = db::Schema::Definition;
        else if (schema_str == "statistics") schema_enum = db::Schema::Statistics;
        else if (schema_str == "status") schema_enum = db::Schema::Status;
        else if (schema_str == "imbalance") schema_enum = db::Schema::Imbalance;
        else {
            SafeStrCopy(error_buffer, error_buffer_size, "Unknown schema type");
            return -3;
        }

        // Convert timestamps
        auto start_unix = NsToUnixNanos(start_time_ns);
        auto end_unix = NsToUnixNanos(end_time_ns);
        db::DateTimeRange<db::UnixNanos> datetime_range{start_unix, end_unix};

        // Call TimeseriesGetRangeToFile
        wrapper->client->TimeseriesGetRangeToFile(
            dataset,
            datetime_range,
            symbol_vec,
            schema_enum,
            std::filesystem::path{file_path}
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

// ============================================================================
// Symbology Resolution API
// ============================================================================

struct SymbologyResolutionWrapper {
    db::SymbologyResolution resolution;

    explicit SymbologyResolutionWrapper(db::SymbologyResolution&& res)
        : resolution(std::move(res)) {}
};

DATABENTO_API DbentoSymbologyResolutionHandle dbento_historical_symbology_resolve(
    DbentoHistoricalClientHandle handle,
    const char* dataset,
    const char** symbols,
    size_t symbol_count,
    const char* stype_in,
    const char* stype_out,
    const char* start_date,
    const char* end_date,
    char* error_buffer,
    size_t error_buffer_size)
{
    try {
        auto* wrapper = reinterpret_cast<HistoricalClientWrapper*>(handle);
        if (!wrapper || !wrapper->client) {
            SafeStrCopy(error_buffer, error_buffer_size, "Invalid client handle");
            return nullptr;
        }

        if (!dataset || !stype_in || !stype_out || !start_date || !end_date) {
            SafeStrCopy(error_buffer, error_buffer_size, "Invalid parameters");
            return nullptr;
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

        // Parse SType from strings
        auto parse_stype = [](const std::string& stype_str) -> db::SType {
            if (stype_str == "instrument_id") return db::SType::InstrumentId;
            if (stype_str == "raw_symbol") return db::SType::RawSymbol;
            if (stype_str == "parent") return db::SType::Parent;
            if (stype_str == "continuous") return db::SType::Continuous;
            if (stype_str == "nasdaq") return db::SType::Nasdaq;
            if (stype_str == "cms") return db::SType::Cms;
            if (stype_str == "gbbg") return db::SType::Gbbg;
            if (stype_str == "figi") return db::SType::Figi;
            if (stype_str == "figi_composite") return db::SType::FigiComposite;
            if (stype_str == "isin") return db::SType::Isin;
            if (stype_str == "sedol") return db::SType::Sedol;
            if (stype_str == "cusip") return db::SType::Cusip;
            throw std::invalid_argument("Unknown SType: " + stype_str);
        };

        db::SType stype_in_enum = parse_stype(stype_in);
        db::SType stype_out_enum = parse_stype(stype_out);

        // Parse dates using date library
        std::istringstream start_stream(start_date);
        std::istringstream end_stream(end_date);
        date::year_month_day start_ymd, end_ymd;

        start_stream >> date::parse("%Y-%m-%d", start_ymd);
        end_stream >> date::parse("%Y-%m-%d", end_ymd);

        if (start_stream.fail() || end_stream.fail()) {
            SafeStrCopy(error_buffer, error_buffer_size, "Invalid date format (expected YYYY-MM-DD)");
            return nullptr;
        }

        db::DateRange date_range{start_ymd, end_ymd};

        // Call SymbologyResolve
        auto resolution = wrapper->client->SymbologyResolve(
            dataset,
            symbol_vec,
            stype_in_enum,
            stype_out_enum,
            date_range
        );

        auto* res_wrapper = new SymbologyResolutionWrapper(std::move(resolution));
        return reinterpret_cast<DbentoSymbologyResolutionHandle>(res_wrapper);
    }
    catch (const std::exception& e) {
        SafeStrCopy(error_buffer, error_buffer_size, e.what());
        return nullptr;
    }
}

DATABENTO_API size_t dbento_symbology_resolution_mappings_count(
    DbentoSymbologyResolutionHandle handle)
{
    try {
        auto* wrapper = reinterpret_cast<SymbologyResolutionWrapper*>(handle);
        return wrapper ? wrapper->resolution.mappings.size() : 0;
    }
    catch (...) {
        return 0;
    }
}

DATABENTO_API int dbento_symbology_resolution_get_mapping_key(
    DbentoSymbologyResolutionHandle handle,
    size_t index,
    char* key_buffer,
    size_t key_buffer_size)
{
    try {
        auto* wrapper = reinterpret_cast<SymbologyResolutionWrapper*>(handle);
        if (!wrapper || !key_buffer || key_buffer_size == 0) {
            return -1;
        }

        if (index >= wrapper->resolution.mappings.size()) {
            return -2; // Index out of bounds
        }

        // Iterate to the index-th element
        auto it = wrapper->resolution.mappings.begin();
        std::advance(it, index);

        SafeStrCopy(key_buffer, key_buffer_size, it->first.c_str());
        return 0;
    }
    catch (...) {
        return -1;
    }
}

DATABENTO_API size_t dbento_symbology_resolution_get_intervals_count(
    DbentoSymbologyResolutionHandle handle,
    const char* symbol_key)
{
    try {
        auto* wrapper = reinterpret_cast<SymbologyResolutionWrapper*>(handle);
        if (!wrapper || !symbol_key) {
            return 0;
        }

        auto it = wrapper->resolution.mappings.find(symbol_key);
        if (it == wrapper->resolution.mappings.end()) {
            return 0;
        }

        return it->second.size();
    }
    catch (...) {
        return 0;
    }
}

DATABENTO_API int dbento_symbology_resolution_get_interval(
    DbentoSymbologyResolutionHandle handle,
    const char* symbol_key,
    size_t interval_index,
    char* start_date_buffer,
    size_t start_date_buffer_size,
    char* end_date_buffer,
    size_t end_date_buffer_size,
    char* symbol_buffer,
    size_t symbol_buffer_size)
{
    try {
        auto* wrapper = reinterpret_cast<SymbologyResolutionWrapper*>(handle);
        if (!wrapper || !symbol_key) {
            return -1;
        }

        auto it = wrapper->resolution.mappings.find(symbol_key);
        if (it == wrapper->resolution.mappings.end()) {
            return -2; // Key not found
        }

        if (interval_index >= it->second.size()) {
            return -3; // Index out of bounds
        }

        const auto& interval = it->second[interval_index];

        // Format dates as YYYY-MM-DD
        std::ostringstream start_ss, end_ss;
        start_ss << date::format("%Y-%m-%d", interval.start_date);
        end_ss << date::format("%Y-%m-%d", interval.end_date);

        SafeStrCopy(start_date_buffer, start_date_buffer_size, start_ss.str().c_str());
        SafeStrCopy(end_date_buffer, end_date_buffer_size, end_ss.str().c_str());
        SafeStrCopy(symbol_buffer, symbol_buffer_size, interval.symbol.c_str());

        return 0;
    }
    catch (...) {
        return -1;
    }
}

DATABENTO_API size_t dbento_symbology_resolution_partial_count(
    DbentoSymbologyResolutionHandle handle)
{
    try {
        auto* wrapper = reinterpret_cast<SymbologyResolutionWrapper*>(handle);
        return wrapper ? wrapper->resolution.partial.size() : 0;
    }
    catch (...) {
        return 0;
    }
}

DATABENTO_API int dbento_symbology_resolution_get_partial(
    DbentoSymbologyResolutionHandle handle,
    size_t index,
    char* symbol_buffer,
    size_t symbol_buffer_size)
{
    try {
        auto* wrapper = reinterpret_cast<SymbologyResolutionWrapper*>(handle);
        if (!wrapper || !symbol_buffer || symbol_buffer_size == 0) {
            return -1;
        }

        if (index >= wrapper->resolution.partial.size()) {
            return -2; // Index out of bounds
        }

        SafeStrCopy(symbol_buffer, symbol_buffer_size, wrapper->resolution.partial[index].c_str());
        return 0;
    }
    catch (...) {
        return -1;
    }
}

DATABENTO_API size_t dbento_symbology_resolution_not_found_count(
    DbentoSymbologyResolutionHandle handle)
{
    try {
        auto* wrapper = reinterpret_cast<SymbologyResolutionWrapper*>(handle);
        return wrapper ? wrapper->resolution.not_found.size() : 0;
    }
    catch (...) {
        return 0;
    }
}

DATABENTO_API int dbento_symbology_resolution_get_not_found(
    DbentoSymbologyResolutionHandle handle,
    size_t index,
    char* symbol_buffer,
    size_t symbol_buffer_size)
{
    try {
        auto* wrapper = reinterpret_cast<SymbologyResolutionWrapper*>(handle);
        if (!wrapper || !symbol_buffer || symbol_buffer_size == 0) {
            return -1;
        }

        if (index >= wrapper->resolution.not_found.size()) {
            return -2; // Index out of bounds
        }

        SafeStrCopy(symbol_buffer, symbol_buffer_size, wrapper->resolution.not_found[index].c_str());
        return 0;
    }
    catch (...) {
        return -1;
    }
}

DATABENTO_API int dbento_symbology_resolution_get_stype_in(
    DbentoSymbologyResolutionHandle handle)
{
    try {
        auto* wrapper = reinterpret_cast<SymbologyResolutionWrapper*>(handle);
        if (!wrapper) {
            return -1;
        }
        return static_cast<int>(wrapper->resolution.stype_in);
    }
    catch (...) {
        return -1;
    }
}

DATABENTO_API int dbento_symbology_resolution_get_stype_out(
    DbentoSymbologyResolutionHandle handle)
{
    try {
        auto* wrapper = reinterpret_cast<SymbologyResolutionWrapper*>(handle);
        if (!wrapper) {
            return -1;
        }
        return static_cast<int>(wrapper->resolution.stype_out);
    }
    catch (...) {
        return -1;
    }
}

DATABENTO_API void dbento_symbology_resolution_destroy(
    DbentoSymbologyResolutionHandle handle)
{
    try {
        auto* wrapper = reinterpret_cast<SymbologyResolutionWrapper*>(handle);
        delete wrapper;
    }
    catch (...) {
        // Swallow exceptions in cleanup
    }
}
