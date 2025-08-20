using System;
using System.Drawing;
using System.IO;
using System.Linq;

namespace Random_File_Opener_Win_Forms
{
    public sealed class GeneratedFileListItem : ISoftDeleteable
    {
        public string DisplayValue { get; private set; }
        public string PathToFile { get; private set; }

        // public List<string> Directories { get; private set; }
        public string Extension { get; private set; }
        public string FileName { get; private set; }

        public string Directory { get; private set; }

        public Bitmap[] Images { get; set; } = Array.Empty<Bitmap>();

        public bool AddedToListBox { get; set; } = false;
        public double LengthMegabytes { get; set; }
        public bool IsDeleted { get; set; }

        public static GeneratedFileListItem FromString(FileInfo fileInfo, string directory)
        {
            var lastIndexOfSlash = fileInfo.FullName.LastIndexOf("\\", StringComparison.InvariantCulture);

            var directories = Utilities.ExtractDirectory(directory, fileInfo.FullName, lastIndexOfSlash);
            var extension = fileInfo.Extension.IsNullOrWhiteSpace() ? string.Empty : fileInfo.Extension.Substring(1).ToUpper();

            var directoriesPart = directories.IsNotNullOrWhiteSpace() ? " (" + directories + ")" : string.Empty;
            var fileLengthMegabytes = Math.Round(fileInfo.Length / 1e+6, 2);

            return new GeneratedFileListItem
            {
                PathToFile = fileInfo.FullName,
                DisplayValue = $"{fileInfo.Name}({fileLengthMegabytes}MB){directoriesPart}",
                Directory = directories,
                Extension = extension,
                FileName = fileInfo.Name,
                LengthMegabytes = fileLengthMegabytes,
            };
        }

        public override string ToString()
            => DisplayValue;

        public string Dump()
            =>$"{DisplayValue}, Images: {Images.Length}, AddedToListBox: {AddedToListBox}";

        public string GetPathForHash(string cacheDirectory, int index)
        {
            var fileName = FileName + "-" + index;
            var hash = Utilities.GetSha256Hash(fileName);

            var filePath = cacheDirectory + $"\\{hash}";
            return filePath;
        }
    }
}