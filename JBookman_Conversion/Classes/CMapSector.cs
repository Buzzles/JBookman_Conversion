using System;
using System.Collections.Generic;
using System.Text;

namespace JBookman_Conversion
{
    [Serializable]
    public class CMapSector
    {
        ushort tileset_number;
        bool bImpassable;
        public UInt16 rotationAngle {get;set;}

        public CMapSector()
        {
            //empty constructor
        }

        public CMapSector(ushort tileID)
        {
            tileset_number = tileID;
            bImpassable = false;

        }
        public CMapSector(ushort tileID, bool impassable,UInt16 rot = 0)
        {
            tileset_number = tileID;
            bImpassable = impassable;
            rotationAngle = rot;
        }

        public Boolean Get_Is_Impassable()
        {
            return bImpassable;
        }
        public void Set_Is_Impassable(bool blocked)
        {
            bImpassable = blocked;
        }

        public ushort Get_Tileset_Number()
        {
            return tileset_number;
        }
        
        public void Set_Tileset_Number(ushort newTilesetNumber)
        {
            tileset_number = newTilesetNumber;
        }
        
        
    //end of class
    }  
}
