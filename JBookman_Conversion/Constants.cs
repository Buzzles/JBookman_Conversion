using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JBookman_Conversion
{
    class Constants
    {
        //public const int MAPSECTORCOUNT = 1764;
        public const int MAXPEOPLE = 100;
        public const int MAXCONTAINERS = 100;
        public const int MAXDOORS = 100;

        public const int TILESETCOLUMNCOUNT = 8;
        public const int TILESETROWCOUNT = 8;

        //view port/player view handling
        public const int NORMALVISIBLEPLAYERCOL = 12;
        public const int NORMALVISIBLEPLAYERROW = 9;
        public const int VISIBLECOLUMNCOUNT = 25;
        public const int VISIBLEROWCOUNT = 19;
        
        //Impassible objects in TownExterior | matching tileset number
        //public const int BRICKWALL_EXT = 6;
        public const int BRICKWALL_EXT = 2;
        public const int VERTFENCE_EXT = 16;
        public const int HORZFENCE_EXT = 17;
        public const int ANGLEFENCE_EXT = 18;
        //public const int TREE_EXT = 3;
        public const int TREE_EXT = 1;
        public const int FOUNTAIN_EXT = 50;
        public const int WELL_EXT = 29;
        //public const int STONEWALL_EXT = 30;
        public const int STONEWALL_EXT = 5;
        public const int WINDOW01_EXT = 8;
        public const int WINDOW02_EXT = 9;
        public const int WATER01_EXT = 32;
        public const int WATER02_EXT =49 ;

   
    //end-class
    }

    //Global Enums.

    public enum g_iDirection
    {
        NORTH,
        SOUTH,
        EAST,
        WEST
    };

    public enum g_iLocationEnum
    {
        FIRSTTOWN,
        FIRSTTOWNHOUSE01ROOM01,
        FIRSTTOWNHOUSE01ROOM02,
        FIRSTTOWNHOUSE02ROOM01,
        FIRSTTOWNHOUSE02ROOM02,
        FIRSTTOWNINNROOM01,
        FIRSTTOWNINNROOM02,
        FIRSTTOWNINNROOM03,
        FIRSTTIWNINNROOM04,
        FIRSTTOWNINNROOM05,
        FIRSTTOWNINNROOM06,
        FIRSTTOWNINNROOM07,
        FIRSTTOWNARMOURER,
        FIRSTTOWNPOTIONS,
        GANJEKWILDS

    };

    public enum g_iMapTypeEnum
    {
        TOWNEXTERIOR,
        TOWNINTERIOR,
        DUNGEON,
        WILDERNESS
    }


}
