using System.Collections.Generic;

namespace Victorina
{
    public class ServerEventsData
    {
        public Dictionary<string, ServerGameEvent> RegisteredEvents { get; } = new Dictionary<string, ServerGameEvent>();
        public Queue<string> PendingToSendEventIds { get; } = new Queue<string>();
    }
}