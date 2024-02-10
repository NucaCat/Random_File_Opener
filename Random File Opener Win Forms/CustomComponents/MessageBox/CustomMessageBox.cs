using System;
using System.Windows.Forms;
using Button = System.Windows.Forms.Button;

namespace Random_File_Opener_Win_Forms.CustomComponents.MessageBox
{
    public partial class CustomMessageBox : Form
    {
        public DialogResult Result { get; set; }

        public Button YesButtonProvider => YesButton;
        public Button NoButtonProvider => NoButton;
        public Button OkButtonProvider => OkButton;

        public static ButtonDescription[] YesNoButtons = {
            new ButtonDescription
            {
                Visible = true,
                ButtonText = "Да",
                ButtonType = ButtonType.Yes,
            },
            new ButtonDescription
            {
                Visible = true,
                ButtonText = "Нет",
                ButtonType = ButtonType.No,
            },
            new ButtonDescription
            {
                Visible = false,
                ButtonType = ButtonType.Ok,
            }
        };

        public static ButtonDescription[] OkButtons = {
            new ButtonDescription
            {
                Visible = false,
                ButtonType = ButtonType.Yes,
            },
            new ButtonDescription
            {
                Visible = false,
                ButtonType = ButtonType.No,
            },
            new ButtonDescription
            {
                Visible = true,
                ButtonText = "Ок",
                ButtonType = ButtonType.Ok,
            }
        };

        public CustomMessageBox()
        {
            InitializeComponent();
        }

        public DialogResult ShowMessageBox(string text, ButtonDescription[] buttons)
        {
            TextLabel.Text = text;

            foreach (var buttonDescription in buttons)
            {
                var button = ButtonByType(buttonDescription.ButtonType);
                button.Visible = buttonDescription.Visible;
                button.Text = buttonDescription.ButtonText ?? button.Text;

                if (button == YesButton && buttonDescription.Visible)
                    YesButton.Select();

                if (button == OkButton && buttonDescription.Visible)
                    OkButton.Select();
            }

            ShowDialog();

            return Result;
        }

        private Button ButtonByType(ButtonType buttonType)
        {
            if (buttonType == ButtonType.Yes)
                return YesButton;

            if (buttonType == ButtonType.No)
                return NoButton;

            if (buttonType == ButtonType.Ok)
                return OkButton;
            
            return null;
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

        private void OkButton_Click(object sender, EventArgs e)
        {
            Result = DialogResult.OK;
            Close();
        }
    }
}