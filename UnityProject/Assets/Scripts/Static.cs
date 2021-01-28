using UnityEngine;

namespace Victorina
{
    public static class Static
    {
        public static int Port = 139;
        public static string LocalhostGameCode = "XEAAAABA";

#if UNITY_EDITOR
        public static string DataPath => $"{Application.persistentDataPath}/Editor";
#else
        public static string DataPath => Application.persistentDataPath;
#endif

        public static BuildMode BuildMode => BuildMode.Development;
    }
}