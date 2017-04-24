using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

using JBookman_Conversion;



namespace JBookman_Mapper
{
    //DoubleBufferPanel to get around WinForms only applying doublebuffering to forms and not child controls
    public class DoubleBufferPanel : Panel
    {
        public DoubleBufferPanel()
        {
            // Set the value of the double-buffering style bits to true.
            /*this.SetStyle(ControlStyles.DoubleBuffer |
            ControlStyles.UserPaint |
            ControlStyles.AllPaintingInWmPaint,
            true);
            
            this.UpdateStyles();*/
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
        }
    }
       
    
    class Mapper_Program : Form
    {
        //Game content variables
        //Map
        Map currentMap = new Map();
               
       
        string map_FileName = null;
        string tileset_FileName = null;
        string g_sCurrentMapName;

        Bitmap currentTileSet;
        Graphics panelGraphics;
        Rectangle[] map_RectArray;
        Rectangle[] tileset_RectArray;
        bool bMapLoaded = false;
        bool bTileSetLoaded = false;
        Image[] tileImageArray;

        ushort currentlySelectedMapSector = 0;
        ushort currentlySelectedTile = 0;
        bool placeImpassable = false;
        UInt16 currentAngle = 0;

        //form items
        Label lblCurrentMapFileName;
        Label lblCurrentMapID;
        Label lblCurrentMapCols;
        Label lblCurrentMapRows;
        Label lblCurrentTileset;

        TextBox txtTileAngle;

        MainMenu mnuFileMenu;

        DoubleBufferPanel outer_mapPanel;
        DoubleBufferPanel mapPanel;
        Panel tilesetPanel;
        Panel tile_selected_panel;
        Graphics tilesetPanelGraphics;
        Graphics tile_selected_Graphics;

        ContextMenu panelContextMenu;

        Button btnEditContainer;
        Button btnEditNPCs;

        RadioButton radioPassable;
        RadioButton radioImpassable;

        //test vars
        Point contextPoint;


        public Mapper_Program()
        {
            
            //set up CMap newMap
            SetUp_currentMap();

            this.Text = "JBookman Conversion Mapper";
            Size formsize = new Size(1024, 768);
            this.Size = formsize;
            this.Location = new Point(100, 100);
            this.WindowState = FormWindowState.Maximized;
            this.DoubleBuffered = true;
            
            this.BackColor = Color.Gray;

            AddMenuAndItems();

            lblCurrentMapFileName = new Label();
            lblCurrentMapFileName.Text = "test1";
            lblCurrentMapFileName.Size = new Size(500, 30);
            lblCurrentMapFileName.ForeColor = Color.White;
            lblCurrentMapFileName.Location = new Point(10, 10);
            this.Controls.Add(lblCurrentMapFileName);

            

            lblCurrentTileset = new Label();
            lblCurrentTileset.Text = "";
            lblCurrentTileset.Size = new Size(400, 30);
            lblCurrentTileset.Location = new Point(10, 40);

            this.Controls.Add(lblCurrentTileset);


            //Context menu setup
            panelContextMenu = new ContextMenu();
            panelContextMenu.Popup += new EventHandler(panelContextMenu_Popup);
            

            btnEditContainer = new Button();
            btnEditContainer.Text = "Edit Containers";
            btnEditContainer.Size = new Size(100, 50);
            btnEditContainer.Location = new Point(25, 600);
            btnEditContainer.BackColor = Color.GhostWhite;
            btnEditContainer.Click += new EventHandler(openEditContainerDialog);

            this.Controls.Add(btnEditContainer);

            btnEditNPCs = new Button();
            btnEditNPCs.Text = "Edit NPCs";
            btnEditNPCs.Size = new Size(100, 50);
            btnEditNPCs.Location = new Point(25, 700);
            btnEditNPCs.BackColor = Color.GhostWhite;
            this.Controls.Add(btnEditNPCs);

            GroupBox gbPassable = new GroupBox();
            gbPassable.Location = new Point(313, 250);
            gbPassable.Size = new Size(100, 70);
            gbPassable.Text = "Is Passable?";
            

            radioPassable = new RadioButton();
            radioImpassable = new RadioButton();
            radioPassable.Text = "Passable";
            radioImpassable.Text = "Impassable";
            radioPassable.Location = new Point(10, 20);
            radioImpassable.Location = new Point(10, 40);
            radioImpassable.Checked = false;
            radioPassable.Checked = true;
            radioPassable.Width = 80;
            radioImpassable.Width = 80;
            radioImpassable.CheckedChanged += new EventHandler(RadioPassableEvent_CheckedChanged);
            radioPassable.CheckedChanged += new EventHandler(RadioPassableEvent_CheckedChanged);
            gbPassable.Controls.AddRange(new Control[]{radioPassable,radioImpassable});

            this.Controls.Add(gbPassable);

            GroupBox tileSetting = new GroupBox();
            tileSetting.Location = new Point(313, 180);
            tileSetting.Size = new Size(100, 50);
            tileSetting.Text = "Tile Settings";
            //Handles the angles of the tile.
            Label lblAngle = new Label();
            lblAngle.Text = "Angle:";
            lblAngle.Location = new Point(10, 20);
            lblAngle.Width = 40;
            txtTileAngle = new TextBox();
            txtTileAngle.Text = "0";
            txtTileAngle.Location = new Point(50, 20);
            txtTileAngle.Width = 40;
            txtTileAngle.TextChanged += new EventHandler(textTileAngle_Changed);

            tileSetting.Controls.AddRange(new Control[]{txtTileAngle, lblAngle});

            this.Controls.Add(tileSetting);
        }

