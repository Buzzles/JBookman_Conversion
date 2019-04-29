using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace JBookman_Conversion.EngineBits
{
    internal class TextRenderer
    {
        private Dictionary<int, Character> Characters;

        public TextRenderer()
        {
            Characters = BuildCharacterMap();
        }

        private Dictionary<int, Character> BuildCharacterMap()
        {
            var charMap = new Dictionary<int, Character>();

            // Lets just load the ASCII map for now
            for (int c = 0; c < 128; c++)
            {
                // GET A BITMAP HERE SOMEHOW

                // Bind texture into gl
                int textureId = GL.GenTexture();
                GL.BindTexture(TextureTarget.Texture2D, textureId);

                //temp hacks -- should be able to get bitmap direct from freetype wrapper
                var bmp = new Bitmap(80, 80, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                bmp.RotateFlip(RotateFlipType.RotateNoneFlipY); // lol, opengl bottom left 0.0 origin

                BitmapData bmpData = bmp.LockBits(
                    new Rectangle(0, 0, bmp.Width, bmp.Height),
                    ImageLockMode.ReadOnly,
                    System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                GL.TexImage2D(
                    TextureTarget.Texture2D,
                    0,
                    PixelInternalFormat.Rgba,
                    bmpData.Width,
                    bmpData.Height,
                    0,
                    OpenTK.Graphics.OpenGL.PixelFormat.Bgra,
                    PixelType.UnsignedByte,
                    bmpData.Scan0);

                bmp.UnlockBits(bmpData);

                bmp.Dispose();

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureParameterName.ClampToEdge);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureParameterName.ClampToEdge);
                //GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapR, (int)TextureParameterName.ClampToEdge); -- not needed?

                // build up character objs for ref
                var newCharacter = new Character
                {
                    TextureId = textureId
                    // todo: size, bearing and advance
                };

                charMap.Add(c, newCharacter);
            }

            return charMap;
        }

        internal void RenderText()
        {
        }

        private struct Character
        {
            public int TextureId { get; set; }
            public Vector2 Size { get; set; }
            public Vector2 Bearing { get; set; }
            public int Advance { get; set; }
        }
    }
}