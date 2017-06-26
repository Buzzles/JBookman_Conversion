using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace JBookman_Conversion.EngineBits
{
    public class Renderer
    {
        public Matrix4 MoveMatrix { get; set; } = new Matrix4();
        ////private const float moveAmount = 0.1f;

        private PlayerRenderer _playerRenderer;

        public Renderer()
        {
            _playerRenderer = new PlayerRenderer();
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

        // This is the render context

        // Will need to split into different bits.
        // Ie, renderer for the world.
        // renderer for the menu
        // renderer for the battlescreen
    }
}