        void SetUp_currentMap()
        {
            currentMap.MapTypeId = 0;
            currentMap.MapCols = 0;
            currentMap.MapRows = 0;

            

        }

        //RIGHT CLICK CONTEXT MENU GUBBINS

        void panelContextMenu_Popup(object sender, EventArgs e)
        {
            // Clear all previously added MenuItems.
            panelContextMenu.MenuItems.Clear();
            panelContextMenu.Name = "Popup Menu";
           //get point details from sender object (map panel) then use PointToClient to
            //convert to client object coordinates rather than global form coords
            // ie: in relation to 0,0 of mapPanel not 0,0 of form.
            ContextMenu cmu1 = sender as ContextMenu;
          
            Point screenPoint = Cursor.Position;
            Point newp1 = cmu1.SourceControl.PointToClient(screenPoint);
           // MessageBox.Show("SourceControl\nX = " + newp1.X + "\nY= " + newp1.Y);
            contextPoint = newp1;

            //throw new NotImplementedException();
            MenuItem cMenuItem1 = new MenuItem("TileInfo", new EventHandler(TileInfo_OnClick));
            MenuItem cMenuItem2 = new MenuItem("Impassible", new EventHandler(Context_Impassible_OnClick));


            ushort sector = getSectorDetails_FromPoint(contextPoint, map_RectArray, (object)mapPanel);

            int x1 = sector % currentMap.MapCols;
            int y1 = (int)((sector / currentMap.MapRows) + 0.001f);

            if (currentMap.m_MapSectors[y1, x1].Impassable)
            { cMenuItem2.Checked = true; }
            else cMenuItem2.Checked = false;

            panelContextMenu.MenuItems.Add(cMenuItem1);
            panelContextMenu.MenuItems.Add(cMenuItem2);


       }

        private void TileInfo_OnClick(object sender, EventArgs args)
        {
            //MessageBox.Show("TileInfo_OnClick");
            ushort sector = getSectorDetails_FromPoint(contextPoint, map_RectArray, (object) mapPanel);

            var mapSec = getMapSectorDetails_FromPoint(contextPoint, map_RectArray, (object)mapPanel);

            MessageBox.Show(@"TileInfo_OnClick \n ContextPoint X: "+contextPoint.X +" Y: "+contextPoint.Y +"\n Sector: "+sector + "\n Rotation: "+ mapSec.rotationAngle );
        }
        private void Context_Impassible_OnClick(object sender, EventArgs args)
        {
            ushort sector = getSectorDetails_FromPoint(contextPoint, map_RectArray, (object)mapPanel);

            int x1 = sector % currentMap.MapCols;
            int y1 = (int)((sector / currentMap.MapRows) + 0.001f);

            if (currentMap.m_MapSectors[y1, x1].Impassable)
            {
                currentMap.m_MapSectors[y1, x1].Impassable = false;
            }
            else
            {
                currentMap.m_MapSectors[y1, x1].Impassable = true;
            }

            //as it's changed. Redraw the map.
            mapPanel.Invalidate();
        }


