namespace Compus.Domain.Server;

/// <summary>
/// Terminal viewport content 
/// </summary>
public class TerminalContent
{
    /// <summary>
    /// Content as single string
    /// </summary>
    public string? Content { get; set; }

    /// <summary>
    /// Content lines
    /// </summary>
    public int Lines { get; set; }
}
