using OpenTK.Input;
using JBookman_Conversion.EngineBits.Consts;
using JBookman_Conversion.EngineBits.StateManagers;
using JBookman_Conversion.EngineBits;

namespace JBookman_Conversion.GameStates.MenuComponents
{
    public class MenuInputHandler
    {
        private KeyboardState _lastKeyState, _keyboardState;

        public MenuInputHandler(object menuComponent)
        {
            // TODO: Make available the array of menu items to know currently selected one.
        }

        internal void HandleKeyboardDown(KeyboardState keyboardState)
        {
            _keyboardState = keyboardState;

            if (KeyPress(Key.Up))
            {
            }

            if (KeyPress(Key.Down))
            {
            }

            if (KeyPress(Key.Enter) || KeyPress(Key.KeypadEnter))
            {
            }
        }

        // TODO: Move to a base for input handler
        private bool KeyPress(Key key)
        {
            return (_keyboardState[key] && (_keyboardState[key] != _lastKeyState[key]));
        }
    }
}
