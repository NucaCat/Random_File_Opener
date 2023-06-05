using System;
using System.Collections.Generic;
using System.Drawing;

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
            "MP4", "MKV",
        };
        
        public static string EmptyFilter { get; } = "*";
        public static string SettingsFileName { get; } = "!appsettings.json";

        public static TimeSpan[] VideoThumbnailPositions { get; } = {
            TimeSpan.FromMinutes(1),
            TimeSpan.FromMinutes(12),
            TimeSpan.FromMinutes(20)
        };

        public static Dictionary<GenerateButtonColors, (Color Main, Color On)> GenerateButtonColors { get; } 
            = new Dictionary<GenerateButtonColors, (Color Main, Color On)>
            {
                { Random_File_Opener_Win_Forms.GenerateButtonColors.Green, (Main: Styles.Autogenerate, On: Styles.OnAutogenerate)},
                { Random_File_Opener_Win_Forms.GenerateButtonColors.Red, (Main: Styles.DoNoAutogenerate, On: Styles.OnDoNoAutogenerate)}
            };
    }

    internal enum GenerateButtonColors
    {
        Green = 0,
        Red = 1
    }
}