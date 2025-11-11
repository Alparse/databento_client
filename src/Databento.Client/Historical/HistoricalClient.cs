using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading.Channels;
using Databento.Client.Metadata;
using Databento.Client.Models;
using Databento.Client.Models.Metadata;
using Databento.Interop;
using Databento.Interop.Handles;
using Databento.Interop.Native;

namespace Databento.Client.Historical;

/// <summary>
/// Historical data client implementation
/// </summary>
public sealed class HistoricalClient : IHistoricalClient
{
    private readonly HistoricalClientHandle _handle;
    private readonly HistoricalGateway _gateway;
    private readonly string? _customHost;
    private readonly ushort? _customPort;
    private readonly VersionUpgradePolicy _upgradePolicy;
    private readonly string? _userAgent;
    private readonly TimeSpan _timeout;
    private bool _disposed;

    internal HistoricalClient(string apiKey)
        : this(apiKey, HistoricalGateway.Bo1, null, null, VersionUpgradePolicy.Upgrade, null, TimeSpan.FromSeconds(30))
    {
    }

    internal HistoricalClient(
        string apiKey,
        HistoricalGateway gateway,
        string? customHost,
        ushort? customPort,
        VersionUpgradePolicy upgradePolicy,
        string? userAgent,
        TimeSpan timeout)
    {
        if (string.IsNullOrEmpty(apiKey))
            throw new ArgumentException("API key cannot be null or empty", nameof(apiKey));

        _gateway = gateway;
        _customHost = customHost;
        _customPort = customPort;
        _upgradePolicy = upgradePolicy;
        _userAgent = userAgent;
        _timeout = timeout;

        byte[] errorBuffer = new byte[512];
        var handlePtr = NativeMethods.dbento_historical_create(apiKey, errorBuffer, (nuint)errorBuffer.Length);

        if (handlePtr == IntPtr.Zero)
        {
            var error = System.Text.Encoding.UTF8.GetString(errorBuffer).TrimEnd('\0');
            throw new DbentoException($"Failed to create historical client: {error}");
        }

        _handle = new HistoricalClientHandle(handlePtr);

        // Note: Gateway, upgrade policy, and other settings are stored for future use
        // when native layer supports configuration. For now, defaults are used.
    }

    /// <summary>
    /// Query historical data for a time range
    /// </summary>
    public async IAsyncEnumerable<Record> GetRangeAsync(
        string dataset,
        Schema schema,
        IEnumerable<string> symbols,
        DateTimeOffset startTime,
        DateTimeOffset endTime,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        var channel = Channel.CreateUnbounded<Record>();
        var symbolArray = symbols.ToArray();

        // Convert times to nanoseconds since epoch
        long startTimeNs = startTime.ToUnixTimeMilliseconds() * 1_000_000;
        long endTimeNs = endTime.ToUnixTimeMilliseconds() * 1_000_000;

        // Create callback delegates
        RecordCallbackDelegate recordCallback;
        unsafe
        {
            recordCallback = (recordBytes, recordLength, recordType, userData) =>
            {
            try
            {
                var bytes = new byte[recordLength];
                unsafe
                {
                    Marshal.Copy((IntPtr)recordBytes, bytes, 0, (int)recordLength);
                }

                var record = Record.FromBytes(bytes, recordType);
                channel.Writer.TryWrite(record);
            }
            catch (Exception)
            {
                // Log or handle error
            }
            };
        }

        // Start query on background thread
        var queryTask = Task.Run(() =>
        {
            try
            {
                byte[] errorBuffer = new byte[512];

                var result = NativeMethods.dbento_historical_get_range(
                    _handle,
                    dataset,
                    schema.ToSchemaString(),
                    symbolArray,
                    (nuint)symbolArray.Length,
                    startTimeNs,
                    endTimeNs,
                    recordCallback,
                    IntPtr.Zero,
                    errorBuffer,
                    (nuint)errorBuffer.Length);

                if (result != 0)
                {
                    var error = System.Text.Encoding.UTF8.GetString(errorBuffer).TrimEnd('\0');
                    throw new DbentoException($"Historical query failed: {error}", result);
                }
            }
            finally
            {
                channel.Writer.Complete();
            }
        }, cancellationToken);

        // Stream results
        await foreach (var record in channel.Reader.ReadAllAsync(cancellationToken))
        {
            yield return record;
        }

        await queryTask;
    }