        public void AddMenuAndItems()
        {

            mnuFileMenu = new MainMenu();
            //File Menu
            MenuItem menuFile = new MenuItem("&File");
            menuFile.MenuItems.Add(new MenuItem("Create New Map", new EventHandler(this.CreateNewMap_MenuEvent)));
            menuFile.MenuItems.Add(new MenuItem("Open Map", new EventHandler(this.OpenMap_MenuEvent)));
            menuFile.MenuItems.Add(new MenuItem("-"));
            menuFile.MenuItems.Add(new MenuItem("Save Map", new EventHandler(this.SaveMap_MenuEvent)));
            menuFile.MenuItems.Add(new MenuItem("-"));
            menuFile.MenuItems.Add(new MenuItem("Exit", new EventHandler(this.Exit_MenuEvent)));

            //TilesetMenu
            MenuItem menuTilesets = new MenuItem("&Tilesets");
            menuTilesets.MenuItems.Add(new MenuItem("Load Tileset", new EventHandler(this.LoadTileset_MenuEvent)));
            menuTilesets.MenuItems.Add(new MenuItem("-"));
            
            //add menus
            mnuFileMenu.MenuItems.Add(menuFile);
            mnuFileMenu.MenuItems.Add(menuTilesets);
            this.Menu = mnuFileMenu;
        }


        private void Exit_MenuEvent(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Do you want to exit: Y/N?", "Exit?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }
      
        ///////////////////////////////////////////////
        // Radio Button CheckChanged Handling Method
        //////////////////////////////////////////////
        private void RadioPassableEvent_CheckedChanged(object sender, EventArgs checkEventArgs)
        {
            if (sender == radioPassable)
            {
                if (radioPassable.Checked == true)
                placeImpassable = false;
            }
            else if (sender == radioImpassable)
            {
                if(radioImpassable.Checked == true)
                placeImpassable = true;
            }
            
        }


      ///////////////////////////////////////////////////
      // Map handling methods
      //////////////////////////////////////////////////
      
        private void CreateNewMap_MenuEvent(object sender, EventArgs e)
        {
           // MessageBox.Show("File->Create New Map");
            //get size of map. Cols + Rows.
            CreateMapInputForm newMapInput = new CreateMapInputForm();
            if (newMapInput.ShowDialog() == DialogResult.OK)
            {
                currentMap.MapTypeId = newMapInput.MapID;
                currentMap.MapCols = newMapInput.MapCols;
                currentMap.MapRows = newMapInput.MapRows;
                g_sCurrentMapName = newMapInput.MapName;
            }

            MessageBox.Show("Map values: ID= "+currentMap.MapTypeId+" Cols= "+currentMap.MapCols+" Rows= "+currentMap.MapRows+ " MapName: "+g_sCurrentMapName);
            
            //currentMap.m_MapSectors = new ushort[currentMap.m_MapRows][];
            currentMap.m_MapSectors = new MapSector[currentMap.MapRows,currentMap.MapCols];

            //Create blank map
            for (short y = 0; y < currentMap.MapRows; y++)
            {
                //currentMap.m_MapSectors[y] = new ushort[currentMap.MapCols];
                for(short x = 0; x< currentMap.MapCols; x++)
                {
                    //currentMap.m_MapSectors[y][x] = 0;
                    //currentMap.m_MapSectors[y,x].Set_Tileset_Number(0);
                    currentMap.m_MapSectors[y, x] = new MapSector(0);
                }
            }

            //test output display
            string output = null;
            int count = 0;
            for (short cy = 0; cy < currentMap.MapRows; cy++)
            {
                
                for (short cx = 0; cx < currentMap.MapCols; cx++)
                {
                    //output += currentMap.m_MapSectors[c1][c2];
                    output += currentMap.m_MapSectors[cy, cx].TileNumberId;
                    count++;
                }
            }

            MessageBox.Show("currentMap.m_MapSectors: " + output+"\n\n Count:  "+count);
            bMapLoaded = true;
           
            DrawMapOnMapPanel();
            mapPanel.Invalidate();
        }
       
        public void OpenMap_MenuEvent(object sender, EventArgs e)
        {
          
            //MessageBox.Show("File->Open Map");
            OpenFileDialog newFileDialog = new OpenFileDialog();
            newFileDialog.Filter = "Map Files (*.map)|*.map|All Files (*.*)|*.*";

            if (newFileDialog.ShowDialog() == DialogResult.OK)
            {
                map_FileName = newFileDialog.FileName;
                lblCurrentMapFileName.Text = map_FileName;
                ReadMap(map_FileName);

             //   map_DoorFileName = string.Concat(map_FileName, ".dor");
             //   map_ContainersFileName = string.Concat(map_FileName, ".con");
            }

            DrawMapOnMapPanel();
            mapPanel.Invalidate();
        
        }

