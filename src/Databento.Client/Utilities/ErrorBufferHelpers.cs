namespace Databento.Client.Utilities;

/// <summary>
/// Helper utilities for safely extracting error strings from native error buffers
/// </summary>
public static class ErrorBufferHelpers
{
    /// <summary>
    /// Safely extract a UTF-8 string from an error buffer with proper null-termination validation
    /// </summary>
    /// <param name="errorBuffer">The error buffer from native code</param>
    /// <returns>The extracted error string, trimmed of null terminators</returns>
    /// <remarks>
    /// HIGH FIX: This method prevents information disclosure by validating null termination
    /// before extracting the string. If native code doesn't properly null-terminate,
    /// this prevents reading uninitialized memory.
    /// </remarks>
    public static string SafeGetString(byte[] errorBuffer)
    {
        if (errorBuffer == null || errorBuffer.Length == 0)
            return string.Empty;

        // Find the first null byte
        int nullIndex = Array.IndexOf(errorBuffer, (byte)0);

        // If no null found, use entire buffer (but this indicates a potential issue)
        int length = nullIndex >= 0 ? nullIndex : errorBuffer.Length;

        // Extract only up to the null terminator
        return System.Text.Encoding.UTF8.GetString(errorBuffer, 0, length);
    }

    /// <summary>
    /// Safely extract a UTF-8 string from an error buffer with maximum length limit
    /// </summary>
    /// <param name="errorBuffer">The error buffer from native code</param>
    /// <param name="maxLength">Maximum number of bytes to read</param>
    /// <returns>The extracted error string</returns>
    public static string SafeGetString(byte[] errorBuffer, int maxLength)
    {
        if (errorBuffer == null || errorBuffer.Length == 0)
            return string.Empty;

        // Find the first null byte within maxLength
        int searchLength = Math.Min(maxLength, errorBuffer.Length);
        int nullIndex = Array.IndexOf(errorBuffer, (byte)0, 0, searchLength);

        // If no null found, use searchLength
        int length = nullIndex >= 0 ? nullIndex : searchLength;

        // Extract only up to the null terminator or maxLength
        return System.Text.Encoding.UTF8.GetString(errorBuffer, 0, length);
    }

    /// <summary>
    /// Validate that a symbol array contains no null or empty elements
    /// </summary>
    /// <param name="symbols">The symbol array to validate</param>
    /// <param name="paramName">Parameter name for exception message</param>
    /// <exception cref="ArgumentException">If any symbol is null or empty</exception>
    /// <remarks>
    /// HIGH FIX: Prevents null strings from being passed to native code which may not handle them correctly
    /// </remarks>
    public static void ValidateSymbolArray(string[] symbols, string paramName = "symbols")
    {
        if (symbols == null)
            throw new ArgumentNullException(paramName);

        for (int i = 0; i < symbols.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(symbols[i]))
                throw new ArgumentException($"Symbol array contains null or empty element at index {i}", paramName);
        }
    }
}
