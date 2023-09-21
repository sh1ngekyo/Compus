using Compus.Domain.Server.Enums;

namespace Compus.Domain.Client;

public class ClientLoginModel
{
    public Guid Id { get; set; }
    public string? UserName { get; set; }
    public string? Password { get; set; }
    public bool Persist { get; set; }
    public string? Captcha { get; set; }
    public SignInStatus Status { get; set; }
    public string? Message { get; set; }
}
