using System;
using System.Collections.Generic;

namespace Victorina
{
    public class ReactiveProperty<T>
    {
        private readonly List<Action> _subscriberActions = new List<Action>();

        private T _value;
        public T Value
        {
            get => _value;
            set
            {
                _value = value;
                NotifyChanged();
            }
        }

        public void SubscribeChanged(Action action)
        {
            _subscriberActions.Add(action);
        }

        public void NotifyChanged()
        {
            foreach (Action subscribeAction in _subscriberActions)
                subscribeAction?.Invoke();
        }

        public override string ToString()
        {
            return $"{Value}";
        }
    }
}