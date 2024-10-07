using System;

namespace Random_File_Opener_Win_Forms.Settings
{
    internal sealed class AppSettings
    {
        public string Filter { get; set; }
        public TimeSpan[] VideoThumbnailPositions { get; set; }
        public bool CacheImages { get; set; }
        public bool ExportOnlyVisible { get; set; }
        public bool ShowPreview { get; set; }
        public StylesSettings Styles { get; set; }
    }

    internal sealed class StylesSettings
    {
        public string Primary { get; set; }
        public string Secondary { get; set; }
        public string PrimaryVariant { get; set; }

        public string Background { get; set; }
        public string Surface { get; set; }

        public string Error { get; set; }
        
        public string Autogenerate { get; set; }
        public string DoNoAutogenerate { get; set; }

        public string OnPrimary { get; set; }
        public string OnSecondary { get; set; }
        public string OnBackground { get; set; }
        public string OnSurface { get; set; }
        public string OnError { get; set; }

        public string OnAutogenerate { get; set; }
        public string OnDoNoAutogenerate { get; set; }
    }
}