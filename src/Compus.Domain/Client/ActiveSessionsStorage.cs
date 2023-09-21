using System;
using System.Collections.Concurrent;

namespace Compus.Domain.Client;

public class ActiveSessionsStorage
{
    public ConcurrentDictionary<Guid, ActiveSession> Sessions { get; set; } = new();
}
