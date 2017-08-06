using JBookman_Conversion.EngineBits.Abstract;
using JBookman_Conversion.EngineBits.Consts;
using System;
using JBookman_Conversion.EngineBits;
using OpenTK.Input;

namespace JBookman_Conversion.GameStates
{
    public class MenuState : IGameState, IDrawable, IUpdatable
    {
        public ProcessState ProcessState => ProcessState.Menu;

        private MenuRenderer _menuRenderer;

        public MenuState()
        {
            _menuRenderer = new MenuRenderer();
        }

        public void Draw(Renderer renderer)
        {
            var textureId = renderer.MainTileSetTextureId;
            _menuRenderer.DrawMenu(textureId);
            //renderer.RenderFrame();
        }

        public void Entering()
        {
            throw new NotImplementedException();
        }

        public void Leaving()
        {
            throw new NotImplementedException();
        }

        public void Obscuring()
        {
            throw new NotImplementedException();
        }

        public void Revealing()
        {
            throw new NotImplementedException();
        }

        public void Update()
        {
        }

        public UpdateResult Update(KeyboardState keyboardState)
        {
            return new UpdateResult();

            // Handle input for menu here
            // InputHandler.HandleKeyboardInput(keyboardState) etc....
        }
    }
}
