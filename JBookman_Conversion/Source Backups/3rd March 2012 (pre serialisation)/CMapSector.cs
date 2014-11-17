using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JBookman_Conversion
{
    class CMapSector
    {
        ushort tileset_number;
        bool is_Impassable;

        public CMapSector(ushort tileID)
        {
            tileset_number = tileID;
            is_Impassable = false;
            
        }
        public CMapSector(ushort tileID, bool impassable)
        {
            tileset_number = tileID;
            is_Impassable = impassable;
        }
    }
}
