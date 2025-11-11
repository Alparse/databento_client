#include "databento_native.h"
#include <string>
#include <cstring>

// ============================================================================
// Callback Bridge Utilities
// ============================================================================

namespace {

// Thread-local storage for last error
thread_local char g_last_error[512] = {0};

} // anonymous namespace

// Helper to set last error (for future error retrieval functionality)
void SetLastError(const char* error_msg) {
    if (error_msg) {
        strncpy(g_last_error, error_msg, sizeof(g_last_error) - 1);
        g_last_error[sizeof(g_last_error) - 1] = '\0';
    }
}

// Helper to get last error
const char* GetLastError() {
    return g_last_error[0] ? g_last_error : "No error";
}
