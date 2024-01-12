using System;
using System.Drawing;
using System.IO;

namespace Random_File_Opener_Win_Forms
{
    public sealed class GeneratedFileListItem
    {
        public string DisplayValue { get; private set; }
        public string PathToFile { get; private set; }

        // public List<string> Directories { get; private set; }
        public string Extension { get; private set; }
        public string FileName { get; private set; }

        public Bitmap[] Images { get; set; } = Array.Empty<Bitmap>();

        public bool AddedToListBox { get; set; } = false;

        public static GeneratedFileListItem FromString(string s, string directory)
        {
            var lastIndexOfSlash = s.LastIndexOf("\\", StringComparison.InvariantCulture);

            var fileName = Utilities.ExtractFileName(s, lastIndexOfSlash);
            var directories = Utilities.ExtractDirectory(directory, s, lastIndexOfSlash);
            var extension = Path.GetExtension(fileName);

            return new GeneratedFileListItem
            {
                PathToFile = s,
                DisplayValue = fileName + (directories.IsNotNullOrWhiteSpace() ? " (" + directories + ")" : string.Empty),
                // Directories = directories == string.Empty
                // ? new List<string>()
                // : directories.Split('\\').ToList(),
                Extension = extension,
                FileName = fileName,
            };
        }

        public override string ToString()
            => DisplayValue;
    }
}