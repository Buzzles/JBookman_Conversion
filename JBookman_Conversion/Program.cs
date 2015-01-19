using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using OpenTK;
using OpenTK.Audio;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
//message handling for testing
using System.Windows.Forms;
using System.Diagnostics;

//serialisation
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;


namespace JBookman_Conversion
{
   class Program
    {
      //program entry point.
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
        protected CPlayer m_Player;
        protected CMap g_CurrentMap;
        protected CMap g_FirstMap;
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

        int m_iUpdateCount=0;

        /// <summary>Creates a 800x600 window with the specified title.</summary>
        public Game()
            : base(800, 600, GraphicsMode.Default, "JBookman Conversion")
        {
            VSync = VSyncMode.On;
         Keyboard.KeyDown += HandleKeyboardKeyDown;
            
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
            m_sFirstMap = "Maps\\SerialiseTest2.map";
            m_iFirstMapType = (int) g_iLocationEnum.FIRSTTOWN;
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

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            Matrix4 modelview = Matrix4.LookAt(0.0f, 0.0f, 10.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f);

            modelview = m_moveMatrix * modelview;

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            GL.LoadMatrix(ref modelview);

            //  move down by 10;
            //  GL.Translate(0.0f, -0.0f, -10.0f);
            //  GL.Rotate(180, Vector3.UnitY);

            drawAxis();
            GL.Enable(EnableCap.Texture2D);
            DrawTiles();
            DrawPlayer();
            GL.Disable(EnableCap.Texture2D);

            SwapBuffers();
        }

        /////////////////////////////////
        //  JBOOKMAN RELATED METHODS
        ////////////////////////////////

        public CMapSector[,] GetSectors()
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
        public CPlayer GetPlayer()
        {
            return m_Player;
        }

      
        public void InitGame()
        {
            // Set map ID's (No longer needed?) 
            m_iCurrentMap = m_iFirstMapType;
            

            //  load textures
            LoadTilesets();

            //  Create new map instance as a loader then use it load the map (not ideal)
            g_FirstMap = new CMap();
            g_CurrentMap = new CMap();

            g_FirstMap = CMap.ReadMapFile(m_sFirstMap);
            

            //if firstmap has loaded, set it to be current map, otherwise it's failed to load
            if (g_FirstMap != null)
            {
                g_CurrentMap = g_FirstMap;
            }
            else
                MessageBox.Show("Map load failure");

            MessageBox.Show("Firstmap loaded rows: "+g_FirstMap.m_MapRows );
            MessageBox.Show("Firstmap loaded tile 1,1: " + g_FirstMap.m_MapSectors[1,1].Get_Tileset_Number());

            //  instantiate player + other objects not in CMap
            m_Player = new CPlayer();
            //  add player starting stats
            m_Player.SetGold(25);
            m_Player.SetHitPoints(10);
            m_Player.SetMaxHitPoints(10);
            m_Player.SetKeys(1);
            m_Player.SetSector(m_iStartSector);
            
            
            Random rand = new Random();
        }
    
        private void drawAxis()
        {
            /*  Axis Draw Code
             *  Simply draw 3 lines representing the 3D axis
             *  Blue = x axis (-left/+right)
             *  Red = y axis (+up/-down)
             *  Green = Z (depth)
             */
            
            //  test code
            GL.Begin(BeginMode.Lines);
            //  x = blue
            GL.Color3(0.0f, 0.0f, 1.0f);
            GL.Vertex3(-100.0f, 0.0f, 0.0f);
            GL.Vertex3(100.0f, 0.0f, 0.0f);
            //  y = red
            GL.Color3(1.0f, 0.0f, 0.0f);
            GL.Vertex3(0.0f, -110.0f, 0.0f);
            GL.Vertex3(0.0f, 100.0f, 0.0f);
            //  z = green
            GL.Color3(0.0f, 1.0f, 0.0f);
            GL.Vertex3(0.0f, 0.0f, -100.0f);
            GL.Vertex3(0.0f, 0.0f, 100.0f);
            GL.Color4(1.0f, 1.0f, 1.0f, 1.0f);

            GL.End();

        }

