namespace JBookman_Conversion.GameStates.MenuComponents
{
    internal class MenuItem
    {
        public string Text { get; set; }

        // Manager items
        public int Order { get; set; }
        public bool IsSelected { get; set; } // move to a manager, not responsibility of class
    }
}