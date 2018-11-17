using JBookman_Conversion.EngineBits.Rendering;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Linq;

namespace JBookman_Conversion.EngineBits
{
    public class Renderer
    {
        public Matrix4 MoveMatrix { get; set; } = new Matrix4();
        ////private const float moveAmount = 0.1f;
        public int MainTileSetTextureId { get; set; }
        public int PlayerTileSetTextureId { get; set; }

        private PlayerRenderer _playerRenderer;
        private TextRenderer _textRenderer;

        public Renderer()
        {
            _playerRenderer = new PlayerRenderer();

            _textRenderer = new TextRenderer();
        }

        public void Initialise()
        {
            GL.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);
            GL.Enable(EnableCap.DepthTest);
            MoveMatrix = Matrix4.Identity;
            MoveMatrix = Matrix4.CreateTranslation(0.0f, 0.0f, 0.0f);
        }

        internal void RenderFrame()
        {
            //throw new NotImplementedException();
        }

        internal void OnResize(Game gameContext)
        {
            var clientRectangle = gameContext.ClientRectangle;
            GL.Viewport(clientRectangle.X, clientRectangle.Y, clientRectangle.Width, clientRectangle.Height);

            //  Matrix4 projection = Matrix4.CreateOrthographicOffCenter(0, 1024 / 56, -768 / 56, 0, -50.0f, 50.0f);

            //  working, calculing 25 cols and 18.75 rows.
            //  Matrix4 projection = Matrix4.CreateOrthographicOffCenter(0, 800 / 32, -600 / 32, 0, -50.0f, 50.0f);
            Matrix4 projection = Matrix4.CreateOrthographicOffCenter(0, 800 / 32, -19, 0, -50.0f, 50.0f);
            //  Matrix4 projection = Matrix4.CreateOrthographicOffCenter(0, 24, -18, 0, -50.0f, 50.0f);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.LoadMatrix(ref projection);
        }

        internal void RenderScene(object scene)
        { }

        internal void BeginRender()
        {
            // Setup Rendering
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            Matrix4 modelview = Matrix4.LookAt(0.0f, 0.0f, 10.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f);

            modelview = MoveMatrix * modelview;

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            GL.LoadMatrix(ref modelview);

            DrawAxis();

            GL.Enable(EnableCap.Texture2D);
        }

        internal void EndRender()
        {
            GL.Disable(EnableCap.Texture2D);
        }

        internal void RenderPrimitives(Primitive[] primitives)
        {
            var primitivesGroupedByTextureId = primitives.GroupBy(p => p.TextureId);

            foreach (var grouping in primitivesGroupedByTextureId)
            {
                // Bind texture for these primitives
                var groupTextureKey = grouping.Key;
                GL.BindTexture(TextureTarget.Texture2D, groupTextureKey); //set texture

                foreach (var primitive in grouping)
                {
                    DrawPrimitive(primitive);
                }
            }
        }

        // TODO: Combine into normal render code above
        internal void RenderPlayerPrimitive(Primitive playerPrimitive)
        {
            _playerRenderer.RenderPlayerPrimitive(playerPrimitive);
        }

        private static void DrawPrimitive(Primitive primitive)
        {
            //Proper GL way, translate grid, then draw at new 0.0
            GL.PushMatrix();

            // Matrices applied from right = last transform operation applied first!
            var translateVector = new Vector3(primitive.X, -primitive.Y, primitive.Z);
            GL.Translate(translateVector);

            if (primitive.Rotation != 0)
            {
                // Make origin in middle of texture
                GL.Translate(0.5, -0.5, 0.0);

                var rotateVex0 = new Vector3(0, 0, 1);
                GL.Rotate(primitive.Rotation, rotateVex0);

                //move origin back
                GL.Translate(-0.5, 0.5, 0.0);
            }

            if (primitive.TileId.HasValue)
            {
                DrawTile(primitive.TileId.Value);
            }

            GL.PopMatrix();
        }

        private static void DrawAxis()
        {
            /*  Axis Draw Code
             *  Simply draw 3 lines representing the 3D axis
             *  Blue = x axis (-left/+right)
             *  Red = y axis (+up/-down)
             *  Green = Z (depth)
             */

            //  test code
            GL.Begin(PrimitiveType.Lines);

            //  x = blue
            GL.Color3(0.0f, 0.0f, 1.0f);
            GL.Vertex3(-100.0f, 0.0f, 0.0f);
            GL.Vertex3(100.0f, 0.0f, 0.0f);
            //  y = red
            GL.Color3(1.0f, 0.0f, 0.0f);
            GL.Vertex3(0.0f, -110.0f, 0.0f);
            GL.Vertex3(0.0f, 100.0f, 0.0f);
            //  z = green
            GL.Color3(new Vector3(0.0f, 1.0f, 0.0f));
            GL.Vertex3(0.0f, 0.0f, -100.0f);
            GL.Vertex3(0.0f, 0.0f, 100.0f);
            GL.Color4(1.0f, 1.0f, 1.0f, 1.0f);

            GL.End();
        }

        private static void DrawTile(int tilesetTileNumber)
        {
            //calulate tilenumber's row and column value on tileset
            // int numberofcolumns = 2;
            int row;
            int column;
            column = tilesetTileNumber % Constants.TILESETCOLUMNCOUNT;
            float texture_size = 1.0f / Constants.TILESETCOLUMNCOUNT;
            //0.5 = size
            row = (int)((tilesetTileNumber * texture_size) + 0.00001f);
            // MessageBox.Show("tile number: " + tile +" row: "+row+" col: "+column +" texturesize:"+texture_size);
            float s1 = texture_size * (column + 0);
            float s2 = texture_size * (column + 1);
            float t1 = 1 - (texture_size * (row + 0));
            float t2 = 1 - (texture_size * (row + 1));

            GL.Begin(PrimitiveType.Quads);

            //quad1
            //bottomleft
            GL.TexCoord2(s1, t2);
            GL.Vertex3(0, -1.0f, 0.0f);  //vertex3(x,y,z)
                                         //top left
            GL.TexCoord2(s1, t1);
            GL.Vertex3(0, 0.0f, 0.0f);
            //top right
            GL.TexCoord2(s2, t1);
            GL.Vertex3(1.0f, 0.0f, 0.0f);
            //bottom right
            GL.TexCoord2(s2, t2);
            GL.Vertex3(1.0f, -1.0f, 0.0f);

            GL.End();
        }
    }
}