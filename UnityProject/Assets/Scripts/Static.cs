using System;
using UnityEngine;

namespace Victorina
{
    public static class Static
    {
        public static int Port = 7000;
        public static string LocalhostGameCode = "XEAAAABA";
        public static float TimeForAnswer = 15f;

        public static readonly Version Version = new Version(0, 1, 4);
        public static BuildMode BuildMode => BuildMode.Development;
        
#if UNITY_EDITOR
        public static string DataPath => $"{Application.persistentDataPath}/Editor";
#else
        public static string DataPath => Application.persistentDataPath;
#endif

        
    }
}