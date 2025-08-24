using System;
using System.Collections.Generic;
using System.Security.Cryptography;

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

        public static TimeSpan[] VideoThumbnailPositions { get; set; } = Array.Empty<TimeSpan>();
        public static bool RetainFileOnListClear { get; set; }

        public static CacheSettings Cache { get; set; } = new CacheSettings();
        public static HashAlgorithm HashAlgorithm = HashAlgorithm.Create("SHA256");
    }

    public class CacheSettings
    {
        public bool SaveCacheOnDisc { get; set; } = true;
        public string CacheLocation { get; set; } = "Cache";
    }

    internal enum GenerateButtonColors
    {
        Green = 0,
        Red = 1
    }
}