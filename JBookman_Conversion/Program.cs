using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using OpenTK;
using OpenTK.Audio;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using JBookman_Conversion.Engine;

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
        protected int m_iCurrentMap, m_iFirstMapType, m_iStartSector;
        protected Player m_Player;
        protected Map g_CurrentMap;
        protected Map g_FirstMap;
        //  protected ushort m_MapTypeID;
        //  protected ushort m_MapCols;
        //  protected ushort m_MapRows;
        //  protected ushort[,] m_MapSectors;
        protected string m_sFirstMap;

        Matrix4 m_moveMatrix = new Matrix4();
        private const float moveAmount = 0.1f;

        //tileset vars
        int m_iPlayerTileSet = 0;
        int m_iTownExtTileSet = 0;
        int m_iTownIntTileSet = 0;
        int m_iDungeonTileSet = 0;
        int m_iWildernessTileSet = 0;
        int m_iCurrentTileSet = 0;

        int m_iUpdateCount = 0;

        /// <summary>Creates a 800x600 window with the specified title.</summary>
        public Game()
            : base(800, 600, GraphicsMode.Default, "JBookman Conversion")
        {
            VSync = VSyncMode.On;
            Keyboard.KeyDown += HandleKeyboardKeyDown;

            // Instantiate renderer here!
            // var renderer = new Renderer();
            // var engine = engine();
            // engine.LoadTextures();
            // engine.LoadMap();
        }

        /// <summary>Load resources here.</summary>
        /// <param name="e">Not used.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            //GL.ClearColor(0.1f, 0.2f, 0.5f, 0.0f);
            GL.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);
            GL.Enable(EnableCap.DepthTest);
            m_moveMatrix = Matrix4.Identity;
            m_moveMatrix = Matrix4.CreateTranslation(0.0f, 0.0f, 0.0f);

            //CENGINE(char* pstrMap, int iMapType, int iSector)
            // m_sFirstMap = "FirstMap.map";
            // m_sFirstMap = "Maps\\SerialiseMapTest01.map";
            m_sFirstMap = "Maps\\April2017.map";
            m_iFirstMapType = (int)g_iLocationEnum.FIRSTTOWN;
            //m_iStartSector = 1485;
            m_iStartSector = 85;
            InitGame();

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

            GL.Viewport(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);

            //  Matrix4 projection = Matrix4.CreateOrthographicOffCenter(0, 1024 / 56, -768 / 56, 0, -50.0f, 50.0f);

            //  working, calculing 25 cols and 18.75 rows.
            //  Matrix4 projection = Matrix4.CreateOrthographicOffCenter(0, 800 / 32, -600 / 32, 0, -50.0f, 50.0f);
            Matrix4 projection = Matrix4.CreateOrthographicOffCenter(0, 800 / 32, -19, 0, -50.0f, 50.0f);
            //  Matrix4 projection = Matrix4.CreateOrthographicOffCenter(0, 24, -18, 0, -50.0f, 50.0f);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.LoadMatrix(ref projection);
        }

        /// <summary>
        /// Called when it is time to setup the next frame. Add you game logic here.
        /// </summary>
        /// <param name="e">Contains timing information for framerate independent logic.</param>
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            //  KeyboardState keystate = OpenTK.Input.Keyboard.GetState();

            if (Keyboard[Key.Escape])
            { // Exit();
            }

            if (m_iUpdateCount == 15)
            {
                m_iUpdateCount = 0;
            }
            else m_iUpdateCount++;

            /*
               if (Keyboard[Key.Up])
               {
                   m_moveMatrix.M42 -= moveAmount;
               }
               if (Keyboard[Key.Down])
               {
                   m_moveMatrix.M42 += moveAmount;
               }
               if (Keyboard[Key.Left])
               {
                   m_moveMatrix.M41 += moveAmount;
               }
               if (Keyboard[Key.Right])
               {
                   m_moveMatrix.M41 -= moveAmount;
               }*/

            //  player location + viewport test code
            if (Keyboard[Key.Number1])
            {
                m_Player.SetSector(85);
            }
            if (Keyboard[Key.Number2])
            {
                m_Player.SetSector(842);
            }
            if (Keyboard[Key.Number3])
            {
                m_Player.SetSector(1530);
            }
            if (Keyboard[Key.Number4])
            {
                m_Player.SetSector(820);
            }
            if (Keyboard[Key.Number5])
            {
                m_Player.SetSector(1558);
            }
            if (Keyboard[Key.Number0])
            {
                m_Player.SetSector(0);
            }


            //  end of OnUpdateFrame
        }

        /*   private void HandleKeyPressEvent(object sender, OpenTK.KeyPressEventArgs e)
           {
               MessageBox.Show("HandleKeyPressEvent triggered by: "+e.KeyChar);


           }*/
        private void HandleKeyboardKeyDown(object sender, KeyboardKeyEventArgs e)
        {
            //  MessageBox.Show("HandleKeyPressEvent triggered by: " + e.Key);

            switch (e.Key)
            {
                case Key.Escape:
                    Exit();
                    break;
                default:
                    //  Default. Do nothing for now
                    break;
                case Key.Up:
                    HandleUpArrow();
                    break;
                case Key.Down:
                    HandleDownArrow();
                    break;
                case Key.Left:
                    HandleLeftArrow();
                    break;
                case Key.Right:
                    HandleRightArrow();
                    break;
            }
        }

        /// <summary>
        /// Called when it is time to render the next frame. Add your rendering code here.
        /// </summary>
        /// <param name="e">Contains timing information.</param>
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            Renderer.Render(g_CurrentMap, m_iCurrentTileSet, m_iPlayerTileSet, m_Player, m_moveMatrix);

            SwapBuffers();
        }

        /////////////////////////////////
        //  JBOOKMAN RELATED METHODS
        ////////////////////////////////

        public MapSector[,] GetSectors()
        {
            //  return m_MapSectors;
            return g_CurrentMap.m_MapSectors;
        }
        public int GetCurrentMap()
        {
            return m_iCurrentMap;
        }
        public void SetCurrentMap(int map)
        {
            m_iCurrentMap = map;
        }
        public Player GetPlayer()
        {
            return m_Player;
        }

        public void InitGame()
        {
            // Set map ID's (No longer needed?) 
            m_iCurrentMap = m_iFirstMapType;

            //  load textures
            Core.LoadTilesets();

            // To remove!
            m_iPlayerTileSet = Core.PlayerTileSetId;
            m_iTownExtTileSet = Core.TownExtTileSetId;
            m_iTownIntTileSet = Core.TownIntTileSetId;
            m_iDungeonTileSet = Core.DungeonTileSetId;
            m_iWildernessTileSet = Core.WildernessTileSetId;
            m_iCurrentTileSet = Core.CurrentTileSetId;

            //  Create new map instance as a loader then use it load the map (not ideal)
            g_FirstMap = new Map();
            g_CurrentMap = new Map();

            g_FirstMap = Map.ReadMapFile(m_sFirstMap);

            //if firstmap has loaded, set it to be current map, otherwise it's failed to load
            if (g_FirstMap != null)
            {
                g_CurrentMap = g_FirstMap;
            }
            else
                MessageBox.Show("Map load failure");

            MessageBox.Show("Firstmap loaded rows: " + g_FirstMap.MapRows);
            MessageBox.Show("Firstmap loaded tile 1,1: " + g_FirstMap.m_MapSectors[1, 1].TileNumberId);

            //  instantiate player + other objects not in CMap
            m_Player = new Player();
            //  add player starting stats
            m_Player.SetGold(25);
            m_Player.SetHitPoints(10);
            m_Player.SetMaxHitPoints(10);
            m_Player.SetKeys(1);
            m_Player.SetSector(m_iStartSector);

            Random rand = new Random();
        }

        //////////////////////
        //
        // Movement handling
        //
        ///////////////////////

        private void HandleUpArrow()
        {
            MovePlayerUp();
        }
        private void HandleDownArrow()
        {
            MovePlayerDown();
        }
        private void HandleLeftArrow()
        {
            MovePlayerLeft();
        }
        private void HandleRightArrow()
        {
            MovePlayerRight();
        }


        private void MovePlayerUp()
        {
            g_iDirection dir = g_iDirection.NORTH;
            if (CharacterCanMove(dir, m_Player.GetSector()))
                m_Player.SetSector(m_Player.GetSector() - g_CurrentMap.MapCols);
        }
        private void MovePlayerDown()
        {
            g_iDirection dir = g_iDirection.SOUTH;
            if (CharacterCanMove(dir, m_Player.GetSector()))
                m_Player.SetSector(m_Player.GetSector() + g_CurrentMap.MapCols);
        }
        private void MovePlayerRight()
        {
            g_iDirection dir = g_iDirection.EAST;
            if (CharacterCanMove(dir, m_Player.GetSector()))
                m_Player.SetSector(m_Player.GetSector() + 1);
        }
        private void MovePlayerLeft()
        {
            g_iDirection dir = g_iDirection.WEST;
            if (CharacterCanMove(dir, m_Player.GetSector()))
                m_Player.SetSector(m_Player.GetSector() - 1);
        }

        private bool CharacterCanMove(g_iDirection direction, int currentSector)
        {
            bool move = true;

            if (direction == g_iDirection.NORTH)
            {
                if (MapUtils.SectorToRow(currentSector, g_CurrentMap.MapRows) < 1)
                {
                    move = false;
                    MessageBox.Show("North, \ntop of map, \nmove=false, \nSectorToRow=" + MapUtils.SectorToRow(currentSector, g_CurrentMap.MapRows));
                }
                else if (PlayerBlocked(currentSector, direction))
                {
                    move = false;
                    // MessageBox.Show("North-elseif-playerblocked=true");
                }
            }
            if (direction == g_iDirection.SOUTH)
            {
                if (MapUtils.SectorToRow(currentSector, g_CurrentMap.MapRows) >= (g_CurrentMap.MapRows - 1))
                {
                    move = false;
                    MessageBox.Show("South, \nBottom of map, \nmove=false, \nSectorToRow=" + MapUtils.SectorToRow(currentSector, g_CurrentMap.MapRows));
                }
                else if (PlayerBlocked(currentSector, direction))
                {
                    move = false;
                }
            }
            if (direction == g_iDirection.EAST)
            {
                if (MapUtils.SectorToCols(currentSector, g_CurrentMap.MapCols) >= (g_CurrentMap.MapCols - 1))
                {
                    move = false;
                    MessageBox.Show("East, \nRight of map, \nmove=false, \nSectorToCol=" + MapUtils.SectorToCols(currentSector, g_CurrentMap.MapCols));
                }
                else if (PlayerBlocked(currentSector, direction))
                { move = false; }
            }
            if (direction == g_iDirection.WEST)
            {
                if (MapUtils.SectorToCols(currentSector, g_CurrentMap.MapCols) <= 0)
                {
                    move = false;
                    MessageBox.Show("West, \nLeft of map, \nmove=false, \nSectorToCol=" + MapUtils.SectorToCols(currentSector, g_CurrentMap.MapCols));
                }
                else if (PlayerBlocked(currentSector, direction))
                { move = false; }
            }

            return move;
        }

        private bool PlayerBlocked(int iPlayerSector, g_iDirection direction)
        {
            bool blocked = false;
            int iSector = iPlayerSector;

            if (direction == g_iDirection.NORTH)
            {
                iSector = iSector - g_CurrentMap.MapCols;
            }
            else if (direction == g_iDirection.SOUTH)
            {
                iSector = iSector + g_CurrentMap.MapCols;
            }
            else if (direction == g_iDirection.EAST)
            {
                iSector = iSector + 1;
            }
            else if (direction == g_iDirection.WEST)
            {
                iSector = iSector - 1;
            }

            int y = MapUtils.SectorToRow(iSector, g_CurrentMap.MapRows);
            int x = MapUtils.SectorToCols(iSector, g_CurrentMap.MapCols);

            int item = g_CurrentMap.m_MapSectors[y, x].TileNumberId;

            g_iMapTypeEnum mapType = (g_iMapTypeEnum)g_CurrentMap.MapTypeId;

            //is tile set to be impassable?
            if (g_CurrentMap.m_MapSectors[y, x].Impassable == true)
            {
                blocked = true;
            }

            return blocked;
        }
    }
}