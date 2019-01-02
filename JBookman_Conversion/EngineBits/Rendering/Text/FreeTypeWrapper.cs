using System;
using System.Runtime.InteropServices;

namespace JBookman_Conversion.EngineBits.Rendering.Text
{    
    class FreeTypeWrapper
    {
        const string freetypePath = "libs/freetype.dll";

        [DllImport(freetypePath)]
        public static extern int FT_Init_FreeType(out IntPtr lib);

        [DllImport(freetypePath)]
        public static extern int FT_New_Face(IntPtr lib, string fontPath, int faceIndex, out IntPtr face);
    }
}
