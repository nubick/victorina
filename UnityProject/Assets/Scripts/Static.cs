using System.Linq;
using UnityEngine;
using Victorina.DevTools;

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

        public static string SupportMail = "vumka.game@gmail.com";
        public static string DiscordInviteLink = "https://discord.gg/VKSTCw6zQe";
        public static string DownloadPacksUrl = "https://sigame.ru/";
        
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

        private static DebugSettings _debugSettings;
        public static DebugSettings DebugSettings
        {
            get
            {
                if (_debugSettings == null)
                    _debugSettings = Resources.LoadAll<DebugSettings>(string.Empty).Single();
                return _debugSettings;
            }
        }
    }
}