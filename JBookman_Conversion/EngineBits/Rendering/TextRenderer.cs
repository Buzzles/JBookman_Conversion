using JBookman_Conversion.EngineBits.Rendering;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace JBookman_Conversion.EngineBits
{
    internal class TextRenderer : TextRendererBase
    {
        public TextRenderer()
            : base()
        {
        }

        internal override void RenderText(TextPrimitive textPrimitive)
        {
            RenderPrimitivesForText(textPrimitive);
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
    }
}