        private int SectorToRow(int sector, int rowCount)
        {
            int rowValue = sector / rowCount;

            return rowValue;
        }
        private int SectorToCols(int sector, int colCount)
        {
            int columnValue = sector % colCount;

            return columnValue;
        }
        
        ////////////////////////////////
        //      LOAD TEXTURE
        // 
        //      Might be worth moving to a new class for textures
        //
        //////////////////////////////// 
        
       
        private int LoadTextureFromFile(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                //  no texture supplied.
                throw new ArgumentException(fileName);
            }

            //  general internal texture ID
            int textureID = GL.GenTexture();
            //bind a named texture to a texturing target
            GL.BindTexture(TextureTarget.Texture2D, textureID);

            //  using System.Drawing.Bimap for bitmap file handling 
            //  (note: bitmaps are not just .bmp. png/jpeg etcc are all bitmaps)
            Bitmap bmp = null;
            try
            {

                bmp = new Bitmap(fileName);
                //  flip y axis as image will have top left 0.0 but GL has bottom left 0.0 origin.
                bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);

                //  lock part of bitmap in memory, in this case, the entire bitmap
                BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                //  create a texture from it.
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmpData.Width,
                    bmpData.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bmpData.Scan0);

                bmp.UnlockBits(bmpData);

                // We haven't uploaded mipmaps, so disable mipmapping (otherwise the texture will not appear).
                // On newer video cards, we can use GL.GenerateMipmaps() or GL.Ext.GenerateMipmaps() to create
                // mipmaps automatically. In that case, use TextureMinFilter.LinearMipmapLinear to enable them.
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

                //  Clamping methods
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureParameterName.ClampToEdge);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureParameterName.ClampToEdge);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapR, (int)TextureParameterName.ClampToEdge);
                
                // Clean up
                bmp.Dispose();
                

            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine("FnF " + e);
            }
            catch (ArgumentException e1)
            {
                Console.WriteLine("ArgExcp " + e1);
            }

            //return texture ID for use.
            return textureID;
        }

        private void LoadTilesets()
        {
            //load tilesets into textures.
            m_iPlayerTileSet = LoadTextureFromFile("Tilesets\\playerTile.png");
            m_iTownExtTileSet = LoadTextureFromFile("Tilesets\\tileset1.png");
            m_iTownIntTileSet = LoadTextureFromFile("Tilesets\\playerTile.bmp");
            m_iDungeonTileSet = LoadTextureFromFile("Tilesets\\playerTile.bmp");
            m_iWildernessTileSet = LoadTextureFromFile("Tilesets\\playerTile.bmp");
            

#if (TILETEST)
            int testTileSetID;
            testTileSetID = LoadTexture("Tilesets\\TestTileset.png");
            //tileSetID = LoadTexture("Tilesets\\tileset1.png");
            tileSetID2 = LoadTexture("Tilesets\\tileset2.png");
            m_iCurrentTileSet = testTileSetID;
#warning This code is not production ready
#else
            m_iCurrentTileSet = m_iTownExtTileSet;
#endif



        }




