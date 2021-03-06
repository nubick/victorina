using System;
using UnityEngine;

namespace Victorina
{
    public class DevSettings : ScriptableObject
    {
        public int LastMajorVersion;
        public int LastMinorVersion;
        public int BuildNumber;

        public Version GetVersion()
        {
            return new Version(LastMajorVersion, LastMinorVersion, BuildNumber);
        }
    }
}