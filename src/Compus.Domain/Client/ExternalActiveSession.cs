using System.Text.Json.Serialization;

namespace Compus.Domain.Client;

public class ExternalActiveSession
{
    public ExternalStoredSession? StoredSession { get; set; }

    public Guid ConnectionId { get; set; } = Guid.NewGuid();

    public string? Status { get; set; }

    [JsonIgnore]
    public string? OutputStr { get; set; }

    public DateTime StartSessionDate { get; set; }
}
