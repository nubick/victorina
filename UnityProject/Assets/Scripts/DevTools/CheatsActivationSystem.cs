using System.Collections.Generic;
using Injection;
using UnityEngine;

namespace Victorina.DevTools
{
    public class CheatsActivationSystem : IKeyPressedHandler
    {
        [Inject] private DevToolsSystem DevToolsSystem { get; set; }
        
        private const int LastPressedKeyCodesMaxSize = 20;

        private readonly List<KeyCode> _lastPressedKeyCodes = new List<KeyCode>(LastPressedKeyCodesMaxSize);
        private readonly List<KeyCode> _showDebugConsoleCheatSequence = new List<KeyCode> {KeyCode.C, KeyCode.H, KeyCode.E, KeyCode.A, KeyCode.T, KeyCode.C, KeyCode.O, KeyCode.N, KeyCode.S, KeyCode.O, KeyCode.L, KeyCode.E};
        
        public void OnKeyPressed(KeyCode keyCode)
        {
            AddToLastPresses(keyCode);
            CheckOnCheats();
        }

        private void AddToLastPresses(KeyCode keyCode)
        {
            _lastPressedKeyCodes.Add(keyCode);
            while (_lastPressedKeyCodes.Count > LastPressedKeyCodesMaxSize)
                _lastPressedKeyCodes.RemoveAt(0);
        }
        
        private void CheckOnCheats()
        {
            if (HasMatch(_lastPressedKeyCodes, _showDebugConsoleCheatSequence))
            {
                Debug.Log("CHEAT: Enable debug console");
                DevToolsSystem.ActivateGameDebugConsole();
            }
        }

        private bool HasMatch(List<KeyCode> last, List<KeyCode> template)
        {
            if (last.Count < template.Count)
                return false;
            
            int diff = last.Count - template.Count;
            for (int i = 0; i < template.Count; i++)
            {
                if (last[i + diff] != template[i])
                    return false;
            }
            
            return true;
        }
    }
}