        private void SaveMap_MenuEvent(object sender, EventArgs e)
        {
            if (bMapLoaded)
            {
                FolderBrowserDialog fbDialogue = new FolderBrowserDialog();
                fbDialogue.RootFolder = Environment.SpecialFolder.MyDocuments;
                string folderpath;

                if (fbDialogue.ShowDialog() == DialogResult.OK)
                {
                    folderpath = fbDialogue.SelectedPath;
                    WriteMapData(folderpath);
                }
            }
        }

        
        private void WriteMapData(string fPath)
        {
            
            Map.SaveMapFile(fPath + "\\" + g_sCurrentMapName + ".map", currentMap);
            
            
            /*
               if (currentMap.MapCols != 0 && currentMap.m_MapRows != 0)
               {

                   try
                   {

                       binWriter = new BinaryWriter(File.Create(fPath+"\\"+g_sCurrentMapName+".map"));
                    //   ushort test = 99;
                     //  binWriter.Write(test);
                      // 
                       binWriter.Write(currentMap.MapTypeId);
                       binWriter.Write(currentMap.m_MapRows);
                       binWriter.Write(currentMap.MapCols);
                   
                       //write map to file
                       for (short y = 0; y < currentMap.m_MapRows; y++)
                       {
                           for (short x = 0; x < currentMap.MapCols; x++)
                           {
                               //binWriter.Write(currentMap.m_MapSectors[y][x]);
                               binWriter.Write(currentMap.m_MapSectors[y,x].Get_Tileset_Number());
                           }
                       }

                       binWriter.Close();
                    
                   }
                   catch (Exception fExp)
                   {
                       MessageBox.Show(fExp.ToString());
                   }
               }
               else
               {
                 //do nothing

               }*/

        }

        private void ReadMap(string fileName)
        {
           //if a map has already been loaded, clear it in prep for loading
            if (currentMap != null)
            {
                currentMap = null;
            }
            
            currentMap = Map.ReadMapFile(fileName);

            if (currentMap != null)
            {
                bMapLoaded = true;
            }
            else bMapLoaded = false;

        }
        ///////////////////////////////////////////////
        // TILESET HANDLING
        //////////////////////////////////////////
        private void LoadTileset_MenuEvent(object sender, EventArgs e)
        {
            //MessageBox.Show("Tileset->Load Tileset");
            OpenFileDialog newFileDialog = new OpenFileDialog();
            newFileDialog.Title = "Load a tileset image";
            newFileDialog.Filter = "Images (*.bmp;*.png;*.jpg)|*.bmp;*.png;*.jpg|Bitmap (*.bmp)|*.bmp|PNG (*.png)|*.png|JPEG (*.jpg)|*.jpg|All Files (*.*)|*.*";
            if (newFileDialog.ShowDialog() == DialogResult.OK)
            {
                tileset_FileName = newFileDialog.FileName;
                lblCurrentTileset.Text = newFileDialog.SafeFileName;
                currentTileSet = new Bitmap(tileset_FileName);

                
                ///
              //  mapPanel.BackgroundImage = currentTileSet;
                if (!bMapLoaded)
                {
                    MessageBox.Show("No map file loaded, can't apply tileset\n Load/Create a map first");
                }
                else
                {
                    //MessageBox.Show("tileset menu else clause");
                    bTileSetLoaded = true;
                    Setup_tileDataArray();
                    Setup_TilePanel();
                    tilesetPanel.Invalidate();
                    mapPanel.Invalidate();
                }
            }



        }

      
        private void Setup_tileDataArray()
        {
            int tileSetCols = currentTileSet.Width / 32;
            int tileSetRows = currentTileSet.Height / 32;
            BitmapData[] tileDataArray = new BitmapData[tileSetCols * tileSetRows];
            tileImageArray = new Image[tileSetCols * tileSetRows];
            int count = 0;

            for (int y = 0; y < tileSetRows; y++)
            {
                for (int x = 0; x < tileSetCols; x++)
                {

                    tileDataArray[count] = currentTileSet.LockBits(new Rectangle(x * 32, y*32, 32, 32), ImageLockMode.ReadOnly, currentTileSet.PixelFormat);
                    

                   // int currentMapSectorValue = currentMap.m_MapSectors[y, x];
                    Image tileImage = (Image)new Bitmap(tileDataArray[count].Height,
                                               tileDataArray[count].Width,
                                               tileDataArray[count].Stride,
                                               tileDataArray[count].PixelFormat,
                                               tileDataArray[count].Scan0);
                    tileImageArray[count] = tileImage;
                    currentTileSet.UnlockBits(tileDataArray[count]);
                    count++;
                }
            }
            
           
        }

