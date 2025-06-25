using System;
using System.Collections.Generic;
using System.Drawing;
using Random_File_Opener_Win_Forms.Settings;

namespace Random_File_Opener_Win_Forms.Style
{
    internal static class Styles
    {
        public static Color Primary { get; set; } = HslStringToColor("267, 95%, 76%");
        public static Color Secondary { get; set; } = HslStringToColor("174, 97%, 43%");
        public static Color PrimaryVariant { get; set; } = HslStringToColor("258, 100%, 35%");

        public static Color Background { get; set; } = HslStringToColor("0, 0%, 7%");
        // public static Color Surface { get; set; } = HslStringToColor("#1F1B24");
        public static Color Surface { get; set; } = HslStringToColor("265, 14%, 17%"); // 5% lighter

        public static Color Error { get; set; } = HslStringToColor("349, 52%, 61%");
        
        public static Color Autogenerate { get; set; } = Secondary;
        public static Color DoNotAutogenerate { get; set; } = Error;

        public static Color OnPrimary { get; set; } = HslStringToColor("0, 0%, 0%");
        public static Color OnSecondary { get; set; } = HslStringToColor("0, 0%, 0%");
        public static Color OnBackground { get; set; } = HslStringToColor("0, 0%, 100%");
        public static Color OnSurface { get; set; } = HslStringToColor("0, 0%, 100%");
        public static Color OnError { get; set; } = HslStringToColor("0, 0%, 0%");

        public static Color OnAutogenerate { get; set; } = OnSecondary;
        public static Color OnDoNotAutogenerate { get; set; } = OnError;
        
        public static Dictionary<GenerateButtonColors, (Color Main, Color On)> GenerateButtonColorsMap { get; set; } 
            = new Dictionary<GenerateButtonColors, (Color Main, Color On)>
            {
                { GenerateButtonColors.Green, (Main: Autogenerate, On: OnAutogenerate)},
                { GenerateButtonColors.Red, (Main: DoNotAutogenerate, On: OnDoNotAutogenerate)}
            };
        
        public static void FillFromSettings(StylesSettings settings)
        {
            if (settings == null)
                return;

            Primary = settings.Primary.IsNullOrWhiteSpace() ? Primary : HslStringToColor(settings.Primary);
            Secondary = settings.Secondary.IsNullOrWhiteSpace() ? Secondary : HslStringToColor(settings.Secondary);
            PrimaryVariant = settings.PrimaryVariant.IsNullOrWhiteSpace() ? PrimaryVariant : HslStringToColor(settings.PrimaryVariant);
            Background = settings.Background.IsNullOrWhiteSpace() ? Background : HslStringToColor(settings.Background);
            Surface = settings.Surface.IsNullOrWhiteSpace() ? Surface : HslStringToColor(settings.Surface);
            Error = settings.Error.IsNullOrWhiteSpace() ? Error : HslStringToColor(settings.Error);
            Autogenerate = settings.Autogenerate.IsNullOrWhiteSpace() ? Autogenerate : HslStringToColor(settings.Autogenerate);
            DoNotAutogenerate = settings.DoNotAutogenerate.IsNullOrWhiteSpace() ? DoNotAutogenerate : HslStringToColor(settings.DoNotAutogenerate);
            OnPrimary = settings.OnPrimary.IsNullOrWhiteSpace() ? OnPrimary : HslStringToColor(settings.OnPrimary);
            OnSecondary = settings.OnSecondary.IsNullOrWhiteSpace() ? OnSecondary : HslStringToColor(settings.OnSecondary);
            OnBackground = settings.OnBackground.IsNullOrWhiteSpace() ? OnBackground : HslStringToColor(settings.OnBackground);
            OnSurface = settings.OnSurface.IsNullOrWhiteSpace() ? OnSurface : HslStringToColor(settings.OnSurface);
            OnError = settings.OnError.IsNullOrWhiteSpace() ? OnError : HslStringToColor(settings.OnError);
            OnAutogenerate = settings.OnAutogenerate.IsNullOrWhiteSpace() ? OnAutogenerate : HslStringToColor(settings.OnAutogenerate);
            OnDoNotAutogenerate = settings.OnDoNotAutogenerate.IsNullOrWhiteSpace() ? OnDoNotAutogenerate : HslStringToColor(settings.OnDoNotAutogenerate);
            GenerateButtonColorsMap = new Dictionary<GenerateButtonColors, (Color Main, Color On)>
            {
                { GenerateButtonColors.Green, (Main: Autogenerate, On: OnAutogenerate)},
                { GenerateButtonColors.Red, (Main: DoNotAutogenerate, On: OnDoNotAutogenerate)}
            };
        }

        private static Color HslStringToColor(string hslString)
        {
            var a = hslString.Split(new [] { ", " }, StringSplitOptions.None);

            var h = Convert.ToInt32(a[0]);
            var s = Convert.ToInt32(a[1].Substring(0, a[1].Length - 1));
            var l = Convert.ToInt32(a[2].Substring(0, a[2].Length - 1));

            var (r, g, b) = HslToRgb(h, s, l);
            return Color.FromArgb(r, g, b);
        }

        public static (int r, int g, int b) HslToRgb(int hh, int ss, int ll)
        {
            var h = hh / 360.0d;
            var s = ss / 100.0d;
            var l = ll / 100.0d;

            double r, g, b;

            if (s == 0)
            {
                r = g = b = l;
            }
            else
            {
                var q = l < 0.5 ? l * (1 + s) : l + s - l * s;
                var p = 2 * l - q;

                r = HueToRgb(p, q, h + 1.0 / 3);
                g = HueToRgb(p, q, h);
                b = HueToRgb(p, q, h - 1.0 / 3);
            }

            var red = (int)Math.Round(r * 255);
            var green = (int)Math.Round(g * 255);
            var blue = (int)Math.Round(b * 255);

            return (red, green, blue);
        }

        private static double HueToRgb(double p, double q, double t)
        {
            if (t < 0) t += 1;
            if (t > 1) t -= 1;
        
            if (t < 1.0 / 6) return p + (q - p) * 6 * t;
            if (t < 1.0 / 2) return q;
            if (t < 2.0 / 3) return p + (q - p) * (2.0 / 3 - t) * 6;
        
            return p;
        }
    }
}