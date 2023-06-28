using System;
using System.Windows.Forms;

namespace Random_File_Opener_Win_Forms.CustomComponents.MessageBox
{
    public partial class CustomMessageBox : Form
    {
        public DialogResult Result { get; set; }

        public Button YesButtonProvider => YesButton;
        public Button NoButtonProvider => NoButton;

        public CustomMessageBox()
        {
            InitializeComponent();
        }

        public DialogResult ShowMessageBox(string text)
        {
            TextLabel.Text = text;

            ShowDialog();

            return Result;
        }

        private void YesButton_Click(object sender, EventArgs e)
        {
            Result = DialogResult.Yes;
            Close();
        }

        private void NoButton_Click(object sender, EventArgs e)
        {
            Result = DialogResult.No;
            Close();
        }
    }
}