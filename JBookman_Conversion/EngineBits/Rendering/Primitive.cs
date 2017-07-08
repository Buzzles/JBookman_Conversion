using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JBookman_Conversion.EngineBits.Rendering
{
    public class Primitive
    {
        // Vector?
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public int Rotation { get; set; }

        public int TextureId { get; set; }

        public int? TileId { get; set; }

        // Transparency?
    }
}
