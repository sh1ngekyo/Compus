namespace Compus.Domain.Server;

public class ServerConfig
{
    public User[]? Users { get; set; }
    public int MaxIdleMinutes { get; set; }
    public bool EnableAuthorization { get; set; }
}