///////////////////////////////////////////
//        Drawing Code
/////////////////////////////////////////

      private void DrawTiles()
        {
            
         //Calculate visible columns
         int MinVisibleCol, MaxVisibleCol, MinVisibleRow, MaxVisibleRow;
         //int playerMapCol = m_Player.GetSector() % m_MapCols;
         //int playerMapRow = (int)m_Player.GetSector() / m_MapRows;
         
         //ushort m_MapCols =  g_CurrentMap.m_MapCols;
         //ushort m_MapRows = g_CurrentMap.m_MapRows;
         CMapSector[,] m_MapSectors = g_CurrentMap.m_MapSectors;

         int playerMapCol = SectorToCols(m_Player.GetSector(), g_CurrentMap.m_MapCols);
         int playerMapRow = SectorToRow(m_Player.GetSector(), g_CurrentMap.m_MapRows);
         MinVisibleCol = playerMapCol - Constants.NORMALVISIBLEPLAYERCOL;
         MaxVisibleCol = playerMapCol + Constants.NORMALVISIBLEPLAYERCOL;

         MinVisibleRow = playerMapRow - Constants.NORMALVISIBLEPLAYERROW;
         MaxVisibleRow = playerMapRow + Constants.NORMALVISIBLEPLAYERROW;
         //min and max cols
         if (playerMapCol < Constants.NORMALVISIBLEPLAYERCOL) //left
         {
             MinVisibleCol = 0;
             MaxVisibleCol = Constants.VISIBLECOLUMNCOUNT;
            
         }
         else if (playerMapCol > ((g_CurrentMap.m_MapCols - 1) - Constants.NORMALVISIBLEPLAYERCOL)) //right
         {
             MinVisibleCol = g_CurrentMap.m_MapCols - Constants.VISIBLECOLUMNCOUNT;
             MaxVisibleCol = g_CurrentMap.m_MapCols - 1;
         }
         //min/max rows
         if (playerMapRow < Constants.NORMALVISIBLEPLAYERROW) //top
         {
             MinVisibleRow = 0;
             MaxVisibleRow = Constants.VISIBLEROWCOUNT ;
         }
         else if (playerMapRow > ((g_CurrentMap.m_MapRows - 1) - Constants.NORMALVISIBLEPLAYERROW)) //bottom
         {
             MinVisibleRow = g_CurrentMap.m_MapRows - Constants.VISIBLEROWCOUNT;
             MaxVisibleRow = g_CurrentMap.m_MapRows - 1;
         }

          /////////////
          //Draw code
          ////////////
          int tile;
          GL.BindTexture(TextureTarget.Texture2D, m_iCurrentTileSet); //set texture
          int drawRow = 0; 
          for (int currRow = MinVisibleRow; currRow <=  MaxVisibleRow; currRow++) //row loop (y)
            {
                int drawCol = 0;
                
                for (int currCol = MinVisibleCol; currCol <= MaxVisibleCol; currCol++) //columns (x)
                {
                    //get the map tile value
                    var currentMapSector = m_MapSectors[currRow, currCol];
                    tile = currentMapSector.Get_Tileset_Number();
                    
                    //calulate tilenumber's row and column value on tileset
                    // int numberofcolumns = 2;
                    int row;
                    int column;
                    column = tile % Constants.TILESETCOLUMNCOUNT;
                    float texture_size = 1.0f / Constants.TILESETCOLUMNCOUNT;
                    //0.5 = size
                    row = (int)((tile * texture_size) + 0.00001f);
                    // MessageBox.Show("tile number: " + tile +" row: "+row+" col: "+column +" texturesize:"+texture_size);
                    float s1 = texture_size * (column + 0);
                    float s2 = texture_size * (column + 1);
                    float t1 = 1 - (texture_size * (row + 0));
                    float t2 = 1 - (texture_size * (row + 1));

                    // MessageBox.Show("s1 = "+s1+" s2 = "+s2+" t1 = "+t1+" t2 = "+t2);

                    //draw the quad
                   // GL.BindTexture(TextureTarget.Texture2D, tileSetID); //set texture --done above
                    //GL.Begin(BeginMode.Quads); 
                    
                    //Draw by drawing at different locations.
                    ////quad1
                    ////bottomleft
                    //GL.TexCoord2(s1, t2);
                    //GL.Vertex3((float)drawCol, -((float)drawRow) - 1.0f, 0.0f);  //vertex3(x,y,z)
                    ////top left
                    //GL.TexCoord2(s1, t1);
                    //GL.Vertex3((float)drawCol, -(float)drawRow, 0.0f);
                    ////top right
                    //GL.TexCoord2(s2, t1);
                    //GL.Vertex3((float)drawCol + 1.0f, -(float)drawRow, 0.0f);
                    ////bottom right
                    //GL.TexCoord2(s2, t2);
                    //GL.Vertex3((float)drawCol + 1.0f, -(float)drawRow - 1.0f, 0.0f);

                    //Proper GL way, translate grid, then draw at new 0.0
                    GL.PushMatrix(); //save
                    GL.LoadIdentity();
                    GL.Translate(drawCol, -drawRow, 0);
                    //Rotation to be handled here. Need to recenter the draw for this to work before shifting tiles around.
                    if (currentMapSector.rotationAngle != 0)
                    {
                       GL.Rotate(currentMapSector.rotationAngle, 0, 0, 1);
                    }
                    

                    GL.Begin(BeginMode.Quads);
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
                    
                    drawCol++;
                }
                drawRow++;  
          }
            //end of drawtiles
        }
