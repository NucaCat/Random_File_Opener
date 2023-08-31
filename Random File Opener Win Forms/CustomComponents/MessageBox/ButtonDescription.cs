namespace Random_File_Opener_Win_Forms.CustomComponents.MessageBox
{
    public sealed class ButtonDescription
    {
        public bool Visible { get; set; }
        public ButtonType ButtonType { get; set; }
        public string ButtonText { get; set; }
    }

    public enum ButtonType
    {
        Yes,
        No,
        Ok,
    }
}