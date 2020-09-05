using JBookman_Conversion.EngineBits.Abstract;
using JBookman_Conversion.EngineBits.Consts;
using System;
using JBookman_Conversion.EngineBits;
using OpenTK.Input;
using JBookman_Conversion.GameStates.MenuComponents;
using JBookman_Conversion.EngineBits.Rendering;

namespace JBookman_Conversion.GameStates
{
    public class MenuState : IGameState, IDrawable, IUpdatable
    {
        public ProcessState ProcessState => ProcessState.Menu;

        private MenuRenderer _menuRenderer;
        private MenuInputHandler _inputHandler;
        private MenuDrawer _menuDrawer;

        public MenuState()
        {
            _menuRenderer = new MenuRenderer();

            _inputHandler = new MenuInputHandler(null);

            _menuDrawer = new MenuDrawer();
        }

        public void Draw(Renderer renderer)
        {
            // Old, to be removed once new drawer is working.
            var textureId = renderer.MainTileSetTextureId;
            ///_menuRenderer.DrawMenu(textureId);
            
            // New!
            // TODO: Pass in the menu container object to the drawer.
            var menuPrimitives = _menuDrawer.GetPrimitivesToRender();

            renderer.BeginRender();

            renderer.RenderPrimitives(menuPrimitives);

            renderer.EndRender();
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

        public void Update(KeyboardState keyboardState)
        {
            _inputHandler.HandleKeyboardDown(keyboardState);
        }
    }
}