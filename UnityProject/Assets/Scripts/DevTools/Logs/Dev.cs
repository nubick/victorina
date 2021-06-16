using UnityEngine;

namespace Victorina.DevTools
{
    public static class Dev
    {
        public static void Log(string message, Color color)
        {
#if UNITY_EDITOR
            Debug.Log($"<color={color}>{message}</color>");
#else
            Debug.Log(message);
#endif
        }
    }
}