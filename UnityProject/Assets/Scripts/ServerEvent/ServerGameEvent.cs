using Assets.Scripts.Utils;

namespace Victorina
{
    public class ServerGameEvent : GameEvent
    {
        public string EventId { get; }
        
        public ServerGameEvent(string eventId)
        {
            EventId = eventId;
        }

        public override string ToString()
        {
            return $"ServerEvent: {EventId}";
        }
    }
}