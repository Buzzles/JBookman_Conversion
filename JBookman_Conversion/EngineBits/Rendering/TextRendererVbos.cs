using JBookman_Conversion.EngineBits.Rendering;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;

namespace JBookman_Conversion.EngineBits
{
    internal class TextRendererVbos : TextRendererBase
    {
        public TextRendererVbos() : base()
        {
        }

        internal override void RenderText(TextPrimitive textPrimitive)
        {
            RenderPrimitivesForText_WithVbos(textPrimitive);
        }

        private void RenderPrimitivesForText_WithVbos(TextPrimitive textPrimitive)
        {
            // VBO (Vertex Buffer Object) = array/s of vertex data stored on gpu
            // VAO (Vertex Array Object) = holds what is essentially a pointer to VBOs, VAO are really just thin state wrappers

            // Create VBO
            // 1: Generate new buffer object (uninitialised, so just get id)
            var buf = GL.GenBuffer(); // short for gl.GenBuffers(1, out int[])
            
            // 2: Bind buffer object -- hook buffer to a type. ArrayBuffer is for vertex attributes, eg: vertex coords, texture coords, normals, colour component arrays. (If using Index arrays, use ElementArrayBuffer)
            // This actually initialises the buffer with initial states and a 0 size mem buffer
            GL.BindBuffer(BufferTarget.ArrayBuffer, buf);

            // 3: copy data info buffer object
            float[] vertices = {
                -0.5f, -0.5f, 0.0f, //Bottom-left vertex
                0.5f, -0.5f, 0.0f, //Bottom-right vertex
                0.0f,  0.5f, 0.0f  //Top vertex
            };

            var size = vertices.Length * sizeof(float); // size of the data we're storing!

            GL.BufferData(BufferTarget.ArrayBuffer, size, vertices, BufferUsageHint.StaticDraw); // upload data to VBO

            // Generate buffer for indices
            var indexBuffer = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBuffer); // bind new bufferObject to the element buffer
            // TODO: load data with GL.BufferData!

            // Draw it all
            DrawVbo_songHo(buf, indexBuffer);

            // Clean up when we've finished!
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.DeleteBuffer(buf);
        }

        private void DrawVbo_songHo(int vboId, int indexBufferId)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, vboId);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBufferId);

            GL.EnableClientState(ArrayCap.VertexArray); // activate vertex position array
            GL.EnableClientState(ArrayCap.NormalArray); // activate vertex normal array
            GL.EnableClientState(ArrayCap.TextureCoordArray); // activate texture coord array

            var stride = 0;
            GL.VertexPointer(3, VertexPointerType.Float, stride, offset);
            GL.NormalPointer(NormalPointerType.Float, stride, offset2);
            GL.TexCoordPointer(2, TexCoordPointerType.Float, stride, offset3);

            var faceCount = 36;
            GL.DrawElements(BeginMode.Triangles, faceCount, DrawElementsType.UnsignedByte, 0);

            GL.DisableClientState(ArrayCap.VertexArray);
            GL.DisableClientState(ArrayCap.NormalArray);
            GL.DisableClientState(ArrayCap.TextureCoordArray);

            // Clean up when we've finished!
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        }

        void Setup_learnOpenGl()
        {
            int VAO, VBO;
            VAO =  GL.GenVertexArray();
            VBO = GL.GenBuffer();
            
            GL.BindVertexArray(VAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * 6 * 4, (System.IntPtr)null, BufferUsageHint.DynamicDraw);
            
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 4, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);
            
            // tidy up
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
        }

        void RenderText_learnOpenGl(Shader s, TextPrimitive textPrimitive, float scale, Vector3 color, int vbaId, int vboId)
        {
            float x = textPrimitive.X;
            float y = textPrimitive.Y;

            // activate corresponding render state	
            s.Use();
            GL.Uniform3(GL.GetUniformLocation(s.Handle, "textColor"), color.X, color.Y, color.Z);
            GL.ActiveTexture(TextureUnit.Texture0);
            
            GL.BindVertexArray(vbaId);

            // iterate through all characters
            int charsMax = textPrimitive.Text.Length;
            for (var c = 0; c < charsMax; c++)
            {
                var alt = Convert.ToChar(textPrimitive.Text[c]);

                Character ch = Characters[alt];

                float xpos = x + ch.Bearing.X * scale;
                float ypos = y - (ch.Size.Y - ch.Bearing.Y) * scale;

                float w = ch.Size.X * scale;
                float h = ch.Size.Y * scale;
                // update VBO for each character
                float[,] vertices = new float[6,4] {
                    { xpos,     ypos + h,   0.0f, 0.0f },            
                    { xpos,     ypos,       0.0f, 1.0f },
                    { xpos + w, ypos,       1.0f, 1.0f },

                    { xpos,     ypos + h,   0.0f, 0.0f },
                    { xpos + w, ypos,       1.0f, 1.0f },
                    { xpos + w, ypos + h,   1.0f, 0.0f }
                };
                // render glyph texture over quad
                GL.BindTexture(TextureTarget.Texture2D, ch.TextureId);
                // update content of VBO memory
                GL.BindBuffer(BufferTarget.ArrayBuffer, vboId);
                // Usage: public static void BufferSubData<T3>(BufferTarget target, IntPtr offset, int size, T3[,] data) where T3 : struct;
                GL.BufferSubData(BufferTarget.ArrayBuffer, (IntPtr)0,  sizeof(float) * vertices.Length , vertices);
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
                
                // render quad
                GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
                // now advance cursors for next glyph (note that advance is number of 1/64 pixels)
                x += (ch.Advance >> 6) * scale; // bitshift by 6 to get value in pixels (2^6 = 64)
            }

            // Tidyup
            GL.BindVertexArray(0);
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }
    }
}