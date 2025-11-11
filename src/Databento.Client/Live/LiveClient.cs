using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Channels;
using Databento.Client.Events;
using Databento.Client.Models;
using Databento.Interop;
using Databento.Interop.Handles;
using Databento.Interop.Native;

namespace Databento.Client.Live;

/// <summary>
/// Live streaming client implementation
/// </summary>
public sealed class LiveClient : ILiveClient
{
    private readonly LiveClientHandle _handle;
    private readonly RecordCallbackDelegate _recordCallback;
    private readonly ErrorCallbackDelegate _errorCallback;
    private readonly Channel<Record> _recordChannel;
    private readonly CancellationTokenSource _cts;
    private readonly string? _defaultDataset;
    private readonly bool _sendTsOut;
    private readonly VersionUpgradePolicy _upgradePolicy;
    private readonly TimeSpan _heartbeatInterval;
    private Task? _streamTask;
    private bool _disposed;

    /// <summary>
    /// Event fired when data is received
    /// </summary>
    public event EventHandler<DataReceivedEventArgs>? DataReceived;

    /// <summary>
    /// Event fired when an error occurs
    /// </summary>
    public event EventHandler<Events.ErrorEventArgs>? ErrorOccurred;

    internal LiveClient(string apiKey)
        : this(apiKey, null, false, VersionUpgradePolicy.Upgrade, TimeSpan.FromSeconds(30))
    {
    }

    internal LiveClient(
        string apiKey,
        string? defaultDataset,
        bool sendTsOut,
        VersionUpgradePolicy upgradePolicy,
        TimeSpan heartbeatInterval)
    {
        if (string.IsNullOrEmpty(apiKey))
            throw new ArgumentException("API key cannot be null or empty", nameof(apiKey));

        _defaultDataset = defaultDataset;
        _sendTsOut = sendTsOut;
        _upgradePolicy = upgradePolicy;
        _heartbeatInterval = heartbeatInterval;

        // Create channel for streaming records
        _recordChannel = Channel.CreateUnbounded<Record>(new UnboundedChannelOptions
        {
            SingleReader = false,
            SingleWriter = false
        });

        _cts = new CancellationTokenSource();

        // Create callbacks (must be stored to prevent GC collection)
        unsafe
        {
            _recordCallback = OnRecordReceived;
            _errorCallback = OnErrorOccurred;
        }

        // Create native client
        byte[] errorBuffer = new byte[512];
        var handlePtr = NativeMethods.dbento_live_create(apiKey, errorBuffer, (nuint)errorBuffer.Length);

        if (handlePtr == IntPtr.Zero)
        {
            var error = System.Text.Encoding.UTF8.GetString(errorBuffer).TrimEnd('\0');
            throw new DbentoException($"Failed to create live client: {error}");
        }

        _handle = new LiveClientHandle(handlePtr);

        // Note: Dataset default, sendTsOut, upgrade policy, and heartbeat settings
        // are stored for future use when native layer supports configuration.
    }

    /// <summary>
    /// Subscribe to a data stream
    /// </summary>
    public Task SubscribeAsync(
        string dataset,
        Schema schema,
        IEnumerable<string> symbols,
        CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        var symbolArray = symbols.ToArray();
        byte[] errorBuffer = new byte[512];

        var result = NativeMethods.dbento_live_subscribe(
            _handle,
            dataset,
            schema.ToSchemaString(),
            symbolArray,
            (nuint)symbolArray.Length,
            errorBuffer,
            (nuint)errorBuffer.Length);

        if (result != 0)
        {
            var error = System.Text.Encoding.UTF8.GetString(errorBuffer).TrimEnd('\0');
            throw new DbentoException($"Subscription failed: {error}", result);
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Start receiving data
    /// </summary>
    public Task StartAsync(CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (_streamTask != null)
            throw new InvalidOperationException("Client is already started");

        // Start receiving on a background thread
        _streamTask = Task.Run(() =>
        {
            byte[] errorBuffer = new byte[512];

            var result = NativeMethods.dbento_live_start(
                _handle,
                _recordCallback,
                _errorCallback,
                IntPtr.Zero,
                errorBuffer,
                (nuint)errorBuffer.Length);

            if (result != 0)
            {
                var error = System.Text.Encoding.UTF8.GetString(errorBuffer).TrimEnd('\0');
                throw new DbentoException($"Start failed: {error}", result);
            }
        }, cancellationToken);

        return Task.CompletedTask;
    }

    /// <summary>
    /// Stop receiving data
    /// </summary>
    public Task StopAsync(CancellationToken cancellationToken = default)
    {
        if (!_disposed)
        {
            NativeMethods.dbento_live_stop(_handle);
            _recordChannel.Writer.Complete();
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Stream records as an async enumerable
    /// </summary>
    public async IAsyncEnumerable<Record> StreamAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await foreach (var record in _recordChannel.Reader.ReadAllAsync(cancellationToken))
        {
            yield return record;
        }
    }

    private unsafe void OnRecordReceived(byte* recordBytes, nuint recordLength, byte recordType, IntPtr userData)
    {
        try
        {
            // Copy bytes to managed memory
            var bytes = new byte[recordLength];
            Marshal.Copy((IntPtr)recordBytes, bytes, 0, (int)recordLength);

            // Deserialize record using the recordType parameter
            var record = Record.FromBytes(bytes, recordType);

            // Write to channel
            _recordChannel.Writer.TryWrite(record);

            // Fire event
            DataReceived?.Invoke(this, new DataReceivedEventArgs(record));
        }
        catch (Exception ex)
        {
            ErrorOccurred?.Invoke(this, new Events.ErrorEventArgs(ex));
        }
    }

    private void OnErrorOccurred(string errorMessage, int errorCode, IntPtr userData)
    {
        var exception = new DbentoException(errorMessage, errorCode);
        ErrorOccurred?.Invoke(this, new Events.ErrorEventArgs(exception, errorCode));
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;

        _disposed = true;

        // Stop streaming
        await StopAsync();

        // Cancel and wait for stream task
        _cts.Cancel();
        if (_streamTask != null)
        {
            try
            {
                await _streamTask;
            }
            catch (OperationCanceledException)
            {
                // Expected
            }
        }

        // Complete channel
        _recordChannel.Writer.Complete();

        // Dispose handle
        _handle?.Dispose();
        _cts?.Dispose();
    }
}
