using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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


        int tileSetID;
        int tileSetID2;
        short[][] g_mSectors;
        Matrix4 m_moveMatrix = new Matrix4();
        private const float moveAmount = 0.1f;


        /// <summary>Creates a 800x600 window with the specified title.</summary>
        public Game()
            : base(800, 600, GraphicsMode.Default, "JBookman Conversion")
        {
            VSync = VSyncMode.On;
        }

        /// <summary>Load resources here.</summary>
        /// <param name="e">Not used.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            
            //GL.ClearColor(0.1f, 0.2f, 0.5f, 0.0f);
            GL.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);
            GL.Enable(EnableCap.DepthTest);

            //load tileset into texture.
            tileSetID = LoadTexture("Tilesets\\tileset1.png");
            tileSetID2 = LoadTexture("Tilesets\\tileset2.png");

            g_mSectors = new short[5][];
            g_mSectors[0] = new short[] { 2, 2, 1, 1, 1, 1, 1, 1, 2, 2 };
            g_mSectors[1] = new short[] { 2, 0, 0, 0, 0, 0, 0, 0, 0, 2 };
            g_mSectors[2] = new short[] { 0, 0, 0, 0, 0, 3, 0, 0, 0, 0 };
            g_mSectors[3] = new short[] { 2, 0, 0, 0, 0, 0, 0, 0, 0, 2 };
            g_mSectors[4] = new short[] { 2, 2, 1, 1, 1, 1, 1, 1, 2, 2 };

            m_moveMatrix = Matrix4.Identity;
            m_moveMatrix = Matrix4.CreateTranslation(0.0f, 0.0f, 0.0f);


            //CENGINE(char* pstrMap, int iMapType, int iSector)
            m_sFirstMap = "FirstTown.map";
            m_iFirstMapType = (int) Constants.g_iLocationEnum.FIRSTTOWN;
            m_iStartSector = 1485;
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

            Matrix4 projection = Matrix4.CreateOrthographicOffCenter(0, 1024 / 56, -768 / 56, 0, -50.0f, 50.0f);
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

           if (Keyboard[Key.Escape])
                Exit();

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
           }

        //end of OnUpdateFrame
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
            GL.Translate(0.0f, -0.0f, -10.0f);
            // GL.Rotate(180, Vector3.UnitY);

            drawAxis();
            GL.Enable(EnableCap.Texture2D);
            DrawTiles();
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

                    /* OLD CODE FROM TUTOTORIAL
                     * 
                     * Handled m_sectors[], one dimensional array
                     * on old file format (mapID,data->)
                     * 
                     * 
                    
                    //get map type first
                    m_byMapType = binReader.ReadByte();
                    //read sectors
                    for (int sectorCount = 0; sectorCount < Constants.MAPSECTORCOUNT; sectorCount++)
                    {
                        short currentSector = 0;
                        currentSector = binReader.ReadInt16();
                        m_Sectors[sectorCount] = currentSector;
                        
                        //convert m_Sectors to bytes or shorts rather than chars?

                        // change m_Sectors to a 2d array. Change this function to read in
                        // a specific amount of x per columm before resetring x but incrementing y  


                    }
                    */

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

        
        ////////////////////////////////
        ////////////////////////////////
        // 
        //     LOAD TEXTURE
        //
        //////////////////////////////// 
        ////////////////////////////////
        


        private int LoadTexture(string fileName)
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
        /* old draw tiles for g_mSectors[y][x]
        private void DrawTiles()
        {
            int tile;
            for (int y = 0; y < 5; y++) //row loop
            {
                for (int x = 0; x < 10; x++) //columns
                {
                    //get the map tile value
                    tile = g_mSectors[y][x];

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
                    GL.BindTexture(TextureTarget.Texture2D, tileSetID); //set texture

                   


                    GL.Begin(BeginMode.Quads);
                    //quad1
                    //bottomleft
                    GL.TexCoord2(s1, t2);
                    GL.Vertex3((float)x, -((float)y) - 1.0f, 0.0f);
                    //top left
                    GL.TexCoord2(s1, t1);
                    GL.Vertex3((float)x, -(float)y, 0.0f);
                    //top right
                    GL.TexCoord2(s2, t1);
                    GL.Vertex3((float)x + 1.0f, -(float)y, 0.0f);
                    //bottom right
                    GL.TexCoord2(s2, t2);
                    GL.Vertex3((float)x + 1.0f, -(float)y - 1.0f, 0.0f);


                    GL.End();


                }



            }

        //end of drawtiles
        }
        */
        //draw for m_Mapsectors[y,x]
        private void DrawTiles()
        {
            int tile;
            GL.BindTexture(TextureTarget.Texture2D, tileSetID); //set texture
            
            for (int y = 0; y < m_MapRows; y++) //row loop
            {
                for (int x = 0; x < m_MapCols; x++) //columns
                {
                    //get the map tile value
                    tile = m_MapSectors[y,x];

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
                    

                    GL.Begin(BeginMode.Quads);
                    //quad1
                    //bottomleft
                    GL.TexCoord2(s1, t2);
                    GL.Vertex3((float)x, -((float)y) - 1.0f, 0.0f);
                    //top left
                    GL.TexCoord2(s1, t1);
                    GL.Vertex3((float)x, -(float)y, 0.0f);
                    //top right
                    GL.TexCoord2(s2, t1);
                    GL.Vertex3((float)x + 1.0f, -(float)y, 0.0f);
                    //bottom right
                    GL.TexCoord2(s2, t2);
                    GL.Vertex3((float)x + 1.0f, -(float)y - 1.0f, 0.0f);


                    GL.End();
                }
            }
            //end of drawtiles
        }
    //end of Game class   
    }
//end-namespace
}


