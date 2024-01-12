using System;
using System.Collections.Generic;
using System.Drawing;
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

            return str;
        }

        public static string ExtractFileName(string u, int lastIndex)
            => u.Substring(lastIndex + 1);

        public static string ExtractExtension(string fileName)
        {
            var lastIndexOfDot = fileName.LastIndexOf('.');
            if (lastIndexOfDot == -1)
                return string.Empty;

            return fileName.Substring(lastIndexOfDot + 1).ToUpper();
        }

        public static string RemoveTrailingSlash(string substring)
        {
            if (substring.IsNotNullOrWhiteSpace() && substring.Last() == '\\')
                return substring.Substring(0, substring.Length - 1);

            return substring;
        }

        public static GeneratedFileListItem ListItemFromPoint(ListBox listBox, Point point)
        {
            var indexFromPoint = listBox.IndexFromPoint(point);
            if (indexFromPoint == -1)
                return null;

            var listItem = (GeneratedFileListItem)listBox.Items[indexFromPoint];
            return listItem;
        }

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

        public static bool IsEmpty<T>(this IEnumerable<T> source)
            => source == null || !source.Any();

        public static bool IsNotEmpty<T>(this IEnumerable<T> source)
            => !source.IsEmpty();

        public static IEnumerable<T> PadRightWithNulls<T>(this ICollection<T> source, int desiredSize) where T : class
        {
            if (source.Count >= desiredSize)
                return source;

            var countOfElementsToAdd = desiredSize - source.Count;

            var padded = source.Concat(Enumerable.Range(0, countOfElementsToAdd).Select(_ => (T)null));
            return padded;
        }

        public static List<T> Shuffle<T>(this List<T> array, Random rng)
        {
            var n = array.Count;
            while (n > 1)
            {
                var k = rng.Next(n--);
                (array[n], array[k]) = (array[k], array[n]);
            }

            return array;
        }

        public static GeneratedFileListItem SelectedFile(this ListBox lb)
            => (GeneratedFileListItem)lb.SelectedItem;

        public static void ForAll<T>(this IEnumerable<T> sequence, Action<T> action)
        {
            foreach (var x in sequence)
            {
                action(x);
            }
        }
    }
}