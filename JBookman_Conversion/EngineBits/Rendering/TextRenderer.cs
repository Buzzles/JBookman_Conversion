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
                // Bind texture into gl
                int textureId = GL.GenTexture();
                GL.BindTexture(TextureTarget.Texture2D, textureId);

                // Rough conver
                var character = char.ConvertFromUtf32(c);
                var alt = Convert.ToChar(c);
                var charImage = getCharacterImage(alt);
                
                ////var bmp = new Bitmap(80, 80, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                var bmp = charImage;

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

        //https://stackoverflow.com/questions/2070365/how-to-generate-an-image-from-text-on-fly-at-runtime
        private Bitmap getCharacterImage(char character)
        {
            // hacks
            var minSize = new Size(8, 8);
            var font = SystemFonts.DefaultFont;
            var backColor = Color.Transparent;
            var textColor = Color.White;
            var text = character.ToString();

            //first, create a dummy bitmap just to get a graphics object
            SizeF textSize;
            using (Image img = new Bitmap(1, 1))
            {
                using (Graphics drawing = Graphics.FromImage(img))
                {
                    //measure the string to see how big the image needs to be
                    textSize = drawing.MeasureString(text, font);
                    if (!minSize.IsEmpty)
                    {
                        textSize.Width = textSize.Width > minSize.Width ? textSize.Width : minSize.Width;
                        textSize.Height = textSize.Height > minSize.Height ? textSize.Height : minSize.Height;
                    }
                }
            }

            //create a new image of the right size
            var retImg = new Bitmap((int)textSize.Width, (int)textSize.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            using (var drawing = Graphics.FromImage(retImg))
            {
                //paint the background
                drawing.Clear(backColor);

                //create a brush for the text
                using (Brush textBrush = new SolidBrush(textColor))
                {
                    drawing.DrawString(text, font, textBrush, 0, 0);
                    drawing.Save();
                }
            }
            return retImg;
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