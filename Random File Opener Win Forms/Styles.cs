using System.Drawing;

namespace Random_File_Opener_Win_Forms
{
    internal static class Styles
    {
        public static Color Primary { get; } = ColorTranslator.FromHtml("#BB86FC");
        public static Color Secondary { get; } = ColorTranslator.FromHtml("#03DAC6");
        public static Color PrimaryVariant { get; } = ColorTranslator.FromHtml("#3700B3");

        public static Color Background { get; } = ColorTranslator.FromHtml("#121212");
        public static Color Surface { get; } = ColorTranslator.FromHtml("#1F1B24");
        public static Color Error { get; } = ColorTranslator.FromHtml("#CF6679");

        public static Color OnPrimary { get; } = ColorTranslator.FromHtml("#000000");
        public static Color OnSecondary { get; } = ColorTranslator.FromHtml("#000000");
        public static Color OnBackground { get; } = ColorTranslator.FromHtml("#FFFFFF");
        public static Color OnSurface { get; } = ColorTranslator.FromHtml("#FFFFFF");
        public static Color OnError { get; } = ColorTranslator.FromHtml("#000000");
        
        
        public static Color Autogenerate { get; } = ColorTranslator.FromHtml("#03DAC6");
        public static Color DoNoAutogenerate { get; } = ColorTranslator.FromHtml("#CF6679");
    }
}