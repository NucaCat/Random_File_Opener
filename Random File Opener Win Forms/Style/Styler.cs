﻿using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Random_File_Opener_Win_Forms.CustomComponents.MessageBox;

namespace Random_File_Opener_Win_Forms.Style
{
    internal static class Styler
    {
        private static Dictionary<GenerateButtonColors, (Color Main, Color On)> GenerateButtonColorsMap { get; } 
            = new Dictionary<GenerateButtonColors, (Color Main, Color On)>
            {
                { GenerateButtonColors.Green, (Main: Styles.Autogenerate, On: Styles.OnAutogenerate)},
                { GenerateButtonColors.Red, (Main: Styles.DoNoAutogenerate, On: Styles.OnDoNoAutogenerate)}
            };
        
        // TODO v.chumachenko unbind from Styles and use some interface
        public static void ApplyStyles(Form form)
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
                textBox.BackColor = Styles.Surface;
                textBox.ForeColor = Styles.OnSurface;

                textBox.BorderStyle = BorderStyle.None;
            }
            
            foreach(var textBox in form.Controls.OfType<Label>())
            {
                textBox.BackColor = Styles.Surface;
                textBox.ForeColor = Styles.OnSurface;

                textBox.BorderStyle = BorderStyle.None;
            }
            
            foreach(var groupBox in form.Controls.OfType<GroupBox>())
            {
                groupBox.BackColor = Styles.Surface;
                groupBox.ForeColor = Styles.OnSurface;
            }
            
            foreach(var pictureBox in form.Controls.OfType<PictureBox>())
            {
                pictureBox.BackColor = Styles.Surface;
                pictureBox.ForeColor = Styles.OnSurface;

                pictureBox.BorderStyle = BorderStyle.None;
            }

            foreach (var listBox in form.Controls.OfType<ListBox>())
            {
                listBox.BackColor = Styles.Surface;
                listBox.ForeColor = Styles.OnSurface;
                listBox.BorderStyle = BorderStyle.None;
            }

            foreach (var flatNumericUpDown in form.Controls.OfType<FlatNumericUpDown>())
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

        public static void ApplyStylesToMessageBox(CustomMessageBox messageBox)
        {
            messageBox.YesButtonProvider.BackColor = Styles.Secondary;
            messageBox.YesButtonProvider.ForeColor = Styles.OnSecondary;

            messageBox.YesButtonProvider.FlatAppearance.MouseDownBackColor = Styles.Secondary;
            messageBox.YesButtonProvider.FlatAppearance.MouseOverBackColor = Styles.Secondary;
            

            messageBox.NoButtonProvider.BackColor = Styles.Error;
            messageBox.NoButtonProvider.ForeColor = Styles.OnError;

            messageBox.NoButtonProvider.FlatAppearance.MouseDownBackColor = Styles.Error;
            messageBox.NoButtonProvider.FlatAppearance.MouseOverBackColor = Styles.Error;
        }
        
        public static void ChangeAutogenerateButtonColor(Button button, GenerateButtonColors color)
        {
            button.BackColor = GenerateButtonColorsMap[color].Main;
            button.ForeColor = GenerateButtonColorsMap[color].On;
            
            button.FlatAppearance.MouseDownBackColor = GenerateButtonColorsMap[color].Main;
            button.FlatAppearance.MouseOverBackColor = GenerateButtonColorsMap[color].Main;
        }
    }
}