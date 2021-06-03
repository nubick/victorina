using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

namespace Victorina
{
    public static class Extensions
    {
        public static string SizeKb(this byte[] bytes)
        {
            return $"{bytes.Length / 1024}kb";
        }

        public static void Rewrite<T>(this List<T> list, IEnumerable<T> items)
        {
            list.Clear();
            list.AddRange(items);
        }

        public static bool IsFinished(this VideoPlayer videoPlayer)
        {
            Debug.Log($"frame: {videoPlayer.frame}, count: {videoPlayer.frameCount}");
            return videoPlayer.frame >= (long) videoPlayer.frameCount - 1;
        }

        public static void AddRange<T>(this HashSet<T> hashSet, IEnumerable<T> items)
        {
            foreach (T item in items)
                hashSet.Add(item);
        }
    }
}