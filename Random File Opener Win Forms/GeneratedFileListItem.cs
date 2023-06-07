using System;
using System.Drawing;

namespace Random_File_Opener_Win_Forms
{
    public sealed class GeneratedFileListItem
    {
        public string DisplayValue { get; set; }
        public string Path { get; set; }
        public string FileName { get; set; }

        public Bitmap[] Images { get; set; } = Array.Empty<Bitmap>();

        public override string ToString()
            => DisplayValue;
    }
}