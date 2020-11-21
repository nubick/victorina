using System;
using System.Collections.Generic;

namespace Assets.Scripts.Utils
{
    public class GameEvent
    {
        private readonly List<Action> _callbacks = new List<Action>();
        
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

    public class GameEvent<TArgs>
    {
        private readonly List<Action<TArgs>> _callbacks = new List<Action<TArgs>>();
        
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

    public class GameEvent<TArgs1, TArgs2>
    {
        private readonly List<Action<TArgs1, TArgs2>> _callbacks = new List<Action<TArgs1, TArgs2>>();
        
        public void Subscribe(Action<TArgs1, TArgs2> callback)
        {
            _callbacks.Add(callback);
        }

        public void Publish(TArgs1 args1, TArgs2 args2)
        {
            for (int i = 0; i < _callbacks.Count; i++)
                _callbacks[i](args1, args2);
        }
    }
}
