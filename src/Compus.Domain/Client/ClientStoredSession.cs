namespace Compus.Domain.Client;
public class ClientStoredSession
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string? DisplayName { get; set; }
    public string? Host { get; set; }
    public int Port { get; set; } = 22;
    public string? UserName { get; set; }
    public string? Password { get; set; }
    public byte[]? FingerPrint { get; set; }
    public byte[]? LoginKey { get; set; }
}
