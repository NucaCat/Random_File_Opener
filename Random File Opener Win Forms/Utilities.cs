using System.Linq;

namespace Random_File_Opener_Win_Forms
{
    internal static class Utilities
    {
        public static string ExtractDirectory(string directory, string u, int lastIndex)
        {
            var str = RemoveTrailingSlash(u.Substring(directory.Length + 1, lastIndex - directory.Length));
            if (str.IsNullOrWhiteSpace())
                return string.Empty;

            return $"({str})";
        }

        public static string ExtractFileName(string u, int lastIndex)
            => u.Substring(lastIndex + 1);

        public static string RemoveTrailingSlash(string substring)
        {
            if (substring.IsNotNullOrWhiteSpace() && substring.Last() == '\\')
                return substring.Substring(0, substring.Length - 1);

            return substring;
        }

        public static bool IsNullOrWhiteSpace(this string str)
            => string.IsNullOrWhiteSpace(str);

        public static bool IsNotNullOrWhiteSpace(this string str)
            => !string.IsNullOrWhiteSpace(str);
    }
}