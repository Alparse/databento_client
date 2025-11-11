#ifndef DATABENTO_NATIVE_H
#define DATABENTO_NATIVE_H

#include <stddef.h>
#include <stdint.h>

#ifdef _WIN32
    #ifdef DATABENTO_EXPORTS
        #define DATABENTO_API __declspec(dllexport)
    #else
        #define DATABENTO_API __declspec(dllimport)
    #endif
#else
    #define DATABENTO_API __attribute__((visibility("default")))
#endif

#ifdef __cplusplus
extern "C" {
#endif

// ============================================================================
// Opaque Handles
// ============================================================================
typedef void* DbentoLiveClientHandle;
typedef void* DbentoHistoricalClientHandle;
typedef void* DbentoMetadataHandle;

// ============================================================================
// Callback Types
// ============================================================================

/**
 * Callback for received records
 * @param record_bytes Raw record data (DBN format)
 * @param record_length Length of record in bytes
 * @param record_type Record type identifier (schema type)
 * @param user_data User-provided context pointer
 */
typedef void (*RecordCallback)(
    const uint8_t* record_bytes,
    size_t record_length,
    uint8_t record_type,
    void* user_data
);

/**
 * Callback for errors
 * @param error_message Error description
 * @param error_code Error code (negative values indicate errors)
 * @param user_data User-provided context pointer
 */
typedef void (*ErrorCallback)(
    const char* error_message,
    int error_code,
    void* user_data
);

// ============================================================================
// Live Client API
// ============================================================================

/**
 * Create a live client (threaded mode)
 * @param api_key Databento API key (required)
 * @param error_buffer Buffer for error messages (can be NULL)
 * @param error_buffer_size Size of error buffer
 * @return Handle to live client, or NULL on failure
 */
DATABENTO_API DbentoLiveClientHandle dbento_live_create(
    const char* api_key,
    char* error_buffer,
    size_t error_buffer_size
);

/**
 * Subscribe to data streams
 * @param handle Live client handle
 * @param dataset Dataset name (e.g., "GLBX.MDP3")
 * @param schema Schema name (e.g., "trades", "mbp-1", "ohlcv-1s")
 * @param symbols Array of symbol strings (NULL-terminated)
 * @param symbol_count Number of symbols in array
 * @param error_buffer Buffer for error messages
 * @param error_buffer_size Size of error buffer
 * @return 0 on success, negative error code on failure
 */
DATABENTO_API int dbento_live_subscribe(
    DbentoLiveClientHandle handle,
    const char* dataset,
    const char* schema,
    const char** symbols,
    size_t symbol_count,
    char* error_buffer,
    size_t error_buffer_size
);

/**
 * Start receiving data (blocking call - runs until stopped)
 * @param handle Live client handle
 * @param on_record Callback invoked for each received record
 * @param on_error Callback invoked on errors
 * @param user_data User context passed to callbacks
 * @param error_buffer Buffer for error messages
 * @param error_buffer_size Size of error buffer
 * @return 0 on success, negative error code on failure
 */
DATABENTO_API int dbento_live_start(
    DbentoLiveClientHandle handle,
    RecordCallback on_record,
    ErrorCallback on_error,
    void* user_data,
    char* error_buffer,
    size_t error_buffer_size
);

/**
 * Stop receiving data
 * @param handle Live client handle
 */
DATABENTO_API void dbento_live_stop(DbentoLiveClientHandle handle);

/**
 * Destroy live client and free resources
 * @param handle Live client handle
 */
DATABENTO_API void dbento_live_destroy(DbentoLiveClientHandle handle);

// ============================================================================
// Historical Client API
// ============================================================================

/**
 * Create a historical data client
 * @param api_key Databento API key (required)
 * @param error_buffer Buffer for error messages (can be NULL)
 * @param error_buffer_size Size of error buffer
 * @return Handle to historical client, or NULL on failure
 */
DATABENTO_API DbentoHistoricalClientHandle dbento_historical_create(
    const char* api_key,
    char* error_buffer,
    size_t error_buffer_size
);

/**
 * Query historical time series data
 * @param handle Historical client handle
 * @param dataset Dataset name (e.g., "GLBX.MDP3")
 * @param schema Schema name (e.g., "trades", "mbp-1")
 * @param symbols Array of symbol strings
 * @param symbol_count Number of symbols
 * @param start_time_ns Start time (nanoseconds since Unix epoch)
 * @param end_time_ns End time (nanoseconds since Unix epoch)
 * @param on_record Callback invoked for each historical record
 * @param user_data User context passed to callback
 * @param error_buffer Buffer for error messages
 * @param error_buffer_size Size of error buffer
 * @return 0 on success, negative error code on failure
 */
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
    size_t error_buffer_size
);

/**
 * Get metadata for a historical query
 * @param handle Historical client handle
 * @param dataset Dataset name
 * @param schema Schema name
 * @param start_time_ns Start time (nanoseconds since Unix epoch)
 * @param end_time_ns End time (nanoseconds since Unix epoch)
 * @param error_buffer Buffer for error messages
 * @param error_buffer_size Size of error buffer
 * @return Metadata handle, or NULL on failure
 */
DATABENTO_API DbentoMetadataHandle dbento_historical_get_metadata(
    DbentoHistoricalClientHandle handle,
    const char* dataset,
    const char* schema,
    int64_t start_time_ns,
    int64_t end_time_ns,
    char* error_buffer,
    size_t error_buffer_size
);

/**
 * Destroy historical client and free resources
 * @param handle Historical client handle
 */
DATABENTO_API void dbento_historical_destroy(DbentoHistoricalClientHandle handle);

// ============================================================================
// Metadata API
// ============================================================================

/**
 * Get symbol mapping from metadata
 * @param handle Metadata handle
 * @param instrument_id Instrument ID to look up
 * @param symbol_buffer Buffer to receive symbol string
 * @param symbol_buffer_size Size of symbol buffer
 * @return 0 on success, negative error code if not found
 */
DATABENTO_API int dbento_metadata_get_symbol_mapping(
    DbentoMetadataHandle handle,
    uint32_t instrument_id,
    char* symbol_buffer,
    size_t symbol_buffer_size
);

/**
 * Destroy metadata handle and free resources
 * @param handle Metadata handle
 */
DATABENTO_API void dbento_metadata_destroy(DbentoMetadataHandle handle);

#ifdef __cplusplus
}
#endif

#endif // DATABENTO_NATIVE_H
