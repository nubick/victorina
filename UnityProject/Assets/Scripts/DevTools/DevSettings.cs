using System;
using UnityEngine;

namespace Victorina
{
    public class DevSettings : ScriptableObject
    {
        public int LastMajorVersion;
        public int LastMinorVersion;
        public int BuildNumber;

        public BuildMode BuildMode;

        public string MinSupportedClientVersion;
        
        public Version GetAppVersion()
        {
            return new Version(LastMajorVersion, LastMinorVersion, BuildNumber);
        }

        public Version GetMinSupportedClientVersion()
        {
            Version version = GetAppVersion();
            if (Version.TryParse(MinSupportedClientVersion, out Version parsedVersion))
                version = parsedVersion;
            else
                Debug.LogWarning($"Can't parse min supported client version: {MinSupportedClientVersion}");
            return version;
        }
    }
}