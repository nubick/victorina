using System.Collections.Generic;

namespace Victorina
{
    public class ServerEventsData
    {
        public Dictionary<string, ServerEventBase> RegisteredEvents { get; } = new Dictionary<string, ServerEventBase>();
        public Queue<(string, ServerEventArgument)> PendingToSendEvents { get; } = new Queue<(string, ServerEventArgument)>();
    }
}