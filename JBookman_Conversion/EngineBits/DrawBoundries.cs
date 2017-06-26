namespace JBookman_Conversion.EngineBits
{
    internal class DrawBoundries
    {
        public int MinVisibleCol { get; private set; }
        public int MaxVisibleCol { get; private set; }
        public int MinVisibleRow { get; private set; }
        public int MaxVisibleRow { get; private set; }

        public DrawBoundries(int _minVisibleCol, int _maxVisibleCol, int _minVisibleRow, int _maxVisibleRow)
        {
            MinVisibleCol = _minVisibleCol;
            MaxVisibleCol = _maxVisibleCol;
            MinVisibleRow = _minVisibleRow;
            MaxVisibleRow = _maxVisibleRow;
        }
    }
}