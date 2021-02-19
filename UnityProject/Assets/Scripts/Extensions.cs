using System.Collections.Generic;

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
    }
}