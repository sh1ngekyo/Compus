using System.Text;
using Compus.Domain.Client;
using Compus.Domain.Shared;

namespace Compus.Web.Client;

public static class TerminalUtilities
{
    public static ExternalStoredSession? StoredSession { get; set; }
    public static ExternalSessionStorage SessionStorage { get; set; } = new();
    private static Dictionary<Guid, StringBuilder> TerminalOutput { get; set; } = new();
    private static Dictionary<Guid, List<string>> SendedCommands { get; set; } = new();
    private static string[] SplitByLines(this string str)
        => str?.Replace("\r\n", "\n").Split("\n") ?? Array.Empty<string>();

    public static void SaveTerminalOutput(Guid sessionId, string message)
    {
        if (!TerminalOutput.TryGetValue(sessionId, out var output))
        {
            output = new StringBuilder();
            TerminalOutput.Add(sessionId, output);
        }

        output.Append(message);

        if (output.Length > Constants.MaxinumOutputLength)
        {
            output.Remove(0, output.Length - Constants.MaxinumOutputLength);
        }
    }

    public static string GetTerminalOutput(Guid sessionId, ref int currentIndex)
    {
        TerminalOutput.TryGetValue(sessionId, out var builder);

        var output = new string(builder?.ToString().Skip(currentIndex).ToArray() ?? Array.Empty<char>());
        currentIndex += output.Length;

        return output;
    }

    public static (string CommandName, bool Successful) GetPrevCommand(Guid sessionId, ref int currentCommandLine)
    {
        if (SendedCommands.TryGetValue(sessionId, out var commands) && 
            commands.Count > 0 &&
            currentCommandLine > 1)
        {
            currentCommandLine--;
            return (commands.Skip(currentCommandLine - 1).FirstOrDefault(), true)!;
        }

        return (null, false)!;
    }

    public static (string CommandName, bool Successful) GetNextCommand(Guid sessionId, ref int currentCommandLine)
    {
        if (SendedCommands.TryGetValue(sessionId, out var commands) && 
            commands.Count > currentCommandLine && 
            currentCommandLine > 0)
        {
            currentCommandLine++;
            return (commands.Skip(currentCommandLine - 1).FirstOrDefault(), true)!;
        }

        return (null, false)!;
    }

    public static void SaveCommand(Guid sessionId, string commandName, ref int currentLineIndex)
    {
        if (!SendedCommands.TryGetValue(sessionId, out var commands))
        {
            commands = new List<string>();
            SendedCommands.Add(sessionId, commands);
        }

        var splitedCommands = commandName.SplitByLines();

        commands.AddRange(splitedCommands);

        if (commands.Count > Constants.MaxinumLines)
        {
            commands.RemoveRange(0, commands.Count - Constants.MaxinumLines);
        }

        currentLineIndex = commands.Count + 1;
    }

    public static int GetCurrentLineIndex(Guid sessionId) 
        => !SendedCommands.TryGetValue(sessionId, out var commands) ? 1 : commands.Count + 1;

    public static void Clear(Guid sessionId)
    {
        TerminalOutput.Remove(sessionId);
        SendedCommands.Remove(sessionId);
    }
}
