using System;
using System.Collections.Generic;

namespace Victorina
{
    public abstract class ServerEventBase
    {
        public string EventId { get; }

        protected ServerEventBase(string eventId)
        {
            EventId = eventId;
        }
        
        public override string ToString()
        {
            return $"ServerEvent: {EventId}";
        }
    }
    
    public class ServerEvent : ServerEventBase
    {
        private readonly List<Action> _callbacks = new List<Action>();
        
        public ServerEvent(string eventId): base(eventId) { }
        
        public void Subscribe(Action callback)
        {
            _callbacks.Add(callback);
        }

        public void Publish()
        {
            for (int i = 0; i < _callbacks.Count; i++)
                _callbacks[i]();
        }
    }

    public class ServerEvent<TArgs> : ServerEventBase
    {
        private readonly List<Action<TArgs>> _callbacks = new List<Action<TArgs>>();

        public ServerEvent(string eventId) : base(eventId) { }
        
        public void Subscribe(Action<TArgs> callback)
        {
            _callbacks.Add(callback);
        }

        public void Publish(TArgs eventArgs)
        {
            for (int i = 0; i < _callbacks.Count; i++)
                _callbacks[i](eventArgs);
        }
    }
}