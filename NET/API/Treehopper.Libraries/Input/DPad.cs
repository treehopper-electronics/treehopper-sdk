namespace Treehopper.Libraries.Input
{
    public class DPadStateEventArgs
    {
        public DPadState NewValue { get; set; }
    }

    public delegate void DPadStateEventHandler(object sender, DPadStateEventArgs e);

    public enum DPadState
    {
        None,
        Left,
        Right,
        Up,
        Down
    }
}