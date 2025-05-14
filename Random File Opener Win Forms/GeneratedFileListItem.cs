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

        public string Directory { get; private set; }

        public Bitmap[] Images { get; set; } = Array.Empty<Bitmap>();

        public bool AddedToListBox { get; set; } = false;

        public static GeneratedFileListItem FromString(string s, string directory)
        {
            var lastIndexOfSlash = s.LastIndexOf("\\", StringComparison.InvariantCulture);

            var fileName = Utilities.ExtractFileName(s, lastIndexOfSlash);
            var directories = Utilities.ExtractDirectory(directory, s, lastIndexOfSlash);
            var extension = Utilities.ExtractExtension(fileName);

            return new GeneratedFileListItem
            {
                PathToFile = s,
                DisplayValue = fileName + (directories.IsNotNullOrWhiteSpace() ? " (" + directories + ")" : string.Empty),
                Directory = directories,
                Extension = extension,
                FileName = fileName,
            };
        }

        public override string ToString()
            => DisplayValue;

        public string GetPathForHash(string cacheDirectory, int index)
        {
            var fileName = FileName + "-" + index;
            var hash = Utilities.GetSha256Hash(fileName);

            var filePath = cacheDirectory + $"\\{hash}";
            return filePath;
        }
    }
}