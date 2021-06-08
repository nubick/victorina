#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Victorina
{
    public class BuildNumberIncreasingSystem : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
            IncreaseBuildNumber(Static.DevSettings);
        }

        private void IncreaseBuildNumber(DevSettings devSettings)
        {
            Version currentVersion = new Version(Static.MajorVersion, Static.MinorVersion);
            Version lastBuildVersion = new Version(devSettings.LastMajorVersion, devSettings.LastMinorVersion);

            if (currentVersion > lastBuildVersion)
            {
                devSettings.LastMajorVersion = Static.MajorVersion;
                devSettings.LastMinorVersion = Static.MinorVersion;
                devSettings.BuildNumber = 0;
            }

            devSettings.BuildNumber++;
            Debug.Log($"Build number is increased: {devSettings.GetAppVersion()}");
            EditorUtility.SetDirty(devSettings);
        }
    }
}
#endif