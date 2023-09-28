namespace Compus.Domain.Shared;

/// <summary>
/// Shared Constants
/// </summary>
public static class Constants
{
    /// <summary>
    /// ClientSessionId
    /// </summary>
    public const string ClientSessionIdName = "ClientSessionId";

    /// <summary>
    /// Maxinum lines for terminal
    /// </summary>
    public const int MaxinumLines = 1000;

    /// <summary>
    /// Maxinum queue buffer size for one connection
    /// </summary>
    public const int MaxinumQueueCount = 10_000;

    /// <summary>
    /// Maxinum terminal view buffer size for one connection
    /// </summary>
    public const int MaxinumOutputLength = 100_000;

    /// <summary>
    /// New line for terminal
    /// </summary>
    public const string NewLine = "\r\n";

    /// <summary>
    /// Default SSH port
    /// </summary>
    public const int DefaultPort = 22;
}