        private void Setup_TilePanel()
        {
            tilesetPanel = new Panel();
            tilesetPanel.Size = new Size(256, 256);
            tilesetPanel.Location = new Point(25, 100);
            tilesetPanel.BackColor = Color.White;
            tilesetPanel.MouseClick += new MouseEventHandler(tileSetPanel_Click);
            tilesetPanel.Paint += new PaintEventHandler(Draw_TilePanel);

            this.Controls.Add(tilesetPanel);
            tilesetPanelGraphics = tilesetPanel.CreateGraphics();
            int numberOfSectors = 64;
            int panelCols = currentTileSet.Width / 32;
            int panelRows = currentTileSet.Height / 32;


            tileset_RectArray = new Rectangle[numberOfSectors];

            int rectCount = 0;
            for (short y = 0; y < panelCols; y++)
            {
                for (short x = 0; x < panelRows; x++)
                {
                    Rectangle newRect = new Rectangle(x * 32, y * 32, 32, 32);

                    tileset_RectArray[rectCount] = newRect;
                    rectCount++;
                }

            }

            //setup tile_selected_panel

            tile_selected_panel = new Panel();
            tile_selected_panel.Size = new Size(48, 48);
            tile_selected_panel.Location = new Point(313, 100);
            tile_selected_panel.BackColor = Color.White;
            tile_selected_panel.Paint += new PaintEventHandler(tile_selected_panel_Paint);

            this.Controls.Add(tile_selected_panel);

        }

        private void Draw_TilePanel(object sender, PaintEventArgs e)
        {
            

            int panelCols = currentTileSet.Width / 32;
            int panelRows = currentTileSet.Height / 32;


            int count = 0;
            for (int y = 0; y < panelRows; y++)
            {
                for (int x = 0; x < panelCols; x++)
                {
                    //Rectangle newRect = new Rectangle(x * 32, y * 32, 32, 32);
                    //  MessageBox.Show("onPaint, both bool =true, map sector value= "+currentMapSectorValue);
                    tilesetPanelGraphics.DrawImage(tileImageArray[count], tileset_RectArray[count]);
                    count++;
                }

            }
        }

        private void tile_selected_panel_Paint(object sender, PaintEventArgs e)
        {
            Graphics tilePainter = e.Graphics;
            //update with currentlyselectedTile
            tilePainter.DrawImage(tileImageArray[currentlySelectedTile],0,0,48,48);
            tilePainter.Dispose();
        }


        private void tileSetPanel_Click(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                currentlySelectedTile = getSectorDetails_FromMouseEvent(e, tileset_RectArray, sender);
              //  MessageBox.Show("CurrentlySelectedTile: "+currentlySelectedTile);
                tile_selected_panel.Invalidate();
            }
        }

        private void textTileAngle_Changed(object sender, EventArgs e)
        {
            try
            {
                UInt16 angle = UInt16.Parse(txtTileAngle.Text);
                if (angle % 90 == 0 && angle < 360) //If a 90 degree angle and less than 360.
                {
                    currentAngle = angle;
                }
                else
                {
                    //throw new FormatException("Not 0, 90, 180, 270 or 360");
                }
            }
            catch (FormatException fe)
            {
                MessageBox.Show(fe.Message +" "+ "Not a valid number");
            }
        }

        //////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////
        //
        //  Drawing on mapPanel + mapPanel Events
        //
        ////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////
        


