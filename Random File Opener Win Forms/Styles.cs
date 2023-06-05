using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Random_File_Opener_Win_Forms
{
    internal static class Styles
    {
        public static Color Primary { get; } = ColorTranslator.FromHtml("#BB86FC");
        public static Color Secondary { get; } = ColorTranslator.FromHtml("#03DAC6");
        public static Color PrimaryVariant { get; } = ColorTranslator.FromHtml("#3700B3");

        public static Color Background { get; } = ColorTranslator.FromHtml("#121212");
        public static Color Surface { get; } = ColorTranslator.FromHtml("#1F1B24");
        public static Color LighterSurface { get; } = ColorTranslator.FromHtml("#2A2531"); // 5% lighter

        public static Color Error { get; } = ColorTranslator.FromHtml("#CF6679");
        
        public static Color Autogenerate { get; } = Secondary;
        public static Color DoNoAutogenerate { get; } = Error;

        public static Color OnPrimary { get; } = ColorTranslator.FromHtml("#000000");
        public static Color OnSecondary { get; } = ColorTranslator.FromHtml("#000000");
        public static Color OnBackground { get; } = ColorTranslator.FromHtml("#FFFFFF");
        public static Color OnSurface { get; } = ColorTranslator.FromHtml("#FFFFFF");
        public static Color OnError { get; } = ColorTranslator.FromHtml("#000000");

        public static Color OnAutogenerate { get; } = OnSecondary;
        public static Color OnDoNoAutogenerate { get; } = OnError;

        public static void ApplyStyles(Form1 form)
        {
            form.BackColor = Background;

            foreach(var button in form.Controls.OfType<Button>())
            {
                button.BackColor = Primary;
                button.ForeColor = OnPrimary;

                button.FlatStyle = FlatStyle.Flat;
                button.FlatAppearance.BorderSize = 0;
                button.FlatAppearance.MouseDownBackColor = Primary;
                button.FlatAppearance.MouseOverBackColor = Primary;
            }
            
            foreach(var textBox in form.Controls.OfType<TextBox>())
            {
                textBox.BackColor = LighterSurface;
                textBox.ForeColor = OnSurface;

                textBox.BorderStyle = BorderStyle.None;
            }
            
            foreach(var pictureBox in form.Controls.OfType<PictureBox>())
            {
                pictureBox.BackColor = LighterSurface;
                pictureBox.ForeColor = OnSurface;

                pictureBox.BorderStyle = BorderStyle.None;
            }

            foreach (var listBox in form.Controls.OfType<ListBox>())
            {
                listBox.BackColor = LighterSurface;
                listBox.ForeColor = OnSurface;
                listBox.BorderStyle = BorderStyle.None;
            }

            foreach (var flatNumericUpDown in form.Controls.OfType<FlatNumericUpDown>())
            {
                flatNumericUpDown.BackColor = LighterSurface;
                flatNumericUpDown.ForeColor = OnSurface;
                flatNumericUpDown.BorderStyle = BorderStyle.FixedSingle;
            
                flatNumericUpDown.ButtonHighlightColor = Primary;
                flatNumericUpDown.BorderColor = LighterSurface;
                flatNumericUpDown.Controls[0].BackColor = Primary;
                flatNumericUpDown.Controls[0].ForeColor = Primary;
            }
        }
    }
}