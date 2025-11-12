#pragma once

#include <chrono>
#include <cstring>
#include <string>
#include <stdexcept>
#include <databento/enums.hpp>
#include <databento/datetime.hpp>

namespace databento_native {

/**
 * Safely copy a C string to a buffer with null termination
 * @param dest Destination buffer
 * @param dest_size Size of destination buffer
 * @param src Source string (can be nullptr)
 */
inline void SafeStrCopy(char* dest, size_t dest_size, const char* src) {
    // Validate destination
    if (!dest || dest_size == 0) {
        return;
    }

    // Handle null source
    if (!src) {
        dest[0] = '\0';
        return;
    }

    // Copy with bounds checking
    strncpy(dest, src, dest_size - 1);
    dest[dest_size - 1] = '\0';  // Ensure null termination
}

/**
 * Parse schema string to databento Schema enum
 * Centralized to ensure consistency across all wrappers
 * @param schema_str Schema string (e.g., "mbo", "mbp-1", "trades")
 * @return Schema enum value
 * @throws std::runtime_error if schema string is unknown
 */
inline databento::Schema ParseSchema(const std::string& schema_str) {
    // MBO/MBP schemas
    if (schema_str == "mbo") return databento::Schema::Mbo;
    if (schema_str == "mbp-1") return databento::Schema::Mbp1;
    if (schema_str == "mbp-10") return databento::Schema::Mbp10;

    // Trade schemas
    if (schema_str == "trades") return databento::Schema::Trades;

    // OHLCV schemas
    if (schema_str == "ohlcv-1s") return databento::Schema::Ohlcv1S;
    if (schema_str == "ohlcv-1m") return databento::Schema::Ohlcv1M;
    if (schema_str == "ohlcv-1h") return databento::Schema::Ohlcv1H;
    if (schema_str == "ohlcv-1d") return databento::Schema::Ohlcv1D;
    if (schema_str == "ohlcv-eod") return databento::Schema::OhlcvEod;

    // Other schemas
    if (schema_str == "definition") return databento::Schema::Definition;
    if (schema_str == "statistics") return databento::Schema::Statistics;
    if (schema_str == "status") return databento::Schema::Status;
    if (schema_str == "imbalance") return databento::Schema::Imbalance;

    // Unknown schema
    throw std::runtime_error("Unknown schema: " + schema_str);
}

/**
 * Convert nanoseconds since epoch to UnixNanos with validation
 * Prevents integer overflow from negative timestamps
 * @param ns Timestamp in nanoseconds since Unix epoch
 * @return UnixNanos value
 * @throws std::invalid_argument if timestamp is negative or too large
 */
inline databento::UnixNanos NsToUnixNanos(int64_t ns) {
    // Validate range - timestamps before Unix epoch not allowed
    if (ns < 0) {
        throw std::invalid_argument("Timestamp cannot be negative (before Unix epoch 1970-01-01)");
    }

    // Validate upper bound (year 9999 - reasonable maximum)
    // Year 9999-12-31 23:59:59.999999999 in nanoseconds
    constexpr uint64_t MAX_TIMESTAMP = 253402300799ULL * 1000000000ULL + 999999999ULL;
    if (static_cast<uint64_t>(ns) > MAX_TIMESTAMP) {
        throw std::invalid_argument("Timestamp too large (after year 9999)");
    }

    // Safe cast to unsigned after validation
    return databento::UnixNanos{std::chrono::duration<uint64_t, std::nano>{static_cast<uint64_t>(ns)}};
}

/**
 * Validate that a string parameter is not NULL and not empty
 * @param param_name Name of the parameter for error messages
 * @param value String value to validate
 * @throws std::invalid_argument if validation fails
 */
inline void ValidateNonEmptyString(const char* param_name, const char* value) {
    if (!value) {
        throw std::invalid_argument(std::string(param_name) + " cannot be NULL");
    }
    if (value[0] == '\0') {
        throw std::invalid_argument(std::string(param_name) + " cannot be empty");
    }
}

/**
 * Validate symbol array parameters for consistency
 * @param symbols Symbol array pointer
 * @param symbol_count Number of symbols
 * @throws std::invalid_argument if validation fails
 */
inline void ValidateSymbolArray(const char** symbols, size_t symbol_count) {
    // If count > 0, symbols array must not be NULL
    if (symbol_count > 0 && !symbols) {
        throw std::invalid_argument("Symbol array cannot be NULL when symbol_count > 0");
    }

    // Validate reasonable symbol count (prevent resource exhaustion)
    constexpr size_t MAX_SYMBOLS = 100000;  // Reasonable limit for batch operations
    if (symbol_count > MAX_SYMBOLS) {
        throw std::invalid_argument("Symbol count exceeds maximum limit of " + std::to_string(MAX_SYMBOLS));
    }
}

/**
 * Validate timestamp range
 * @param start_ns Start timestamp in nanoseconds
 * @param end_ns End timestamp in nanoseconds
 * @throws std::invalid_argument if validation fails
 */
inline void ValidateTimeRange(int64_t start_ns, int64_t end_ns) {
    // Both timestamps validated by NsToUnixNanos, just check ordering
    if (start_ns > end_ns) {
        throw std::invalid_argument("Start time must be before or equal to end time");
    }
}

/**
 * Validate error buffer parameters
 * @param error_buffer Error buffer pointer
 * @param error_buffer_size Error buffer size
 * @return true if error buffer is valid and can be used
 */
inline bool IsErrorBufferValid(char* error_buffer, size_t error_buffer_size) {
    return error_buffer != nullptr && error_buffer_size > 0;
}

}  // namespace databento_native