        private void DrawMapOnMapPanel()
        {
            //set up panels!
            
            //resize the map
            /*int scrollbarHeight = SystemInformation.HorizontalScrollBarHeight;
            int scrollbarWidth = SystemInformation.VerticalScrollBarWidth;
            mapPanel.HorizontalScroll.Maximum = (currentMap.m_MapRows * 32) + scrollbarWidth;
            mapPanel.VerticalScroll.Maximum = (currentMap.m_MapRows * 32) + scrollbarHeight;
            Size mapPanelClientSize = new Size((currentMap.m_MapRows * 32) + scrollbarWidth, (currentMap.m_MapRows * 32) + scrollbarHeight);
            */
            Size mapPanelSize = new Size((currentMap.MapCols * 32), (currentMap.MapRows * 32));
            MessageBox.Show("Draw on map. MapPanelClientSize width= " + mapPanelSize.Width + " height:" + mapPanelSize.Height);

            outer_mapPanel = new DoubleBufferPanel();
            outer_mapPanel.Location = new Point((this.Size.Width / 2)-100, 50);
            outer_mapPanel.Size = new Size(700, 800);
            outer_mapPanel.BackColor = Color.White;
            outer_mapPanel.AutoScroll = true;
            outer_mapPanel.AutoScrollMinSize = mapPanelSize;
            //outer_mapPanel.Paint += new PaintEventHandler(MapPanel_OnPaint);


            mapPanel = new DoubleBufferPanel();
            mapPanel.Location = new Point(0, 0);
            mapPanel.MinimumSize = mapPanelSize;
            mapPanel.BackColor = Color.White;
            mapPanel.ContextMenu = panelContextMenu;

            mapPanel.MouseClick += new MouseEventHandler(MapPanel_OnClick);
           
            mapPanel.Paint += new PaintEventHandler(MapPanel_OnPaint);
            panelGraphics = mapPanel.CreateGraphics();

            outer_mapPanel.Controls.Add(mapPanel);
            this.Controls.Add(outer_mapPanel);


            //draw code!
            int numberOfSectors = currentMap.MapRows * currentMap.MapCols;

            map_RectArray = new Rectangle[numberOfSectors];

            int rectCount = 0;
            for (short y = 0; y < currentMap.MapRows; y++)
            {
                for (short x = 0; x < currentMap.MapCols; x++)
                {
                    Rectangle newRect = new Rectangle(x * 32, y * 32, 32, 32);

                    map_RectArray[rectCount] = newRect;
                    rectCount++;
                }

            }
            panelGraphics.DrawRectangles(new Pen(Color.Blue), map_RectArray);
           
        }



        public void MapPanel_OnClick(object sender, MouseEventArgs e)
        {
            if (sender == mapPanel && e.Button == MouseButtons.Right)
            {
                //MessageBox.Show("X: " + e.X + " Y: " + e.Y);

                //get current sector
              // currentlySelectedMapSector= getSectorDetails_FromMouseEvent(e, map_RectArray, sender);
              //  MessageBox.Show("Right Mouse Click \n X: " + e.X + " Y: " + e.Y +" \nCurrentlySelectedMapSector: "+currentlySelectedMapSector);
              //  panelContextMenu.Show(this.mapPanel, e.Location);
             }
            else if (sender == mapPanel && e.Button == MouseButtons.Left)
            {
               currentlySelectedMapSector = getSectorDetails_FromMouseEvent(e, map_RectArray, sender);
             //   MessageBox.Show("CurrentlySelectedMapSector: " + currentlySelectedMapSector);

                int x1 = currentlySelectedMapSector % currentMap.MapCols;
                int y1 = (int)((currentlySelectedMapSector / currentMap.MapCols) + 0.001f);
               // MessageBox.Show("x1 (col)= " + x1 + " y1(row)= " + y1);
               // MessageBox.Show("Tile value = " + currentMap.m_MapSectors[y1, x1]);


                //update maptile if different to currently selected tile
                // or if redrawing with a different placeImpassable flag or a different angle
                if (currentMap.m_MapSectors[y1, x1].TileNumberId != currentlySelectedTile 
                    || currentMap.m_MapSectors[y1,x1].Impassable != placeImpassable
                    || currentMap.m_MapSectors[y1,x1].rotationAngle != currentAngle
                    )
                {
                    currentMap.m_MapSectors[y1, x1].TileNumberId = currentlySelectedTile;
                    
                    //set passable/impassibe flag from global bool.
                    currentMap.m_MapSectors[y1, x1].Impassable = placeImpassable;
                    
                    //Set the angle.
                    currentMap.m_MapSectors[y1, x1].rotationAngle = currentAngle;

                    //force redraw
                    mapPanel.Invalidate();

                }

            }
           
        }
         


