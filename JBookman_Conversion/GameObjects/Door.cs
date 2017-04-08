using System;

namespace JBookman_Conversion
{
    [Serializable]
    public class Door
    {
        public bool IsSecret { get; set; }
        public bool IsLocked { get; set; }

        public int Sector { get; set; } //Private?
        public int Tile { get; set; }

        public Door()
        {
            IsSecret = false;
            IsLocked = false;
            Sector = 0;
            Tile = 0;
        }
        ~Door()
        {
        }
    }
}
