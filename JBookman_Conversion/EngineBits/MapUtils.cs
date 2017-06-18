namespace JBookman_Conversion.EngineBits
{
    internal static class MapUtils
    {
        public static int SectorToRow(int sector, int rowCount)
        {
            int rowValue = sector / rowCount;

            return rowValue;
        }

        public static int SectorToCols(int sector, int colCount)
        {
            int columnValue = sector % colCount;

            return columnValue;
        }
    }
}