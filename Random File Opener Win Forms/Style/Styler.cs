using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Random_File_Opener_Win_Forms.CustomComponents.MessageBox;
using Random_File_Opener_Win_Forms.Settings;

namespace Random_File_Opener_Win_Forms.Style
{
    internal static class Styler
    {
        // TODO v.chumachenko unbind from Styles and use some interface
        public static void ApplyStyles(Form form)
        {
            form.BackColor = Styles.Background;
            
            foreach(var textBox in form.GetAllControls().OfType<Panel>())
            {
                textBox.BackColor = Styles.Surface;
                textBox.ForeColor = Styles.OnSurface;

                textBox.BorderStyle = BorderStyle.None;
            }

            foreach(var button in form.GetAllControls().OfType<Button>())
            {
                button.BackColor = Styles.Primary;
                button.ForeColor = Styles.OnPrimary;

                button.FlatStyle = FlatStyle.Flat;
                button.FlatAppearance.BorderSize = 0;
                button.FlatAppearance.MouseDownBackColor = Styles.Primary;
                button.FlatAppearance.MouseOverBackColor = Styles.Primary;
            }
            
            foreach(var textBox in form.GetAllControls().OfType<TextBox>())
            {
                textBox.BackColor = Styles.Surface;
                textBox.ForeColor = Styles.OnSurface;

                textBox.BorderStyle = BorderStyle.None;
            }
            
            foreach(var textBox in form.GetAllControls().OfType<Label>())
            {
                textBox.BackColor = Styles.Surface;
                textBox.ForeColor = Styles.OnSurface;

                textBox.BorderStyle = BorderStyle.None;
            }
            
            foreach(var groupBox in form.GetAllControls().OfType<GroupBox>())
            {
                groupBox.BackColor = Styles.Surface;
                groupBox.ForeColor = Styles.OnSurface;
            }
            
            foreach(var pictureBox in form.GetAllControls().OfType<PictureBox>())
            {
                pictureBox.BackColor = Styles.Surface;
                pictureBox.ForeColor = Styles.OnSurface;

                pictureBox.BorderStyle = BorderStyle.None;
            }

            foreach (var listBox in form.GetAllControls().OfType<ListBox>())
            {
                listBox.BackColor = Styles.Surface;
                listBox.ForeColor = Styles.OnSurface;
                listBox.BorderStyle = BorderStyle.None;
            }

            foreach (var progressBar in form.GetAllControls().OfType<ProgressBar>())
            {
                progressBar.Style = ProgressBarStyle.Continuous;
            }

            foreach (var flatNumericUpDown in form.GetAllControls().OfType<FlatNumericUpDown>())
            {
                flatNumericUpDown.BackColor = Styles.Surface;
                flatNumericUpDown.ForeColor = Styles.OnSurface;
                flatNumericUpDown.BorderStyle = BorderStyle.FixedSingle;
            
                flatNumericUpDown.ButtonHighlightColor = Styles.Primary;
                flatNumericUpDown.BorderColor = Styles.Surface;
                flatNumericUpDown.Controls[0].BackColor = Styles.Primary;
                flatNumericUpDown.Controls[0].ForeColor = Styles.Primary;
            }
        }

        public static IEnumerable<Control> GetAllControls(this Control parent)
        {
            foreach (Control control in parent.Controls)
            {
                yield return control;
                if (control.HasChildren)
                {
                    foreach (var grandChild in GetAllControls(control))
                    {
                        yield return grandChild;
                    }
                }
            }
        }

        public static void ApplyStylesToMessageBox(CustomMessageBox messageBox)
        {
            messageBox.YesButtonProvider.BackColor = Styles.Secondary;
            messageBox.YesButtonProvider.ForeColor = Styles.OnSecondary;

            messageBox.YesButtonProvider.FlatAppearance.MouseDownBackColor = Styles.Secondary;
            messageBox.YesButtonProvider.FlatAppearance.MouseOverBackColor = Styles.Secondary;


            messageBox.OkButtonProvider.BackColor = Styles.Secondary;
            messageBox.OkButtonProvider.ForeColor = Styles.OnSecondary;

            messageBox.OkButtonProvider.FlatAppearance.MouseDownBackColor = Styles.Secondary;
            messageBox.OkButtonProvider.FlatAppearance.MouseOverBackColor = Styles.Secondary;


            messageBox.NoButtonProvider.BackColor = Styles.Error;
            messageBox.NoButtonProvider.ForeColor = Styles.OnError;

            messageBox.NoButtonProvider.FlatAppearance.MouseDownBackColor = Styles.Error;
            messageBox.NoButtonProvider.FlatAppearance.MouseOverBackColor = Styles.Error;
        }
        
        public static void ChangeAutogenerateButtonColor(Button button, GenerateButtonColors color)
        {
            button.BackColor = Styles.GenerateButtonColorsMap[color].Main;
            button.ForeColor = Styles.GenerateButtonColorsMap[color].On;
            
            button.FlatAppearance.MouseDownBackColor = Styles.GenerateButtonColorsMap[color].Main;
            button.FlatAppearance.MouseOverBackColor = Styles.GenerateButtonColorsMap[color].Main;
        }

        public static void FillFromSettings(StylesSettings styles)
            => Styles.FillFromSettings(styles);
    }
}