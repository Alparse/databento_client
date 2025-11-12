#include "databento_native.h"
#include <string>
#include <cstring>

// ============================================================================
// Callback Bridge Utilities
// ============================================================================
// Note: Error handling uses caller-provided error buffers (better pattern)
// Thread-local error storage removed as it was unused dead code
