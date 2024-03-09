using OpenTK.Input;
using MessageBox = System.Windows.Forms.MessageBox;
using OpenTK;
using JBookman_Conversion.EngineBits.Consts;

namespace JBookman_Conversion.EngineBits
{
    internal class InputHandler
    {
        private KeyboardState _lastKeyState, _keyboardState;

        public InputHandler()
        {
        }

        internal void HandleKeyboardDown(FrameEventArgs e, Game gameContext, Engine engineContext)
        {
            _keyboardState = Keyboard.GetState();

            if (KeyPress(Key.Q))
            {
                gameContext.Exit();
            }

            // State change!
            var engine = engineContext;

            if (KeyPress(Key.F1))
            {
                engine.StateManager.MoveNext(ProcessAction.GoToMenu);
                //engine.StateHandler.MoveNext(ProcessAction.GoToMenu)
            }

            if (KeyPress(Key.F2))
            {
                //engine.StateHandler.MoveNext(ProcessAction.ToWorld);
                engine.StateManager.MoveNext(ProcessAction.ToWorld);

            }

            if (KeyPress(Key.F3))
            {
                engine.StateManager.MoveNext(ProcessAction.BattleStart);
                //engine.StateHandler.MoveNext(ProcessAction.BattleStart);
            }

            // Store for next update method
            _lastKeyState = _keyboardState;
        }

        private bool KeyPress(Key key)
        {
            return (_keyboardState[key] && (_keyboardState[key] != _lastKeyState[key]));
        }
    }
}
