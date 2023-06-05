using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

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

        public static string ExtractExtension(string fileName)
            => fileName.Substring(fileName.LastIndexOf('.') + 1).ToUpper();

        public static bool IsNullOrWhiteSpace(this string str)
            => string.IsNullOrWhiteSpace(str);

        public static string ToFriendlyString(this SearchOption searchOption)
        {
            if (searchOption == SearchOption.AllDirectories)
                return "С подпапками";

            if (searchOption == SearchOption.TopDirectoryOnly)
                return "Без подпапок";
            
            return string.Empty;
        }

        public static SearchOption Next(this SearchOption searchOption)
        {
            if (searchOption == SearchOption.AllDirectories)
                return SearchOption.TopDirectoryOnly;

            return SearchOption.AllDirectories;
        }

        public static GenerateButtonColors Next(this GenerateButtonColors searchOption)
        {
            if (searchOption == GenerateButtonColors.Green)
                return GenerateButtonColors.Red;

            return GenerateButtonColors.Green;
        }

        public static void InvokeIfRequired(this Control control, Action action)
        {
            if (control.InvokeRequired)
            {
                control.Invoke(action);
                return;
            }

            action();
        }

        public static bool IsNotNullOrWhiteSpace(this string str)
            => !string.IsNullOrWhiteSpace(str);
    }
}