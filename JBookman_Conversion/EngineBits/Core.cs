using System;
using System.IO;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using System.Drawing.Imaging;

namespace JBookman_Conversion.EngineBits
{
    public static class Core
    {
        public static int PlayerTileSetId;
        public static int TownExtTileSetId;
        public static int TownIntTileSetId;
        public static int DungeonTileSetId;
        public static int WildernessTileSetId;
        public static int CurrentTileSetId;

        public static void LoadTilesets()
        {
            //load tilesets into textures.
            PlayerTileSetId = LoadTextureFromFile("Tilesets\\playerTile.png");
            TownExtTileSetId = LoadTextureFromFile("Tilesets\\tileset1.png");
            TownIntTileSetId = LoadTextureFromFile("Tilesets\\playerTile.bmp");
            DungeonTileSetId = LoadTextureFromFile("Tilesets\\playerTile.bmp");
            WildernessTileSetId = LoadTextureFromFile("Tilesets\\playerTile.bmp");


#if (TILETEST)
            int testTileSetID;
            testTileSetID = LoadTexture("Tilesets\\TestTileset.png");
            //tileSetID = LoadTexture("Tilesets\\tileset1.png");
            tileSetID2 = LoadTexture("Tilesets\\tileset2.png");
            m_iCurrentTileSet = testTileSetID;
#warning This code is not production ready
#else
            CurrentTileSetId = TownExtTileSetId;
#endif
        }

        public static int LoadTextureFromFile(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                //  no texture supplied.
                throw new ArgumentException(fileName);
            }

            //  general internal texture ID
            int textureID = GL.GenTexture();
            //bind a named texture to a texturing target
            GL.BindTexture(TextureTarget.Texture2D, textureID);

            //  using System.Drawing.Bimap for bitmap file handling 
            //  (note: bitmaps are not just .bmp. png/jpeg etcc are all bitmaps)
            Bitmap bmp = null;
            try
            {

                bmp = new Bitmap(fileName);
                //  flip y axis as image will have top left 0.0 but GL has bottom left 0.0 origin.
                bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);

                //  lock part of bitmap in memory, in this case, the entire bitmap
                BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                //  create a texture from it.
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmpData.Width,
                    bmpData.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bmpData.Scan0);

                bmp.UnlockBits(bmpData);

                // We haven't uploaded mipmaps, so disable mipmapping (otherwise the texture will not appear).
                // On newer video cards, we can use GL.GenerateMipmaps() or GL.Ext.GenerateMipmaps() to create
                // mipmaps automatically. In that case, use TextureMinFilter.LinearMipmapLinear to enable them.
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

                //  Clamping methods
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureParameterName.ClampToEdge);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureParameterName.ClampToEdge);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapR, (int)TextureParameterName.ClampToEdge);

                // Clean up
                bmp.Dispose();
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine("FnF " + e);
            }
            catch (ArgumentException e1)
            {
                Console.WriteLine("ArgExcp " + e1);
            }

            // Unbind texture
            GL.BindTexture(TextureTarget.Texture2D, 0);

            //return texture ID for use.
            return textureID;
        }
    }
}