/////////////////
// Draw player
/////////////////
      private void DrawPlayer()
      {
       // TODO: Move to seperate classes.
          
          
          /* int playerMapCol = m_Player.GetSector() % m_MapCols;
          int playerMapRow = (int)m_Player.GetSector() / m_MapRows;*/
                    
          int playerMapCol = SectorToCols(m_Player.GetSector(), g_CurrentMap.m_MapCols);
          int playerMapRow = SectorToRow(m_Player.GetSector(), g_CurrentMap.m_MapRows);

          int FinalVisiblePlayerCol = Constants.NORMALVISIBLEPLAYERCOL;
          int FinalVisiblePlayerRow = Constants.NORMALVISIBLEPLAYERROW;

          // Handling player being near edges of game world
          // by handling playercols/rows in respect to the viewport
          // reaching min or max movement

          //if location is left of last possible leftmost viewport centre line
          if (playerMapCol < Constants.NORMALVISIBLEPLAYERCOL)
          {
              FinalVisiblePlayerCol = 
                  Constants.NORMALVISIBLEPLAYERCOL - (Constants.NORMALVISIBLEPLAYERCOL - playerMapCol);
          }
          //else if location is right of last possible rightmost viewport centre line
          else if (playerMapCol > ((g_CurrentMap.m_MapCols - 1) - Constants.NORMALVISIBLEPLAYERCOL))
          {
              FinalVisiblePlayerCol =
                  Constants.VISIBLECOLUMNCOUNT - ((g_CurrentMap.m_MapCols - 1) - playerMapCol) - 1;
              //-1 on end is to take into account the visible display is 25 tiles wide, but range is 0 to 24.
          }
          //if location is top of last possible uppermost viewport centre line
          if (playerMapRow < Constants.NORMALVISIBLEPLAYERROW)
          {
              FinalVisiblePlayerRow =
                  Constants.NORMALVISIBLEPLAYERROW - (Constants.NORMALVISIBLEPLAYERROW - playerMapRow);
          }
          //else if location is below last possible lowermost viewport centre line
          else if (playerMapRow > ((g_CurrentMap.m_MapRows - 1) - Constants.NORMALVISIBLEPLAYERROW))
          {
              FinalVisiblePlayerRow =
                  Constants.VISIBLEROWCOUNT - ((g_CurrentMap.m_MapRows - 1) - playerMapRow) - 1;

          }
          //drawing the bastard.
          GL.BindTexture(TextureTarget.Texture2D, m_iPlayerTileSet); //set texture

          GL.Enable(EnableCap.Blend);
         // GL.BlendFunc(BlendingFactorSrc.SrcAlpha,BlendingFactorDest.OneMinusSrcAlpha);
          GL.BlendFunc(BlendingFactorSrc.SrcAlpha,BlendingFactorDest.OneMinusSrcAlpha);
          GL.LoadIdentity();
          GL.Begin(BeginMode.Quads);
          //quad1
          //bottomleft
          GL.TexCoord2(0, 0);
          GL.Vertex3((float)FinalVisiblePlayerCol, -((float)FinalVisiblePlayerRow) - 1.0f, 1.0f);
          //top left
          GL.TexCoord2(0, 1);
          GL.Vertex3((float)FinalVisiblePlayerCol, -(float)FinalVisiblePlayerRow, 1.0f);
          //top right
          GL.TexCoord2(1, 1);
          GL.Vertex3((float)FinalVisiblePlayerCol + 1.0f, -(float)FinalVisiblePlayerRow, 1.0f);
          //bottom right
          GL.TexCoord2(1, 0);
          GL.Vertex3((float)FinalVisiblePlayerCol + 1.0f, -(float)FinalVisiblePlayerRow - 1.0f, 1.0f);


          GL.End();

          GL.Disable(EnableCap.Blend);

//end of DrawPlayer()
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
            m_Player.SetSector(m_Player.GetSector() - g_CurrentMap.m_MapCols);
    }
    private void MovePlayerDown()
    {
        g_iDirection dir = g_iDirection.SOUTH;
        if (CharacterCanMove(dir, m_Player.GetSector()))
            m_Player.SetSector(m_Player.GetSector() + g_CurrentMap.m_MapCols);
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
            m_Player.SetSector(m_Player.GetSector() -1);
    }
    
    
    private bool CharacterCanMove(g_iDirection direction, int currentSector)
    {
        bool move = true;

        if (direction == g_iDirection.NORTH)
        {
            if (SectorToRow(currentSector, g_CurrentMap.m_MapRows) < 1)
            { 
                move = false;
                MessageBox.Show("North, \ntop of map, \nmove=false, \nSectorToRow=" + SectorToRow(currentSector, g_CurrentMap.m_MapRows));
            }
            else if (PlayerBlocked(currentSector, direction))
            { 
                move = false;
               // MessageBox.Show("North-elseif-playerblocked=true");
            }
        }
        if (direction == g_iDirection.SOUTH)
        {
            if (SectorToRow(currentSector, g_CurrentMap.m_MapRows) >= (g_CurrentMap.m_MapRows - 1))
            {
                move = false;
                MessageBox.Show("South, \nBottom of map, \nmove=false, \nSectorToRow=" + SectorToRow(currentSector, g_CurrentMap.m_MapRows));
            }
            else if (PlayerBlocked(currentSector, direction))
            {
                move = false;
            }
        }
        if (direction == g_iDirection.EAST)
        {
            if (SectorToCols(currentSector, g_CurrentMap.m_MapCols) >= (g_CurrentMap.m_MapCols - 1))
            { move = false;
            MessageBox.Show("East, \nRight of map, \nmove=false, \nSectorToCol=" + SectorToCols(currentSector, g_CurrentMap.m_MapCols));
            }
            else if (PlayerBlocked(currentSector, direction))
            { move = false; }
        }
        if (direction == g_iDirection.WEST)
        {
            if (SectorToCols(currentSector, g_CurrentMap.m_MapCols) <= 0) 
            { move = false;
            MessageBox.Show("West, \nLeft of map, \nmove=false, \nSectorToCol=" + SectorToCols(currentSector, g_CurrentMap.m_MapCols));
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
            iSector = iSector - g_CurrentMap.m_MapCols;
        }
        else if (direction == g_iDirection.SOUTH)
        {
            iSector = iSector + g_CurrentMap.m_MapCols;
        }
        else if (direction == g_iDirection.EAST)
        {
            iSector = iSector + 1;
        }
        else if (direction == g_iDirection.WEST)
        {
            iSector = iSector -1;
        }

        int y = SectorToRow(iSector, g_CurrentMap.m_MapRows);
        int x = SectorToCols(iSector, g_CurrentMap.m_MapCols);

        int item = g_CurrentMap.m_MapSectors[y, x].Get_Tileset_Number();

        g_iMapTypeEnum mapType = (g_iMapTypeEnum)g_CurrentMap.m_MapTypeID;
       

        //is tile set to be impassable?
        if (g_CurrentMap.m_MapSectors[y, x].Get_Is_Impassable() == true)
        {
            blocked = true;
        }

        /* 
         * OLD blocking code, horrible isn't it?
        switch (mapType)
        {
            case Constants.g_iMapTypeEnum.TOWNEXTERIOR:
                //stuff
                //MessageBox.Show("TOWNEXTERIOR in PlayerBlocked switch statement");
                if (item == Constants.BRICKWALL_EXT || item == Constants.VERTFENCE_EXT
                    || item == Constants.HORZFENCE_EXT || item == Constants.ANGLEFENCE_EXT
                    || item == Constants.TREE_EXT || item == Constants.WINDOW01_EXT
                    || item == Constants.WINDOW02_EXT || item == Constants.WELL_EXT
                    || item == Constants.FOUNTAIN_EXT || item == Constants.FOUNTAIN_EXT+1
                    || item == Constants.STONEWALL_EXT
                    || ((item >= Constants.WATER01_EXT) && (item <= Constants.WATER02_EXT)))
                    blocked = true;
                
                
                break;

            default: break;
        }
        */
        
        return blocked;
    }




    //end of Game class   
    }
//end-namespace
}


