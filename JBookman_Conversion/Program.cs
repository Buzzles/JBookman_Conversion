using JBookman_Conversion.EngineBits;
using JBookman_Conversion.EngineBits.Abstract;
using JBookman_Conversion.GameStates;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Input;
using System;

namespace JBookman_Conversion
{
    class Program
    {
        [STAThread]
        static void Main()
        {
            using (Game game = new Game())
            {
                game.Run(30.0);
            }
        }
    }

    class Game : GameWindow
    {
        //tileset vars
        int m_iPlayerTileSet = 0;
        int m_iTownExtTileSet = 0;
        int m_iTownIntTileSet = 0;
        int m_iDungeonTileSet = 0;
        int m_iWildernessTileSet = 0;
        int m_iCurrentTileSet = 0;

        int m_iUpdateCount = 0;
        private readonly Renderer _renderer;

        private Engine _engine;

        private KeyboardState lastState;

        /// <summary>Creates a 800x600 window with the specified title.</summary>
        public Game()
            : base(800, 600, GraphicsMode.Default, "JBookman Conversion")
        {
            VSync = VSyncMode.On;

            // Instantiate renderer here!
            _renderer = new Renderer();
            // var engine = engine();
            // engine.LoadTextures();
            // engine.LoadMap();

            _engine = new Engine();
        }

        /// <summary>Load resources here.</summary>
        /// <param name="e">Not used.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            _renderer.Initialise();

            //CENGINE(char* pstrMap, int iMapType, int iSector)
            // m_sFirstMap = "FirstMap.map";
            // m_sFirstMap = "Maps\\SerialiseMapTest01.map";
            var mapName = "Maps\\April2017.map";
            var mapType = (int)g_iLocationEnum.FIRSTTOWN;

            //m_iStartSector = 1485;
            var startSector = 85;

            // Should initing the player + map be part of this?
            _engine.InitPlayer(startSector);
            _engine.InitMap(mapName, mapType);
            _engine.InitialiseEngine();

            // TODO: Load stuff for menu

            // Load textures
            Core.LoadTilesets();

            // To remove!
            m_iPlayerTileSet = Core.PlayerTileSetId;
            m_iTownExtTileSet = Core.TownExtTileSetId;
            m_iTownIntTileSet = Core.TownIntTileSetId;
            m_iDungeonTileSet = Core.DungeonTileSetId;
            m_iWildernessTileSet = Core.WildernessTileSetId;
            m_iCurrentTileSet = Core.CurrentTileSetId;

            _engine.StateManager.AddNewState(new MenuState());

            // Hack
            _renderer.MainTileSetTextureId = m_iCurrentTileSet;
            _renderer.PlayerTileSetTextureId = m_iPlayerTileSet;

            var worldState = new WorldState(_engine._currentMap, _engine._player);

            _engine.StateManager.AddNewState(worldState);
        }

        /// <summary>
        /// Called when your window is resized. Set your viewport here. It is also
        /// a good place to set up your projection matrix (which probably changes
        /// along when the aspect ratio of your window).
        /// </summary>
        /// <param name="e">Not used.</param>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            _renderer.OnResize(this);
        }

        /// <summary>
        /// Called when it is time to setup the next frame. Add you game logic here.
        /// </summary>
        /// <param name="e">Contains timing information for framerate independent logic.</param>
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            _engine.InputHandler.HandleKeyboardDown(e, this, _engine);

            KeyboardState keystate = Keyboard.GetState();

            if (keystate != lastState)
            {
                if (keystate.IsKeyDown(Key.Up))
                { }

                if (keystate.IsKeyDown(Key.Escape))
                { }
            }
            // log last state to see if the state has changed or if it's just an update cycle
            lastState = keystate;

            if (Keyboard[Key.Escape])
            {
                // Exit();
            }

            if (m_iUpdateCount == 15)
            {
                m_iUpdateCount = 0;
            }
            else m_iUpdateCount++;

            var player = _engine.GetPlayer();

            _engine.StateManager.UpdateCurrentState();

            ////  player location + viewport test code
            //if (Keyboard[Key.Number1])
            //{
            //    player.SetSector(85);
            //}
            //if (Keyboard[Key.Number2])
            //{
            //    player.SetSector(842);
            //}
            //if (Keyboard[Key.Number3])
            //{
            //    player.SetSector(1530);
            //}
            //if (Keyboard[Key.Number4])
            //{
            //    player.SetSector(820);
            //}
            //if (Keyboard[Key.Number5])
            //{
            //    player.SetSector(1558);
            //}
            //if (Keyboard[Key.Number0])
            //{
            //    player.SetSector(0);
            //}

            //if (Keyboard[Key.F1])
            //{
            //    _engine.StateHandler.MoveNext(StateHandler.ProcessAction.GoToMenu);
            //}

            //if (Keyboard[Key.F2])
            //{
            //    _engine.StateHandler.MoveNext(StateHandler.ProcessAction.ToWorld);
            //}

            //if (Keyboard[Key.F2])
            //{
            //    _engine.StateHandler.MoveNext(StateHandler.ProcessAction.BattleStart);
            //}

            //  end of OnUpdateFrame
        }


        // OLD, TO BIN!
        private void HandleKeyboardKeyDown(object sender, KeyboardKeyEventArgs e)
        {
            //  MessageBox.Show("HandleKeyPressEvent triggered by: " + e.Key);
        }

        /// <summary>
        /// Called when it is time to render the next frame. Add your rendering code here.
        /// </summary>
        /// <param name="e">Contains timing information.</param>
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            var player = _engine.GetPlayer();
            var map = _engine.GetMap();
            var m_moveMatrix = _renderer.MoveMatrix;

            _engine.StateManager.DrawCurrentState(_renderer);

            ////if (_engine.StateManager.CurrentState == EngineBits.Consts.ProcessState.Menu)
            ////{
            ////    //_engine.StateManager.DrawCurrentState();
            ////}
            ////else
            ////{
            ////    //StaticRenderer.Render(map, m_iCurrentTileSet, m_iPlayerTileSet, player, m_moveMatrix);
            ////    //_renderer.RenderFrame();
            ////}

            SwapBuffers();
        }
    }
}