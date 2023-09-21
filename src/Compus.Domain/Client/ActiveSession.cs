using System.Text.Json.Serialization;

namespace Compus.Domain.Client;

public class ActiveSession
{
    public ClientStoredSession? StoredSession { get; set; }
    public Guid Id { get; set; } = Guid.NewGuid();
    public string? Status { get; set; }
    [JsonIgnore]
    public string? Output { get; set; }
    public DateTime StartSessionDate { get; set; }
}
