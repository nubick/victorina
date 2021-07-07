using UnityEngine;

namespace Victorina.DevTools
{
    public static class Dev
    {
        public static void Log(string message, Color color)
        {
#if UNITY_EDITOR
            string colorStr = ColorUtility.ToHtmlStringRGB(color);
            Debug.Log($"<color=#{colorStr}>{message}</color>");
#else
            Debug.Log(message);
#endif
        }
    }
}