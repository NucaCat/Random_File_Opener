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

            foreach(var button in GetControlsOfType<Button>(form))
            {
                button.BackColor = Styles.Primary;
                button.ForeColor = Styles.OnPrimary;

                button.FlatStyle = FlatStyle.Flat;
                button.FlatAppearance.BorderSize = 0;
                button.FlatAppearance.MouseDownBackColor = Styles.Primary;
                button.FlatAppearance.MouseOverBackColor = Styles.Primary;
            }
            
            foreach(var textBox in GetControlsOfType<TextBox>(form))
            {
                textBox.BackColor = Styles.Surface;
                textBox.ForeColor = Styles.OnSurface;

                textBox.BorderStyle = BorderStyle.None;
            }
            
            foreach(var textBox in GetControlsOfType<Label>(form))
            {
                textBox.BackColor = Styles.Surface;
                textBox.ForeColor = Styles.OnSurface;

                textBox.BorderStyle = BorderStyle.None;
            }
            
            foreach(var groupBox in GetControlsOfType<GroupBox>(form))
            {
                groupBox.BackColor = Styles.Surface;
                groupBox.ForeColor = Styles.OnSurface;
            }
            
            foreach(var pictureBox in GetControlsOfType<PictureBox>(form))
            {
                pictureBox.BackColor = Styles.Surface;
                pictureBox.ForeColor = Styles.OnSurface;

                pictureBox.BorderStyle = BorderStyle.None;
            }

            foreach (var listBox in GetControlsOfType<ListBox>(form))
            {
                listBox.BackColor = Styles.Surface;
                listBox.ForeColor = Styles.OnSurface;
                listBox.BorderStyle = BorderStyle.None;
            }

            foreach (var progressBar in GetControlsOfType<ProgressBar>(form))
            {
                progressBar.Style = ProgressBarStyle.Continuous;
            }

            foreach (var flatNumericUpDown in GetControlsOfType<FlatNumericUpDown>(form))
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

        private static IEnumerable<T> GetControlsOfType<T>(Form form)
        {
            var panelControls = form.Controls.OfType<Panel>()
                .SelectMany(u => u.Controls.OfType<T>());
            return form.Controls.OfType<T>()
                .Concat(panelControls);
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