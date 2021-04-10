using System.Linq;
using UnityEngine;

namespace Victorina
{
    public static class Static
    {
        public static int Port = 7000;
        public static string LocalhostGameCode = "XEAAAABA";
        public static float TimeForAnswer = 15f;

        public static int MajorVersion = 0;
        public static int MinorVersion = 2;

        public static string EmptyPlayerName = "Баба-Яга";
        
        public static BuildMode BuildMode => DevSettings.BuildMode;

        public static int AuctionMinStep = 100;
        
#if UNITY_EDITOR
        public static string DataPath => $"{Application.persistentDataPath}/Editor";
#else
        public static string DataPath => Application.persistentDataPath;
#endif
        
        private static DevSettings _devSettings;
        public static DevSettings DevSettings
        {
            get
            {
                if (_devSettings == null)
                    _devSettings = Resources.LoadAll<DevSettings>(string.Empty).Single();
                return _devSettings;
            }
        }
    }
}