    /// <summary>
    /// Get metadata for a historical query
    /// Note: This feature is currently not fully implemented in the native layer
    /// </summary>
    public IMetadata? GetMetadata(
        string dataset,
        Schema schema,
        DateTimeOffset startTime,
        DateTimeOffset endTime)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        // Convert times to nanoseconds since epoch
        long startTimeNs = startTime.ToUnixTimeMilliseconds() * 1_000_000;
        long endTimeNs = endTime.ToUnixTimeMilliseconds() * 1_000_000;

        byte[] errorBuffer = new byte[512];

        var metadataHandle = NativeMethods.dbento_historical_get_metadata(
            _handle,
            dataset,
            schema.ToSchemaString(),
            startTimeNs,
            endTimeNs,
            errorBuffer,
            (nuint)errorBuffer.Length);

        if (metadataHandle == IntPtr.Zero)
        {
            // Native layer doesn't support metadata-only queries yet
            return null;
        }

        return new Metadata.Metadata(new MetadataHandle(metadataHandle));
    }

    // ========================================================================
    // Metadata API Methods
    // ========================================================================

    /// <summary>
    /// List all publishers
    /// </summary>
    public async Task<IReadOnlyList<PublisherDetail>> ListPublishersAsync(
        CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        return await Task.Run(() =>
        {
            byte[] errorBuffer = new byte[512];
            var jsonPtr = NativeMethods.dbento_metadata_list_publishers(
                _handle, errorBuffer, (nuint)errorBuffer.Length);

            if (jsonPtr == IntPtr.Zero)
            {
                var error = System.Text.Encoding.UTF8.GetString(errorBuffer).TrimEnd('\0');
                throw new DbentoException($"Failed to list publishers: {error}");
            }

            try
            {
                var json = Marshal.PtrToStringUTF8(jsonPtr) ?? "[]";
                var publishers = JsonSerializer.Deserialize<List<PublisherDetail>>(json) ?? new List<PublisherDetail>();
                return (IReadOnlyList<PublisherDetail>)publishers;
            }
            finally
            {
                NativeMethods.dbento_free_string(jsonPtr);
            }
        }, cancellationToken);
    }

    /// <summary>
    /// List datasets, optionally filtered by venue
    /// </summary>
    public async Task<IReadOnlyList<string>> ListDatasetsAsync(
        string? venue = null,
        CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        return await Task.Run(() =>
        {
            byte[] errorBuffer = new byte[512];
            var jsonPtr = NativeMethods.dbento_metadata_list_datasets(
                _handle, venue, errorBuffer, (nuint)errorBuffer.Length);

            if (jsonPtr == IntPtr.Zero)
            {
                var error = System.Text.Encoding.UTF8.GetString(errorBuffer).TrimEnd('\0');
                throw new DbentoException($"Failed to list datasets: {error}");
            }

            try
            {
                var json = Marshal.PtrToStringUTF8(jsonPtr) ?? "[]";
                var datasets = JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>();
                return (IReadOnlyList<string>)datasets;
            }
            finally
            {
                NativeMethods.dbento_free_string(jsonPtr);
            }
        }, cancellationToken);
    }

    /// <summary>
    /// List schemas available for a dataset
    /// </summary>
    public async Task<IReadOnlyList<Schema>> ListSchemasAsync(
        string dataset,
        CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        return await Task.Run(() =>
        {
            byte[] errorBuffer = new byte[512];
            var jsonPtr = NativeMethods.dbento_metadata_list_schemas(
                _handle, dataset, errorBuffer, (nuint)errorBuffer.Length);

            if (jsonPtr == IntPtr.Zero)
            {
                var error = System.Text.Encoding.UTF8.GetString(errorBuffer).TrimEnd('\0');
                throw new DbentoException($"Failed to list schemas: {error}");
            }

            try
            {
                var json = Marshal.PtrToStringUTF8(jsonPtr) ?? "[]";
                var schemaStrings = JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>();
                var schemas = schemaStrings.Select(s => SchemaExtensions.ParseSchema(s)).ToList();
                return (IReadOnlyList<Schema>)schemas;
            }
            finally
            {
                NativeMethods.dbento_free_string(jsonPtr);
            }
        }, cancellationToken);
    }

    /// <summary>
    /// List fields for a given encoding and schema
    /// </summary>
    public async Task<IReadOnlyList<FieldDetail>> ListFieldsAsync(
        Encoding encoding,
        Schema schema,
        CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        return await Task.Run(() =>
        {
            byte[] errorBuffer = new byte[512];
            var jsonPtr = NativeMethods.dbento_metadata_list_fields(
                _handle,
                encoding.ToEncodingString(),
                schema.ToSchemaString(),
                errorBuffer,
                (nuint)errorBuffer.Length);

            if (jsonPtr == IntPtr.Zero)
            {
                var error = System.Text.Encoding.UTF8.GetString(errorBuffer).TrimEnd('\0');
                throw new DbentoException($"Failed to list fields: {error}");
            }

            try
            {
                var json = Marshal.PtrToStringUTF8(jsonPtr) ?? "[]";
                var fields = JsonSerializer.Deserialize<List<FieldDetail>>(json) ?? new List<FieldDetail>();
                return (IReadOnlyList<FieldDetail>)fields;
            }
            finally
            {
                NativeMethods.dbento_free_string(jsonPtr);
            }
        }, cancellationToken);
    }

    /// <summary>
    /// Get dataset availability condition
    /// </summary>
    public async Task<DatasetConditionInfo> GetDatasetConditionAsync(
        string dataset,
        CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        return await Task.Run(() =>
        {
            byte[] errorBuffer = new byte[512];
            var jsonPtr = NativeMethods.dbento_metadata_get_dataset_condition(
                _handle, dataset, errorBuffer, (nuint)errorBuffer.Length);

            if (jsonPtr == IntPtr.Zero)
            {
                var error = System.Text.Encoding.UTF8.GetString(errorBuffer).TrimEnd('\0');
                throw new DbentoException($"Failed to get dataset condition: {error}");
            }

            try
            {
                var json = Marshal.PtrToStringUTF8(jsonPtr) ?? "{}";
                return JsonSerializer.Deserialize<DatasetConditionInfo>(json)
                    ?? throw new DbentoException("Failed to deserialize dataset condition");
            }
            finally
            {
                NativeMethods.dbento_free_string(jsonPtr);
            }
        }, cancellationToken);
    }

    /// <summary>
    /// Get dataset time range
    /// </summary>
    public async Task<DatasetRange> GetDatasetRangeAsync(
        string dataset,
        CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        return await Task.Run(() =>
        {
            byte[] errorBuffer = new byte[512];
            var jsonPtr = NativeMethods.dbento_metadata_get_dataset_range(
                _handle, dataset, errorBuffer, (nuint)errorBuffer.Length);

            if (jsonPtr == IntPtr.Zero)
            {
                var error = System.Text.Encoding.UTF8.GetString(errorBuffer).TrimEnd('\0');
                throw new DbentoException($"Failed to get dataset range: {error}");
            }

            try
            {
                var json = Marshal.PtrToStringUTF8(jsonPtr) ?? "{}";
                return JsonSerializer.Deserialize<DatasetRange>(json)
                    ?? throw new DbentoException("Failed to deserialize dataset range");
            }
            finally
            {
                NativeMethods.dbento_free_string(jsonPtr);
            }
        }, cancellationToken);
    }

    /// <summary>
    /// Get record count for a query
    /// </summary>
    public async Task<ulong> GetRecordCountAsync(
        string dataset,
        Schema schema,
        DateTimeOffset startTime,
        DateTimeOffset endTime,
        IEnumerable<string> symbols,
        CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        var symbolArray = symbols.ToArray();
        long startTimeNs = startTime.ToUnixTimeMilliseconds() * 1_000_000;
        long endTimeNs = endTime.ToUnixTimeMilliseconds() * 1_000_000;

        return await Task.Run(() =>
        {
            byte[] errorBuffer = new byte[512];
            var count = NativeMethods.dbento_metadata_get_record_count(
                _handle,
                dataset,
                schema.ToSchemaString(),
                startTimeNs,
                endTimeNs,
                symbolArray,
                (nuint)symbolArray.Length,
                errorBuffer,
                (nuint)errorBuffer.Length);

            // If count is ulong.MaxValue, an error occurred
            if (count == ulong.MaxValue)
            {
                var error = System.Text.Encoding.UTF8.GetString(errorBuffer).TrimEnd('\0');
                throw new DbentoException($"Failed to get record count: {error}");
            }

            return count;
        }, cancellationToken);
    }

    /// <summary>
    /// Get billable size for a query
    /// </summary>
    public async Task<ulong> GetBillableSizeAsync(
        string dataset,
        Schema schema,
        DateTimeOffset startTime,
        DateTimeOffset endTime,
        IEnumerable<string> symbols,
        CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        var symbolArray = symbols.ToArray();
        long startTimeNs = startTime.ToUnixTimeMilliseconds() * 1_000_000;
        long endTimeNs = endTime.ToUnixTimeMilliseconds() * 1_000_000;

        return await Task.Run(() =>
        {
            byte[] errorBuffer = new byte[512];
            var size = NativeMethods.dbento_metadata_get_billable_size(
                _handle,
                dataset,
                schema.ToSchemaString(),
                startTimeNs,
                endTimeNs,
                symbolArray,
                (nuint)symbolArray.Length,
                errorBuffer,
                (nuint)errorBuffer.Length);

            // If size is ulong.MaxValue, an error occurred
            if (size == ulong.MaxValue)
            {
                var error = System.Text.Encoding.UTF8.GetString(errorBuffer).TrimEnd('\0');
                throw new DbentoException($"Failed to get billable size: {error}");
            }

            return size;
        }, cancellationToken);
    }

    /// <summary>
    /// Get cost estimate for a query
    /// </summary>
    public async Task<decimal> GetCostAsync(
        string dataset,
        Schema schema,
        DateTimeOffset startTime,
        DateTimeOffset endTime,
        IEnumerable<string> symbols,
        CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        var symbolArray = symbols.ToArray();
        long startTimeNs = startTime.ToUnixTimeMilliseconds() * 1_000_000;
        long endTimeNs = endTime.ToUnixTimeMilliseconds() * 1_000_000;

        return await Task.Run(() =>
        {
            byte[] errorBuffer = new byte[512];
            var costPtr = NativeMethods.dbento_metadata_get_cost(
                _handle,
                dataset,
                schema.ToSchemaString(),
                startTimeNs,
                endTimeNs,
                symbolArray,
                (nuint)symbolArray.Length,
                errorBuffer,
                (nuint)errorBuffer.Length);

            if (costPtr == IntPtr.Zero)
            {
                var error = System.Text.Encoding.UTF8.GetString(errorBuffer).TrimEnd('\0');
                throw new DbentoException($"Failed to get cost: {error}");
            }

            try
            {
                var costString = Marshal.PtrToStringUTF8(costPtr) ?? "0";
                return decimal.Parse(costString);
            }
            finally
            {
                NativeMethods.dbento_free_string(costPtr);
            }
        }, cancellationToken);
    }

    /// <summary>
    /// Get combined billing information for a query
    /// </summary>
    public async Task<BillingInfo> GetBillingInfoAsync(
        string dataset,
        Schema schema,
        DateTimeOffset startTime,
        DateTimeOffset endTime,
        IEnumerable<string> symbols,
        CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        var symbolArray = symbols.ToArray();
        long startTimeNs = startTime.ToUnixTimeMilliseconds() * 1_000_000;
        long endTimeNs = endTime.ToUnixTimeMilliseconds() * 1_000_000;

        return await Task.Run(() =>
        {
            byte[] errorBuffer = new byte[512];
            var jsonPtr = NativeMethods.dbento_metadata_get_billing_info(
                _handle,
                dataset,
                schema.ToSchemaString(),
                startTimeNs,
                endTimeNs,
                symbolArray,
                (nuint)symbolArray.Length,
                errorBuffer,
                (nuint)errorBuffer.Length);

            if (jsonPtr == IntPtr.Zero)
            {
                var error = System.Text.Encoding.UTF8.GetString(errorBuffer).TrimEnd('\0');
                throw new DbentoException($"Failed to get billing info: {error}");
            }

            try
            {
                var json = Marshal.PtrToStringUTF8(jsonPtr) ?? "{}";
                return JsonSerializer.Deserialize<BillingInfo>(json)
                    ?? throw new DbentoException("Failed to deserialize billing info");
            }
            finally
            {
                NativeMethods.dbento_free_string(jsonPtr);
            }
        }, cancellationToken);
    }

    public ValueTask DisposeAsync()
    {
        if (_disposed) return ValueTask.CompletedTask;

        _disposed = true;
        _handle?.Dispose();

        return ValueTask.CompletedTask;
    }
}
