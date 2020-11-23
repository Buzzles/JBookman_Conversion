using JBookman_Conversion.EngineBits.Rendering;
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

                // Unbind texture!
                GL.BindTexture(TextureTarget.Texture2D, 0);
                
                // build up character objs for ref
                var newCharacter = new Character
                {
                    Char = alt,
                    TextureId = textureId
                    // todo: size, bearing and advance
                };

                charMap.Add(c, newCharacter);
            }

            return charMap;
        }

        internal void RenderText(TextPrimitive textPrimitive)
        {
            RenderPrimitivesForText(textPrimitive);
        }

        // https://stackoverflow.com/questions/2070365/how-to-generate-an-image-from-text-on-fly-at-runtime
        private Bitmap getCharacterImage(char character)
        {
            // hacks
            var minSize = new Size(8, 8);
            var font = SystemFonts.DefaultFont;
            var backColor = Color.Transparent;
            var textColor = Color.White;
            var text = character.ToString();

            var testfont = new Font(FontFamily.GenericMonospace, 16f);

            font = testfont;

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

        // TEMP! Need to completely redo properly
        private void RenderPrimitivesForText(TextPrimitive textPrimitive)
        {
            var charId = char.ConvertToUtf32(textPrimitive.Character.ToString(), 0);
            var texId = Characters[charId].TextureId;

            // TEMP, do texture binding at a higher level
            GL.BindTexture(TextureTarget.Texture2D, texId); //set texture

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.LoadIdentity();

            //Proper GL way, translate grid, then draw at new 0.0
            GL.PushMatrix();

            // Matrices applied from right = last transform operation applied first!
            var translateVector = new Vector3(textPrimitive.X, -textPrimitive.Y, textPrimitive.Z);
            GL.Translate(translateVector);

            GL.Begin(PrimitiveType.Quads);

            //bottomleft
            GL.TexCoord2(0, 0);
            GL.Vertex3(0, -1.0f, 1.0f);  //vertex3(x,y,z)
            //top left
            GL.TexCoord2(0, 1);
            GL.Vertex3(0, 0.0f, 1.0f);
            //top right
            GL.TexCoord2(1, 1);
            GL.Vertex3(1.0f, 0.0f, 1.0f);
            //bottom right
            GL.TexCoord2(1, 0);
            GL.Vertex3(1.0f, -1.0f, 1.0f);

            GL.End();

            GL.PopMatrix();

            GL.Disable(EnableCap.Blend);

            GL.BindTexture(TextureTarget.Texture2D, 0); //unbind texture
        }

        private struct Character
        {
            public char Char { get; set; }
            public int TextureId { get; set; }
            public Vector2 Size { get; set; }
            public Vector2 Bearing { get; set; }
            public int Advance { get; set; }
        }
    }
}