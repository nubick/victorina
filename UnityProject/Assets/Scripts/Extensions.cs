namespace Victorina
{
    public static class Extensions
    {
        public static string SizeKb(this byte[] bytes)
        {
            return $"{bytes.Length / 1024}kb";
        }
    }
}