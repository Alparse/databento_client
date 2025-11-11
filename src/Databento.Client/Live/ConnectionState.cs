namespace Databento.Client.Live;

/// <summary>
/// Connection state for live streaming client
/// </summary>
public enum ConnectionState
{
    /// <summary>Not connected to gateway</summary>
    Disconnected,

    /// <summary>Connecting to gateway</summary>
    Connecting,

    /// <summary>Connected and authenticated</summary>
    Connected,

    /// <summary>Actively streaming data</summary>
    Streaming,

    /// <summary>Reconnecting after disconnection</summary>
    Reconnecting,

    /// <summary>Stopped by user</summary>
    Stopped
}
