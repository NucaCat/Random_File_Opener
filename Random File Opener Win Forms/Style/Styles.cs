using System.Drawing;
using Random_File_Opener_Win_Forms.Settings;

namespace Random_File_Opener_Win_Forms.Style
{
    internal static class Styles
    {
        public static Color Primary { get; set; } = ColorTranslator.FromHtml("#BB86FC");
        public static Color Secondary { get; set; } = ColorTranslator.FromHtml("#03DAC6");
        public static Color PrimaryVariant { get; set; } = ColorTranslator.FromHtml("#3700B3");

        public static Color Background { get; set; } = ColorTranslator.FromHtml("#121212");
        public static Color Surface { get; set; } = ColorTranslator.FromHtml("#1F1B24");
        public static Color LighterSurface { get; set; } = ColorTranslator.FromHtml("#2A2531"); // 5% lighter

        public static Color Error { get; set; } = ColorTranslator.FromHtml("#CF6679");
        
        public static Color Autogenerate { get; set; } = Secondary;
        public static Color DoNoAutogenerate { get; set; } = Error;

        public static Color OnPrimary { get; set; } = ColorTranslator.FromHtml("#000000");
        public static Color OnSecondary { get; set; } = ColorTranslator.FromHtml("#000000");
        public static Color OnBackground { get; set; } = ColorTranslator.FromHtml("#FFFFFF");
        public static Color OnSurface { get; set; } = ColorTranslator.FromHtml("#FFFFFF");
        public static Color OnError { get; set; } = ColorTranslator.FromHtml("#000000");

        public static Color OnAutogenerate { get; set; } = OnSecondary;
        public static Color OnDoNoAutogenerate { get; set; } = OnError;

        public static void FillFromSettings(StylesSettings settings)
        {
            if (settings == null)
                return;

            Primary = settings.Primary.IsNullOrWhiteSpace() ? Primary : ColorTranslator.FromHtml(settings.Primary);
            Secondary = settings.Secondary.IsNullOrWhiteSpace() ? Secondary : ColorTranslator.FromHtml(settings.Secondary);
            PrimaryVariant = settings.PrimaryVariant.IsNullOrWhiteSpace() ? PrimaryVariant : ColorTranslator.FromHtml(settings.PrimaryVariant);
            Background = settings.Background.IsNullOrWhiteSpace() ? Background : ColorTranslator.FromHtml(settings.Background);
            Surface = settings.Surface.IsNullOrWhiteSpace() ? Surface : ColorTranslator.FromHtml(settings.Surface);
            LighterSurface = settings.LighterSurface.IsNullOrWhiteSpace() ? LighterSurface : ColorTranslator.FromHtml(settings.LighterSurface);
            Error = settings.Error.IsNullOrWhiteSpace() ? Error : ColorTranslator.FromHtml(settings.Error);
            Autogenerate = settings.Autogenerate.IsNullOrWhiteSpace() ? Autogenerate : ColorTranslator.FromHtml(settings.Autogenerate);
            DoNoAutogenerate = settings.DoNoAutogenerate.IsNullOrWhiteSpace() ? DoNoAutogenerate : ColorTranslator.FromHtml(settings.DoNoAutogenerate);
            OnPrimary = settings.OnPrimary.IsNullOrWhiteSpace() ? OnPrimary : ColorTranslator.FromHtml(settings.OnPrimary);
            OnSecondary = settings.OnSecondary.IsNullOrWhiteSpace() ? OnSecondary : ColorTranslator.FromHtml(settings.OnSecondary);
            OnBackground = settings.OnBackground.IsNullOrWhiteSpace() ? OnBackground : ColorTranslator.FromHtml(settings.OnBackground);
            OnSurface = settings.OnSurface.IsNullOrWhiteSpace() ? OnSurface : ColorTranslator.FromHtml(settings.OnSurface);
            OnError = settings.OnError.IsNullOrWhiteSpace() ? OnError : ColorTranslator.FromHtml(settings.OnError);
            OnAutogenerate = settings.OnAutogenerate.IsNullOrWhiteSpace() ? OnAutogenerate : ColorTranslator.FromHtml(settings.OnAutogenerate);
            OnDoNoAutogenerate = settings.OnDoNoAutogenerate.IsNullOrWhiteSpace() ? OnDoNoAutogenerate : ColorTranslator.FromHtml(settings.OnDoNoAutogenerate);
        }
    }
}