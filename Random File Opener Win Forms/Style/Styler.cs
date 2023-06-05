using System.Linq;
using System.Windows.Forms;

namespace Random_File_Opener_Win_Forms.Style
{
    internal static class Styler
    {
        // TODO v.chumachenko unbind from Styles and use some interface
        public static void ApplyStyles(Form1 form)
        {
            form.BackColor = Styles.Background;

            foreach(var button in form.Controls.OfType<Button>())
            {
                button.BackColor = Styles.Primary;
                button.ForeColor = Styles.OnPrimary;

                button.FlatStyle = FlatStyle.Flat;
                button.FlatAppearance.BorderSize = 0;
                button.FlatAppearance.MouseDownBackColor = Styles.Primary;
                button.FlatAppearance.MouseOverBackColor = Styles.Primary;
            }
            
            foreach(var textBox in form.Controls.OfType<TextBox>())
            {
                textBox.BackColor = Styles.LighterSurface;
                textBox.ForeColor = Styles.OnSurface;

                textBox.BorderStyle = BorderStyle.None;
            }
            
            foreach(var pictureBox in form.Controls.OfType<PictureBox>())
            {
                pictureBox.BackColor = Styles.LighterSurface;
                pictureBox.ForeColor = Styles.OnSurface;

                pictureBox.BorderStyle = BorderStyle.None;
            }

            foreach (var listBox in form.Controls.OfType<ListBox>())
            {
                listBox.BackColor = Styles.LighterSurface;
                listBox.ForeColor = Styles.OnSurface;
                listBox.BorderStyle = BorderStyle.None;
            }

            foreach (var flatNumericUpDown in form.Controls.OfType<FlatNumericUpDown>())
            {
                flatNumericUpDown.BackColor = Styles.LighterSurface;
                flatNumericUpDown.ForeColor = Styles.OnSurface;
                flatNumericUpDown.BorderStyle = BorderStyle.FixedSingle;
            
                flatNumericUpDown.ButtonHighlightColor = Styles.Primary;
                flatNumericUpDown.BorderColor = Styles.LighterSurface;
                flatNumericUpDown.Controls[0].BackColor = Styles.Primary;
                flatNumericUpDown.Controls[0].ForeColor = Styles.Primary;
            }
        }
    }
}