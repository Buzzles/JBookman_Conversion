using System;

namespace JBookman_Conversion
{
    [Serializable]
    public class Person
    {
        public string Name { get; set; }
        public int SectorId { get; set; }
        public int TileId { get; set; }
        public bool CanMove { get; set; }

        public Person()
        {
            Name = null;
            SectorId = 0;
            TileId = 0;
            CanMove = false;
        }
        ~Person()
        {
        }
    }
}
