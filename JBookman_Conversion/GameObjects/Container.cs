using System;

namespace JBookman_Conversion
{
    [Serializable]
    public class Container
    {
        public int Gold { get; set; }
        public int Keys { get; set; }
        public int Potions { get; set; }
        public int Armour { get; set; }
        public int Weapon { get; set; }
        public int Sector { get; set; }
        public int Tile { get; set; }

        public bool Locked { get; set; }

        public Container()
        {
            Gold = 0;
            Keys = 0;
            Potions = 0;
            Armour = 0;
            Weapon = 0;
            Locked = false;
            Sector = 0;
            Tile = 0;
        }

        ~Container()
        {
        }
    }
}
