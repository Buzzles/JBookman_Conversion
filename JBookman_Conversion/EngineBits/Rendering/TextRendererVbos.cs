using JBookman_Conversion.EngineBits.Rendering;
using OpenTK;
using OpenTK.Graphics.OpenGL;

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

            GL.BufferData(BufferTarget.ArrayBuffer, size, vertices, BufferUsageHint.StaticDraw);

            // Clean up when we've finished!
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.DeleteBuffer(buf);
        }
    }
}