        // getSector Details (FROM MOUSE EVENT)
        private ushort getSectorDetails_FromMouseEvent(MouseEventArgs e, Rectangle[] rectArray,object senderObj)
        {
            if (senderObj == mapPanel)
            {
                if (bMapLoaded)
                {
                   int i =0;
                    for (i = 0; i < rectArray.Length; i++)
                    {
                        if (e.X >= rectArray[i].X && e.X <= (rectArray[i].X + 32))
                        {
                            if (e.Y >= rectArray[i].Y && e.Y <= (rectArray[i].Y + 32))
                            {
                               // MessageBox.Show("Current map sector= " + i);
                              /*  int x1 = i % currentMap.MapCols;
                                int y1 = (int)((i / currentMap.MapCols) + 0.001f);
                                MessageBox.Show("x1 (col)= " + x1 + " y1(row)= " + y1);
                                MessageBox.Show("Tile value = " + currentMap.m_MapSectors[y1, x1]);
                                mapPanel.Invalidate();*/
                                break;

                            }
                        }
                    }
                    return (ushort)i;
                }
            }
            if (senderObj == tilesetPanel)
            {
               // MessageBox.Show("tilesetpanel event");
                if (bTileSetLoaded)
                {
                    int i = 0;
                    for (i = 0; i < rectArray.Length; i++)
                    {
                        if (e.X > rectArray[i].X && e.X < (rectArray[i].X + 32))
                        {
                            if (e.Y > rectArray[i].Y && e.Y < (rectArray[i].Y + 32))
                            {
                                //MessageBox.Show("Current tile= " + i);
                              /*  int x1 = i % currentMap.MapCols;
                                int y1 = (int)((i / currentMap.MapCols) + 0.001f);
                                MessageBox.Show("x1 (col)= " + x1 + " y1(row)= " + y1);
                                MessageBox.Show("Tile value = " + currentMap.m_MapSectors[y1, x1]);
                                mapPanel.Invalidate();*/
                                break;

                            }
                        }
                    }
                    return (ushort)i;
                }
            }
            return 0;
        }

        //GET SECTOR DETAILS (FROM POINT DATA).
        private ushort getSectorDetails_FromPoint(Point location, Rectangle[] rectArray, object senderObj)
        {
            if (senderObj == mapPanel)
            {
                if (bMapLoaded)
                {
                    int i = 0;
                    for (i = 0; i < rectArray.Length; i++)
                    {
                        if (location.X >= rectArray[i].X && location.X <= (rectArray[i].X + 32))
                        {
                            if (location.Y >= rectArray[i].Y && location.Y <= (rectArray[i].Y + 32))
                            {
                                // MessageBox.Show("Current map sector= " + i);
                                /*  int x1 = i % currentMap.MapCols;
                                  int y1 = (int)((i / currentMap.MapCols) + 0.001f);
                                  MessageBox.Show("x1 (col)= " + x1 + " y1(row)= " + y1);
                                  MessageBox.Show("Tile value = " + currentMap.m_MapSectors[y1, x1]);
                                  mapPanel.Invalidate();*/
                                break;

                            }
                        }
                    }
                    return (ushort)i;
                }
            }
            if (senderObj == tilesetPanel)
            {
                // MessageBox.Show("tilesetpanel event");
                if (bTileSetLoaded)
                {
                    int i = 0;
                    for (i = 0; i < rectArray.Length; i++)
                    {
                        if (location.X > rectArray[i].X && location.X < (rectArray[i].X + 32))
                        {
                            if (location.Y > rectArray[i].Y && location.Y < (rectArray[i].Y + 32))
                            {
                                //MessageBox.Show("Current tile= " + i);
                                /*  int x1 = i % currentMap.MapCols;
                                  int y1 = (int)((i / currentMap.MapCols) + 0.001f);
                                  MessageBox.Show("x1 (col)= " + x1 + " y1(row)= " + y1);
                                  MessageBox.Show("Tile value = " + currentMap.m_MapSectors[y1, x1]);
                                  mapPanel.Invalidate();*/
                                break;

                            }
                        }
                    }
                    return (ushort)i;
                }
            }
            return 0;
        }

        //GET SECTOR DETAILS (FROM POINT DATA).
        private MapSector getMapSectorDetails_FromPoint(Point location, Rectangle[] rectArray, object senderObj)
        {
            if (senderObj == mapPanel)
            {
                if (bMapLoaded)
                {
                    int i = 0;

                    for (i = 0; i < rectArray.Length; i++)
                    {
                        if (location.X >= rectArray[i].X && location.X <= (rectArray[i].X + 32))
                        {
                            if (location.Y >= rectArray[i].Y && location.Y <= (rectArray[i].Y + 32))
                            {
                                  int x1 = i % currentMap.MapCols;
                                  int y1 = (int)((i / currentMap.MapCols) + 0.001f);
                                return currentMap.m_MapSectors[y1, x1];
                            }
                        }
                    }
                    return null;
                }
            };
            return null;
        }

