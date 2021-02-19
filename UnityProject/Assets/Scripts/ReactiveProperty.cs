using System;
using System.Collections.Generic;

namespace Victorina
{
    public class ReactiveProperty<T>
    {
        private readonly List<Action> _subscriberActions = new List<Action>();
        private readonly List<Action<T>> _subscriberGenericActions = new List<Action<T>>();

        private T _value;
        public T Value
        {
            get => _value;
            set
            {
                _value = value;
                NotifyChanged();
                NotifyChanged(value);
            }
        }

        public void SubscribeChanged(Action action)
        {
            _subscriberActions.Add(action);
        }

        public void SubscribeChanged(Action<T> action)
        {
            _subscriberGenericActions.Add(action);
        }
        
        public void NotifyChanged()
        {
            foreach (Action subscribeAction in _subscriberActions)
                subscribeAction?.Invoke();
        }

        public void NotifyChanged(T value)
        {
            foreach(Action<T> subscribeAction in _subscriberGenericActions)
                subscribeAction?.Invoke(value);
        }

        public override string ToString()
        {
            return $"{Value}";
        }
    }
}