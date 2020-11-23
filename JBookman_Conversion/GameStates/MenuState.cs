using JBookman_Conversion.EngineBits.Abstract;
using JBookman_Conversion.EngineBits.Consts;
using System;
using JBookman_Conversion.EngineBits;
using OpenTK.Input;
using JBookman_Conversion.GameStates.MenuComponents;
using System.Collections.Generic;

namespace JBookman_Conversion.GameStates
{
    public class MenuState : IGameState, IDrawable, IUpdatable
    {
        public ProcessState ProcessState => ProcessState.Menu;

        private MenuInputHandler _inputHandler;
        private MenuDrawer _menuDrawer;

        private List<MenuItem> _itemsList;

        public MenuState()
        {
            _inputHandler = new MenuInputHandler(null);

            _menuDrawer = new MenuDrawer();

            _itemsList = new List<MenuItem>();

            var testItem = new MenuItem
            {
                Text = "Hello",
                Order = 1
            };

            _itemsList.Add(testItem);
        }

        public void Draw(Renderer renderer)
        {
            // New!
            // TODO: Pass in the menu container object to the drawer.
            var menuPrimitives = _menuDrawer.GetPrimitivesToRender(_itemsList);

            renderer.BeginRender();

            renderer.RenderTextPrimitives(menuPrimitives.ToArray());

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