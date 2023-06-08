using System;
using System.Drawing;

namespace Random_File_Opener_Win_Forms
{
    public sealed class GeneratedFileListItem
    {
        public string DisplayValue { get; set; }
        public string Path { get; private set; }
        public string FileName { get; private set; }

        public Bitmap[] Images { get; set; } = Array.Empty<Bitmap>();

        public static GeneratedFileListItem FromString(string s, string directory)
        {
            var lastIndex = s.LastIndexOf("\\", StringComparison.InvariantCulture);
            var fileName = Utilities.ExtractFileName(s, lastIndex);
            return new GeneratedFileListItem
            {
                Path = s,
                DisplayValue = fileName + " " + Utilities.ExtractDirectory(directory, s, lastIndex),
                FileName = fileName,
            };
        }

        public override string ToString()
            => DisplayValue;
    }
}