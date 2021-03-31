using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Victorina
{
    public class KeyboardManager : MonoBehaviour
    {
        private KeyCode[] _keyCodesCache;
        private readonly List<IKeyPressedHandler> _handlers = new List<IKeyPressedHandler>();
        
        public void Awake()
        {
            _keyCodesCache = Enum.GetValues(typeof(KeyCode)).Cast<KeyCode>().ToArray();
        }

        public void Register(IKeyPressedHandler handlers)
        {
            _handlers.Add(handlers);
        }
        
        public void Update()
        {
            foreach (KeyCode keyCode in _keyCodesCache)
                if (Input.GetKeyUp(keyCode))
                    OnKeyPressed(keyCode);
        }

        private void OnKeyPressed(KeyCode keyCode)
        {
            foreach (IKeyPressedHandler handler in _handlers)
            {
                try
                {
                    handler.OnKeyPressed(keyCode);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }
    }
}