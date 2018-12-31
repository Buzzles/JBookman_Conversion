using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media;

namespace JBookman_Conversion.EngineBits
{
    internal class TextRenderer
    {
        private Dictionary<int, Character> Characters;

        public TextRenderer()
        {
            Characters = BuildCharacterMap();
        }

        internal void RenderText()
        {
        }

        private Dictionary<int, Character> BuildCharacterMap()
        {
            var characterMap = new Dictionary<int, Character>();

            GlyphTypeface ttf = new GlyphTypeface(new Uri(@"c:\windows\fonts\verdana.ttf"));

            // https://learnopengl.com/In-Practice/Text-Rendering

            // Build up ASCII size set (128 chars!)
            for (int c = 0; c < 128; c++)
            {
                if (!ttf.CharacterToGlyphMap.TryGetValue(c, out var glyphIndex))
                {
                    Console.WriteLine("Failed to get character from map");
                    throw new Exception("Failed to get char from glyph map");
                }

                var renderingEmSize = 13.333333333333334d;

                var outline = ttf.GetGlyphOutline(glyphIndex, renderingEmSize, 1d);
                var geomDrawing = new GeometryDrawing(System.Windows.Media.Brushes.Black, null, outline);

                var drawImg = new DrawingImage(geomDrawing);

                // render to image?

                int textureID = GL.GenTexture();
                //bind a named texture to a texturing target
                GL.BindTexture(TextureTarget.Texture2D, textureID);

                // Can combine with Core.LoadTextureFromFile? 
                //// This might not be needed at all.
                var fileName = "HahaNo"; // 
                var bmp = new Bitmap(fileName); // swap to take an Image?

                //  flip y axis as image will have top left 0.0 but GL has bottom left 0.0 origin.
                bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
                //  lock part of bitmap in memory, in this case, the entire bitmap
                BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmpData.Width,
                    bmpData.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bmpData.Scan0);

                bmp.UnlockBits(bmpData);

                bmp.Dispose();

                // Setup texture options
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureParameterName.ClampToEdge);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureParameterName.ClampToEdge);
                ////GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapR, (int)TextureParameterName.ClampToEdge); -- not needed

                // Build up char map from glyph face.
                var character = new Character
                {
                    TextureId = textureID,
                    ////glm::ivec2(face->glyph->bitmap.width, face->glyph->bitmap.rows),
                    ////glm::ivec2(face->glyph->bitmap_left, face->glyph->bitmap_top),
                    ////face->glyph->advance.x
                };

                Characters.Add(c, character);
            }
            return characterMap;
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