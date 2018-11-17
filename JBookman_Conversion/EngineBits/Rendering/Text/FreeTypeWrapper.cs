using System.Runtime.InteropServices;

namespace JBookman_Conversion.EngineBits.Rendering.Text
{
    class FreeTypeWrapper
    {
        [DllImport("freetype.dll")]
        public static extern int FT_Init_FreeType(out System.IntPtr lib);
    }
}