        private void MapPanel_OnPaint(object sender, PaintEventArgs paintEventArgs)
        {
            //panelGraphics.Clear(Color.White);

            //create overlay image.
            Bitmap overlayImage = new Bitmap(8, 8, PixelFormat.Format32bppArgb);
            Color c1 = Color.FromArgb(80, Color.Silver);
            Color c2 = Color.FromArgb(80, Color.DarkGray);
            int length = 8;
            int halfLength = length /2;

            for (int x=0; x < length; x++)
            {

                for (int y = 0; y < length; y++)
                {
                    if ((x < halfLength && y < halfLength) || (x >= halfLength && y >= halfLength)) 
                overlayImage.SetPixel(x, y, c1);
            else 
                overlayImage.SetPixel(x, y, c2);
                }
            }

            //get the graphics context from the eventArgs
            if (sender.Equals(mapPanel))
            {
                Graphics currGraphics = paintEventArgs.Graphics;

                // Create bitmap for panel
                // Create graphics context for bitmap.
                // Draw to bitmap using graphics context
                // Draw bitmap on panel using panel graphics context.
                Bitmap mapBitmap = new Bitmap(mapPanel.Size.Width, mapPanel.Size.Height, PixelFormat.Format32bppArgb);
                Graphics gBmp = Graphics.FromImage(mapBitmap);
                gBmp.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;


                if (bMapLoaded && !bTileSetLoaded)
                {
                    //panelGraphics.DrawRectangles(new Pen(Color.Blue), map_RectArray);
                    gBmp.DrawRectangles(new Pen(Color.Blue), map_RectArray);
                    //currGraphics.DrawImage(mapBitmap, 0, 0, mapBitmap.Width, mapBitmap.Height);
                    currGraphics.DrawImage(mapBitmap, 0, 0);
                }

                //draw matching images from tileset and map onto mapanel

                //MessageBox.Show("mapPanel_OnPaint: bMapLoaded:"+bMapLoaded+" bTilesetloaded: "+bTileSetLoaded);
                if (bMapLoaded && bTileSetLoaded)
                {
                    int count = 0;
                    for (int y = 0; y < currentMap.MapRows; y++)
                    {
                        for (int x = 0; x < currentMap.MapCols; x++)
                        {
                            int currentMapSectorValue = currentMap.m_MapSectors[y, x].TileNumberId;
                            //  MessageBox.Show("onPaint, both bool =true, map sector value= "+currentMapSectorValue);

                            //panelGraphics.DrawImage(tileImageArray[currentMapSectorValue], map_RectArray[count]);

                            Image currentImage = (Image)tileImageArray[currentMapSectorValue].Clone();

                            switch (currentMap.m_MapSectors[y, x].rotationAngle)
                            {
                                case 90:
                                    currentImage.RotateFlip(RotateFlipType.Rotate90FlipNone);
                                    break;
                                case 180:
                                    currentImage.RotateFlip(RotateFlipType.RotateNoneFlipY);
                                    break;
                                case 270:
                                    currentImage.RotateFlip(RotateFlipType.Rotate270FlipNone);
                                    break;
                            }


                            gBmp.DrawImage(currentImage, map_RectArray[count]);

                            //Draw overlay for passable.
                            if (currentMap.m_MapSectors[y, x].Impassable)
                            {
                                gBmp.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
                                TextureBrush newTextureBrush = new TextureBrush(overlayImage);
                                gBmp.FillRectangle(newTextureBrush, map_RectArray[count]);
                                gBmp.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;

                                
                            }
                            
                            
                            count++;
                        }

                    }
                    //draw bitmap on panel
                    //currGraphics.DrawImage(mapBitmap, 0, 0, mapBitmap.Width, mapBitmap.Height);
                    currGraphics.DrawImage(mapBitmap, 0, 0);

                }
                mapBitmap.Dispose();
                gBmp.Dispose();
            }

        }

        private void openEditContainerDialog(object sender, EventArgs eventarg)
        {
            EditContainerForm editConts = new EditContainerForm(currentMap);
            editConts.Show();
        }
       


        ////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////


        //Application entry point.
        [STAThread]
        static void Main()
        {
            Mapper_Program newForm = new Mapper_Program();
            Application.Run(newForm);
        }


//end class def
    }
//end namespace def
}
