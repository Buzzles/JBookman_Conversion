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
        protected CPeople m_People;
        protected CDoors m_Doors;
        protected CContainers m_Containers;
        
        protected ushort m_MapTypeID;
        protected ushort m_MapCols;
        protected ushort m_MapRows;

        protected ushort[,] m_MapSectors;
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
            m_sFirstMap = "FirstMap.map";
            m_iFirstMapType = (int) Constants.g_iLocationEnum.FIRSTTOWN;
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
            
            //Matrix4 projection = Matrix4.CreateOrthographicOffCenter(0, 1024 / 56, -768 / 56, 0, -50.0f, 50.0f);
            
            //working, calculing 25 cols and 18.75 rows.
           // Matrix4 projection = Matrix4.CreateOrthographicOffCenter(0, 800 / 32, -600 / 32, 0, -50.0f, 50.0f);
            Matrix4 projection = Matrix4.CreateOrthographicOffCenter(0, 800 / 32, -19, 0, -50.0f, 50.0f);
            //Matrix4 projection = Matrix4.CreateOrthographicOffCenter(0, 24, -18, 0, -50.0f, 50.0f);
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

            //KeyboardState keystate = OpenTK.Input.Keyboard.GetState();

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
               
                //player location + viewport test code
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
         
           
        //end of OnUpdateFrame
        }

     /*   private void HandleKeyPressEvent(object sender, OpenTK.KeyPressEventArgs e)
        {
            MessageBox.Show("HandleKeyPressEvent triggered by: "+e.KeyChar);
                               

        }*/
        private void HandleKeyboardKeyDown(object sender, KeyboardKeyEventArgs e)
        {
            //MessageBox.Show("HandleKeyPressEvent triggered by: " + e.Key);
            
             switch (e.Key)
            {
                 case Key.Escape:
                    Exit();
                    break;
                 default: 
                     //Default. Do nothing for now
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

            //move down by 10;
          //  GL.Translate(0.0f, -0.0f, -10.0f);
            // GL.Rotate(180, Vector3.UnitY);

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

        public ushort[,] GetSectors()
        {
            return m_MapSectors;
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
            OpenMapFiles(m_sFirstMap);
            m_iCurrentMap = m_iFirstMapType;
            //m_Sectors = new short[Constants.MAPSECTORCOUNT];

            //load textures
            LoadTilesets();

            //instantiate objects
            m_Player = new CPlayer();
            m_People = new CPeople();
            m_Doors = new CDoors();
            m_Containers = new CContainers();
            
            //add player starting stats
            m_Player.SetGold(25);
            m_Player.SetHitPoints(10);
            m_Player.SetMaxHitPoints(10);
            m_Player.SetKeys(1);
            m_Player.SetSector(m_iStartSector);
            
            
            Random rand = new Random();
        }
        public void OpenMapFiles(string fName)
        {
            string mapFileName = ConstructFileName(fName);
            MessageBox.Show("Mapfilename: " + mapFileName);
            ReadMapFile(mapFileName);
            ReadPeopleFile(mapFileName);
            ReadContainerFile(mapFileName);
            ReadDoorFile(mapFileName);

        }



        //private methods
        public string ConstructFileName(string fileName)
        {
            //string newFileName = "\\Map";
            string newFileName = string.Concat("Maps\\", fileName);

            return newFileName;
        }
        
        
        private void ReadMapFile(string fileName)
        {
            
            BinaryReader binReader;
            try
            {
                //set up bin reader and try and open the file
                binReader = new BinaryReader(File.Open(fileName, FileMode.Open));

                //test as suggested by BinaryReader msdn article
                byte[] testArray = new byte[3];
                int count = binReader.Read(testArray, 0, 3);

                if (count != 0)
                {
                    //reset reader
                    binReader.BaseStream.Seek(0, SeekOrigin.Begin);

                    
                    //get map type first
                    m_MapTypeID = binReader.ReadUInt16();

                    //read columns
                    m_MapCols = binReader.ReadUInt16();

                    //read rows
                    m_MapRows = binReader.ReadUInt16();

                    m_MapSectors = new ushort[m_MapRows, m_MapCols];

                    //read sectors
                    for (short y = 0; y < m_MapRows; y++)
                    {
                        for (short x = 0; x < m_MapCols; x++)
                        {
                            ushort currentSector = 0;
                            currentSector = binReader.ReadUInt16();
                            //map_Sectors[y][x] = currentSector;
                            m_MapSectors[y, x] = currentSector;
                        }

                    }

                    MessageBox.Show("Map loaded");
                }
                binReader.Close();
            }
            catch (EndOfStreamException e)
            {
                //end of stream exception.
                //in case of this, should wack some default values into the m_People object via instantiation.
                string errorString = e.ToString();
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine("Person File not found exception: " + e);
                MessageBox.Show("File not found");
            }
            catch (DirectoryNotFoundException e)
            {
                Console.WriteLine("Directory not found exception; "+e);
                
            }
            //binReader.Close();

        }
       
        public void ReadPeopleFile(string fileName)
        {
            //append .peo to fileName string to access
            //correct people file.
            string peopleFileName = string.Concat(fileName, ".peo");
            string buffer=null;
            BinaryReader binReader;
            try
            {
                //set up bin reader, try and read file
                binReader = new BinaryReader(File.Open(peopleFileName, FileMode.Open));
                
                //test as suggested by BinaryReader msdn article
                byte[] testArray = new byte[3];
                int count = binReader.Read(testArray, 0, 3);

                if(count != 0)
                {
                    //reset reader
                    binReader.BaseStream.Seek(0,SeekOrigin.Begin);

                    //get number of people in file
                    //buffer = GetStringFromFile(binReader, buffer);
                    buffer = binReader.ReadString();
                    m_People.SetPersonCount(int.Parse(buffer));

                    for (int personCount = 0; personCount < m_People.GetPersonCount(); personCount++)
                    {
                       CPerson newPerson = new CPerson();
                       newPerson.SetName(binReader.ReadString());
                       newPerson.SetSector(int.Parse(binReader.ReadString()));
                       newPerson.SetCanMove(bool.Parse(binReader.ReadString()));
                       newPerson.SetTile(int.Parse(binReader.ReadString()));
                       m_People.AddPerson(newPerson, personCount);
                     }

                }
                binReader.Close();   
            }
            catch(EndOfStreamException e)
            { 
                //end of stream exception.
                //in case of this, should wack some default values into the m_People object via instantiation.
                string errorString = e.ToString();
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine("Person File not found exception: " + e);
            }
            catch (DirectoryNotFoundException e)
            {
                Console.WriteLine("Directory not found exception; " + e);
            }
            //binReader.Close();

        }
        public void ReadContainerFile(string fileName)
        {
            //append .itm to map file (eg, FirstTown.map.itm)
            string containerFileName = string.Concat(fileName, ".itm");
            string buffer = null;
            BinaryReader binReader; 
            try
            {
                //set up binreader, try and read file
                binReader = new BinaryReader(File.Open(containerFileName, FileMode.Open));
                //test as suggested by BinaryReader msdn article
                byte[] testArray = new byte[3];
                int count = binReader.Read(testArray, 0, 3);

                if (count != 0)
                {
                    //reset reader
                    binReader.BaseStream.Seek(0, SeekOrigin.Begin);

                    //get number of containers in mapfile
                    //buffer = GetStringFromFile(binReader, buffer);
                    buffer = binReader.ReadString();
                    m_Containers.SetContainerCount(int.Parse(buffer));
                    

                    for (int containerCount = 0; containerCount < m_Containers.GetContainerCount(); containerCount++)
                    {
                        CContainer newContainer = new CContainer();
                        //////////////////////
                        //what's in the box?
                        ////////////////
                        //gold
                        newContainer.SetGold(int.Parse(binReader.ReadString()));
                        //keys
                        newContainer.SetKeys(int.Parse(binReader.ReadString()));
                        //potions
                        newContainer.SetPotion(int.Parse(binReader.ReadString()));
                        //armour type
                        newContainer.SetArmour(int.Parse(binReader.ReadString()));
                        //weapon type
                        newContainer.SetWeapon(int.Parse(binReader.ReadString()));
                        //locked?
                        buffer = binReader.ReadString();
                        if (buffer.Equals("F"))
                            newContainer.SetLocked(false);
                        else
                            newContainer.SetLocked(true);
                        //sector
                        newContainer.SetSector(int.Parse(binReader.ReadString()));
                        //tile numer
                        newContainer.SetTile(int.Parse(binReader.ReadString()));

                        m_Containers.AddContainer(newContainer, containerCount);
                    }

                }
                binReader.Close();
            }
            catch (EndOfStreamException e)
            {
                //end of stream exception.
                //in case of this, should wack some default values into the m_Containers object via instantiation.
                string errorString = e.ToString();
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine("Container File not found exception: " + e);
            }
            catch (DirectoryNotFoundException e)
            {
                Console.WriteLine("Directory not found exception; " + e);
            }
           
        }
        
        /////////////////////////////
        // Reading the door files
        ////////////////////////////
        public void ReadDoorFile(string fileName)
        {
            //append .dor to map file (eg, FirstTown.map.dor)
            string doorFileName = string.Concat(fileName, ".dor");
            string buffer = null;
            BinaryReader binReader;
            try
            {
                //set up binreader, try to read file
                binReader = new BinaryReader(File.Open(doorFileName, FileMode.Open));
                
                //test as suggested by BinaryReader msdn article
                byte[] testArray = new byte[3];
                int count = binReader.Read(testArray, 0, 3);

                if (count != 0)
                {
                    //reset reader
                    binReader.BaseStream.Seek(0, SeekOrigin.Begin);

                    //get number of doors in file
                    //buffer = GetStringFromFile(binReader, buffer);
                    buffer = binReader.ReadString();
                    m_People.SetPersonCount(int.Parse(buffer));

                    for (int doorCount = 0; doorCount < m_Doors.GetDoorCount(); doorCount++)
                    {
                        CDoor newDoor = new CDoor();
                        //secret?
                        buffer = binReader.ReadString();
                        if (buffer.Equals("F"))
                            newDoor.SetSecret(false);
                        else
                            newDoor.SetSecret(true);
                        //locked?
                        buffer = binReader.ReadString();
                        if (buffer.Equals("F"))
                            newDoor.SetLocked(false);
                        else
                            newDoor.SetLocked(true);
                        //sector
                        newDoor.SetSector(int.Parse(binReader.ReadString()));
                        //tile number
                        newDoor.SetTile(int.Parse(binReader.ReadString()));
                    }

                }
                binReader.Close();
            }
            catch (EndOfStreamException e)
            {
                //end of stream exception.
                //in case of this, should wack some default values into the m_Doors object via instantiation.
                string errorString = e.ToString();
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine("Door file not found exception: " + e);
            }
            catch (DirectoryNotFoundException e)
            {
                Console.WriteLine("Directory not found exception; " + e);
            }
            

        }
        /*
        public void GetStringFromFile(StreamReader streamIn, string buf)
        {

        }
        */
        public string GetStringFromFile(BinaryReader streamIn, string buf)
        {
           /* while (streamIn.PeekChar() != 13 && streamIn.PeekChar() != -1)
            {
                buf += streamIn.ReadChar();

            }*/

            buf = streamIn.ReadString();
            return buf;
        }


        private void drawAxis()
        {
            //test code
            GL.Begin(BeginMode.Lines);
            // x = blue
            GL.Color3(0.0f, 0.0f, 1.0f);
            GL.Vertex3(-100.0f, 0.0f, 0.0f);
            GL.Vertex3(100.0f, 0.0f, 0.0f);
            // y = red
            GL.Color3(1.0f, 0.0f, 0.0f);
            GL.Vertex3(0.0f, -110.0f, 0.0f);
            GL.Vertex3(0.0f, 100.0f, 0.0f);
            // z = green
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
        ////////////////////////////////
        // 
        //     LOAD TEXTURE
        //
        //////////////////////////////// 
        ////////////////////////////////
        


        private int LoadTextureFromFile(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                //no texture supplied.
                throw new ArgumentException(fileName);
            }

            //general internal texture ID
            int textureID = GL.GenTexture();
            //bind a named texture to a texturing target
            GL.BindTexture(TextureTarget.Texture2D, textureID);

            //using System.Drawing.Bimap for bitmap file handling 
            //(note: bitmaps are not just .bmp. png/jpeg etcc are all bitmaps)
            Bitmap bmp = null;
            try
            {

                bmp = new Bitmap(fileName);
                //flip y axis as image will have top left 0.0 but GL has bottom left 0.0 origin.
                bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);

                //lock part of bitmap in memory, in this case, the entire bitmap
                BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                //create a texture from it.
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmpData.Width,
                    bmpData.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bmpData.Scan0);

                bmp.UnlockBits(bmpData);

                // We haven't uploaded mipmaps, so disable mipmapping (otherwise the texture will not appear).
                // On newer video cards, we can use GL.GenerateMipmaps() or GL.Ext.GenerateMipmaps() to create
                // mipmaps automatically. In that case, use TextureMinFilter.LinearMipmapLinear to enable them.
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

                //clamping
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureParameterName.ClampToEdge);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureParameterName.ClampToEdge);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapR, (int)TextureParameterName.ClampToEdge);


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
            m_iPlayerTileSet = LoadTextureFromFile("Tilesets\\playerTile.bmp");
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
         int playerMapCol = SectorToCols(m_Player.GetSector(), m_MapCols);
         int playerMapRow = SectorToRow(m_Player.GetSector(), m_MapRows);
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
         else if (playerMapCol > ((m_MapCols - 1) - Constants.NORMALVISIBLEPLAYERCOL)) //right
         {
             MinVisibleCol = m_MapCols - Constants.VISIBLECOLUMNCOUNT;  
             MaxVisibleCol = m_MapCols-1;
         }
         //min/max rows
         if (playerMapRow < Constants.NORMALVISIBLEPLAYERROW) //top
         {
             MinVisibleRow = 0;
             MaxVisibleRow = Constants.VISIBLEROWCOUNT ;
         }
         else if (playerMapRow > ((m_MapRows - 1) - Constants.NORMALVISIBLEPLAYERROW)) //bottom
         {
             MinVisibleRow = m_MapRows - Constants.VISIBLEROWCOUNT; 
             MaxVisibleRow = m_MapRows-1;
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
                    tile = m_MapSectors[currRow, currCol];

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
                   // GL.BindTexture(TextureTarget.Texture2D, tileSetID); //set texture
                    
                    GL.Begin(BeginMode.Quads);
                    //quad1
                    //bottomleft
                    GL.TexCoord2(s1, t2);
                    GL.Vertex3((float)drawCol, -((float)drawRow) - 1.0f, 0.0f);  //vertex3(x,y,z)
                    //top left
                    GL.TexCoord2(s1, t1);
                    GL.Vertex3((float)drawCol, -(float)drawRow, 0.0f);
                    //top right
                    GL.TexCoord2(s2, t1);
                    GL.Vertex3((float)drawCol + 1.0f, -(float)drawRow, 0.0f);
                    //bottom right
                    GL.TexCoord2(s2, t2);
                    GL.Vertex3((float)drawCol + 1.0f, -(float)drawRow - 1.0f, 0.0f);


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
         /* int playerMapCol = m_Player.GetSector() % m_MapCols;
          int playerMapRow = (int)m_Player.GetSector() / m_MapRows;*/

          int playerMapCol = SectorToCols(m_Player.GetSector(), m_MapCols);
          int playerMapRow = SectorToRow(m_Player.GetSector(), m_MapRows);

          int FinalVisiblePlayerCol = Constants.NORMALVISIBLEPLAYERCOL;
          int FinalVisiblePlayerRow = Constants.NORMALVISIBLEPLAYERROW;

          //
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
          else if(playerMapCol > ((m_MapCols-1)-Constants.NORMALVISIBLEPLAYERCOL))
          {
              FinalVisiblePlayerCol =
                  Constants.VISIBLECOLUMNCOUNT - ((m_MapCols - 1) - playerMapCol) - 1;
              //-1 on end is to take into account the visible display is 25 tiles wide, but range is 0 to 24.
          }
          //if location is top of last possible uppermost viewport centre line
          if (playerMapRow < Constants.NORMALVISIBLEPLAYERROW)
          {
              FinalVisiblePlayerRow =
                  Constants.NORMALVISIBLEPLAYERROW - (Constants.NORMALVISIBLEPLAYERROW - playerMapRow);
          }
          //else if location is below last possible lowermost viewport centre line
          else if (playerMapRow > ((m_MapRows - 1) - Constants.NORMALVISIBLEPLAYERROW))
          {
              FinalVisiblePlayerRow =
                  Constants.VISIBLEROWCOUNT - ((m_MapRows - 1) - playerMapRow)-1;

          }
          //drawing the bastard.
          GL.BindTexture(TextureTarget.Texture2D, m_iPlayerTileSet); //set texture

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
        Constants.g_iDirection dir = Constants.g_iDirection.NORTH;
        if (CharacterCanMove(dir, m_Player.GetSector()))
            m_Player.SetSector(m_Player.GetSector() - m_MapCols);
    }
    private void MovePlayerDown()
    {
        Constants.g_iDirection dir = Constants.g_iDirection.SOUTH;
        if (CharacterCanMove(dir, m_Player.GetSector()))
            m_Player.SetSector(m_Player.GetSector() + m_MapCols);
    }
    private void MovePlayerRight()
    {
        Constants.g_iDirection dir = Constants.g_iDirection.EAST;
        if (CharacterCanMove(dir, m_Player.GetSector()))
            m_Player.SetSector(m_Player.GetSector() + 1);
    }
    private void MovePlayerLeft()
    {
        Constants.g_iDirection dir = Constants.g_iDirection.WEST;
        if (CharacterCanMove(dir, m_Player.GetSector()))
            m_Player.SetSector(m_Player.GetSector() -1);
    }
    
    
    private bool CharacterCanMove(Constants.g_iDirection direction, int currentSector)
    {
        bool move = true;

        if (direction == Constants.g_iDirection.NORTH)
        {
            if (SectorToRow(currentSector, m_MapRows) < 1)
            { 
                move = false;
                MessageBox.Show("North, \ntop of map, \nmove=false, \nSectorToRow=" + SectorToRow(currentSector, m_MapRows));
            }
            else if (PlayerBlocked(currentSector, direction))
            { 
                move = false;
                MessageBox.Show("North-elseif-playerblocked=true");
            }
        }
        if (direction == Constants.g_iDirection.SOUTH)
        {
            if (SectorToRow(currentSector, m_MapRows) >= (m_MapRows - 1))
            {
                move = false;
                MessageBox.Show("South, \nBottom of map, \nmove=false, \nSectorToRow=" + SectorToRow(currentSector, m_MapRows));
            }
            else if (PlayerBlocked(currentSector, direction))
            {
                move = false;
            }
        }
        if (direction == Constants.g_iDirection.EAST)
        {
            if (SectorToCols(currentSector, m_MapCols) >= (m_MapCols - 1))
            { move = false;
            MessageBox.Show("East, \nRight of map, \nmove=false, \nSectorToCol=" + SectorToCols(currentSector, m_MapCols));
            }
            else if (PlayerBlocked(currentSector, direction))
            { move = false; }
        }
        if (direction == Constants.g_iDirection.WEST)
        {
            if (SectorToCols(currentSector, m_MapCols) <= 0) 
            { move = false;
            MessageBox.Show("West, \nLeft of map, \nmove=false, \nSectorToCol=" + SectorToCols(currentSector, m_MapCols));
            }
            else if (PlayerBlocked(currentSector, direction)) 
            { move = false; }
        }

        return move;
    }
    
    private bool PlayerBlocked(int iPlayerSector, Constants.g_iDirection direction)
    {
        bool blocked = false;
        int iSector = iPlayerSector;

        if (direction == Constants.g_iDirection.NORTH)
        {
            iSector = iSector - m_MapCols;
        }
        else if (direction == Constants.g_iDirection.SOUTH)
        {
            iSector = iSector + m_MapCols;
        }
        else if (direction == Constants.g_iDirection.EAST)
        {
            iSector = iSector + 1;
        }
        else if (direction == Constants.g_iDirection.WEST)
        {
            iSector = iSector -1;
        }

        int y = SectorToRow(iSector, m_MapRows);
        int x = SectorToCols(iSector, m_MapCols);
        
        int item = m_MapSectors[y, x];

        Constants.g_iMapTypeEnum mapType = (Constants.g_iMapTypeEnum)m_MapTypeID;
        /*
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


