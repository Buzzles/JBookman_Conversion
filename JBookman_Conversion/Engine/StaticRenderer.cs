using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Audio;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace JBookman_Conversion.Engine
{
    public static class StaticRenderer
    {
        internal static void Render(Map g_CurrentMap, int mapTileSet, int m_iPlayerTileSet, Player m_Player, Matrix4 m_moveMatrix)
        {
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

            DrawTiles(g_CurrentMap, mapTileSet, m_Player);

            StaticPlayerRenderer.DrawPlayer(g_CurrentMap, m_Player, m_iPlayerTileSet);

            GL.Disable(EnableCap.Texture2D);
        }

        private static void drawAxis()
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

        private static void DrawTiles(Map g_CurrentMap, int m_iCurrentTileSet, Player m_Player)
        {
            //Calculate visible columns
            int MinVisibleCol, MaxVisibleCol, MinVisibleRow, MaxVisibleRow;
            //int playerMapCol = m_Player.GetSector() % m_MapCols;
            //int playerMapRow = (int)m_Player.GetSector() / m_MapRows;

            //ushort m_MapCols =  g_CurrentMap.m_MapCols;
            //ushort m_MapRows = g_CurrentMap.m_MapRows;
            MapSector[,] m_MapSectors = g_CurrentMap.m_MapSectors;

            int playerMapCol = MapUtils.SectorToCols(m_Player.GetSector(), g_CurrentMap.MapCols);
            int playerMapRow = MapUtils.SectorToRow(m_Player.GetSector(), g_CurrentMap.MapRows);
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
            else if (playerMapCol > ((g_CurrentMap.MapCols - 1) - Constants.NORMALVISIBLEPLAYERCOL)) //right
            {
                MinVisibleCol = g_CurrentMap.MapCols - Constants.VISIBLECOLUMNCOUNT;
                MaxVisibleCol = g_CurrentMap.MapCols - 1;
            }
            //min/max rows
            if (playerMapRow < Constants.NORMALVISIBLEPLAYERROW) //top
            {
                MinVisibleRow = 0;
                MaxVisibleRow = Constants.VISIBLEROWCOUNT;
            }
            else if (playerMapRow > ((g_CurrentMap.MapRows - 1) - Constants.NORMALVISIBLEPLAYERROW)) //bottom
            {
                MinVisibleRow = g_CurrentMap.MapRows - Constants.VISIBLEROWCOUNT;
                MaxVisibleRow = g_CurrentMap.MapRows - 1;
            }

            int tile;

            GL.BindTexture(TextureTarget.Texture2D, m_iCurrentTileSet); //set texture

            int drawRow = 0;

            for (int currRow = MinVisibleRow; currRow <= MaxVisibleRow; currRow++) //row loop (y)
            {
                int drawCol = 0;

                for (int currCol = MinVisibleCol; currCol <= MaxVisibleCol; currCol++) //columns (x)
                {
                    //get the map tile value
                    var currentMapSector = m_MapSectors[currRow, currCol];
                    tile = currentMapSector.TileNumberId;

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

                    // Rotation should be done before trans!
                    //GL.Translate(drawCol, -drawRow, 0);
                    var translateVector = new Vector3(drawCol, -drawRow, 0);
                    GL.Translate(translateVector);

                    //Rotation to be handled here. Need to recenter the draw for this to work before shifting tiles around.
                    if (currentMapSector.rotationAngle != 0)
                    {
                        //GL.PushMatrix();

                        //GL.Rotate(currentMapSector.rotationAngle, 0, 0, 1);
                        var rotateVex = new Vector3(0, 0, 1);
                        GL.Rotate(currentMapSector.rotationAngle, rotateVex);

                        //GL.PopMatrix();
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
    }
}