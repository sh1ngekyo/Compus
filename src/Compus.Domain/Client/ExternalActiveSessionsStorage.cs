using System;
using System.Collections.Concurrent;

namespace Compus.Domain.Client;

public class ExternalActiveSessionsStorage
{
    public ConcurrentDictionary<Guid, ExternalActiveSession> Sessions { get; set; } = new();
}
