namespace Compus.Domain.Server;

/// <summary>
/// API user
/// </summary>
public class User
{
    /// <summary>
    /// User Id
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// User Name
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// User Password
    /// </summary>
    public string? Password { get; set; }
}
