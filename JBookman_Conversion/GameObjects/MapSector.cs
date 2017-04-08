using System;

namespace JBookman_Conversion
{
    [Serializable]
    public class MapSector
    {
        public bool Impassable { get; set; }
        public ushort TileNumberId { get; set; }

        public ushort rotationAngle {get;set;}

        public MapSector()
        {
        }

        public MapSector(ushort tileID)
        {
            TileNumberId = tileID;
            Impassable = false;
        }

        public MapSector(ushort tileID, bool impassable,UInt16 rot = 0)
        {
            TileNumberId = tileID;
            Impassable = impassable;
            rotationAngle = rot;
        }
     }  
}