using System;
using System.Collections.Generic;

namespace Random_File_Opener_Win_Forms
{
    internal static class Consts
    {
        public static HashSet<string> ImageExtensions { get; } = new HashSet<string>
        {
            "JPG", "JPEG", "JPE", "BMP", "GIF", "PNG",
        };

        public static HashSet<string> VideoExtensions { get; } = new HashSet<string>
        {
            "MP4", "MKV", "WMV", "AVI", "TS"
        };
        
        public static string EmptyFilter { get; } = "*";
        public static string SettingsFileName { get; } = "!appsettings.json";

        public static TimeSpan[] VideoThumbnailPositions { get; set; } = {
            TimeSpan.FromMinutes(1),
            TimeSpan.FromMinutes(12),
            TimeSpan.FromMinutes(20)
        };

        public static bool CacheImages { get; set; } = true;
    }

    internal enum GenerateButtonColors
    {
        Green = 0,
        Red = 1
    }
}