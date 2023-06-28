﻿using System;
using System.Drawing;

namespace Random_File_Opener_Win_Forms
{
    public sealed class GeneratedFileListItem
    {
        public string DisplayValue { get; private set; }
        public string Path { get; private set; }
        public string Directories { get; private set; }
        public string Extension { get; private set; }
        public string FileName { get; private set; }

        public Bitmap[] Images { get; set; } = Array.Empty<Bitmap>();

        public static GeneratedFileListItem FromString(string s, string directory)
        {
            var lastIndexOfSlash = s.LastIndexOf("\\", StringComparison.InvariantCulture);

            var fileName = Utilities.ExtractFileName(s, lastIndexOfSlash);
            var directories = Utilities.ExtractDirectory(directory, s, lastIndexOfSlash);
            var extension = Utilities.ExtractExtension(fileName);

            return new GeneratedFileListItem
            {
                Path = s,
                DisplayValue = fileName + " (" + directories + ")",
                Directories = directories,
                Extension = extension,
                FileName = fileName,
            };
        }

        public override string ToString()
            => DisplayValue;
    }
}