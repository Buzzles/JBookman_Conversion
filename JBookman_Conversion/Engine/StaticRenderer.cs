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
            GL.Begin(PrimitiveType.Lines);

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
            var drawBoundries = GetDrawBoundries(g_CurrentMap, m_iCurrentTileSet, m_Player);

            int tile;

            GL.BindTexture(TextureTarget.Texture2D, m_iCurrentTileSet); //set texture

            int drawRow = 0;

            // Draw row by row so: outer == rows, inner == cols
            for (int currRow = drawBoundries.MinVisibleRow; currRow <= drawBoundries.MaxVisibleRow; currRow++)
            {
                int drawCol = 0;

                for (int currCol = drawBoundries.MinVisibleCol; currCol <= drawBoundries.MaxVisibleCol; currCol++)
                {
                    //get the map tile value
                    var currentMapSector = g_CurrentMap.m_MapSectors[currRow, currCol];
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

                    //Proper GL way, translate grid, then draw at new 0.0
                    GL.PushMatrix(); //save
                    GL.LoadIdentity();

                    // Rotation should be done before trans!
                    //GL.Translate(drawCol, -drawRow, 0);
                    var translateVector = new Vector3(drawCol, -drawRow, 0);
                    GL.Translate(translateVector);

                    ////Rotation to be handled here. Need to recenter the draw for this to work before shifting tiles around.
                    //if (currentMapSector.rotationAngle != 0)
                    //{
                    //    //GL.PushMatrix();

                    //    //GL.Rotate(currentMapSector.rotationAngle, 0, 0, 1);
                    //    var rotateVex = new Vector3(0, 0, 1);
                    //    GL.Rotate(currentMapSector.rotationAngle, rotateVex);

                    //    //GL.PopMatrix();
                    //}

                    // Rotate the texture matrix
                    if (currentMapSector.rotationAngle != 0)
                    {
                        var rotateVex = new Vector3(0, 0, 1);
                        GL.Rotate(currentMapSector.rotationAngle, rotateVex);

                        //GL.MatrixMode(MatrixMode.Texture);
                        //GL.LoadIdentity();

                        //// Make origin in middle of texture
                        //GL.Translate(0.5, 0.5, 0.0);

                        //var rotateVex = new Vector3(0, 0, 1);
                        //GL.Rotate(currentMapSector.rotationAngle, rotateVex);

                        //// move texture back
                        //GL.Translate(-0.5, -0.5, 0.0);

                        //GL.MatrixMode(MatrixMode.Modelview);
                    }

                    GL.Begin(PrimitiveType.Quads);

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

                    GL.PopMatrix();

                    drawCol++;
                }
                drawRow++;
            }
            //end of drawtiles
        }

        private static DrawBoundries GetDrawBoundries(Map g_CurrentMap, int m_iCurrentTileSet, Player m_Player)
        {
            int _minVisibleCol, _maxVisibleCol, _minVisibleRow, _maxVisibleRow;
        
            int playerMapCol = MapUtils.SectorToCols(m_Player.GetSector(), g_CurrentMap.MapCols);
            int playerMapRow = MapUtils.SectorToRow(m_Player.GetSector(), g_CurrentMap.MapRows);

            _minVisibleCol = playerMapCol - Constants.NORMALVISIBLEPLAYERCOL;
            _maxVisibleCol = playerMapCol + Constants.NORMALVISIBLEPLAYERCOL;

            _minVisibleRow = playerMapRow - Constants.NORMALVISIBLEPLAYERROW;
            _maxVisibleRow = playerMapRow + Constants.NORMALVISIBLEPLAYERROW;
            //min and max cols
            if (playerMapCol < Constants.NORMALVISIBLEPLAYERCOL) //left
            {
                _minVisibleCol = 0;
                _maxVisibleCol = Constants.VISIBLECOLUMNCOUNT;

            }
            else if (playerMapCol > ((g_CurrentMap.MapCols - 1) - Constants.NORMALVISIBLEPLAYERCOL)) //right
            {
                _minVisibleCol = g_CurrentMap.MapCols - Constants.VISIBLECOLUMNCOUNT;
                _maxVisibleCol = g_CurrentMap.MapCols - 1;
            }
            //min/max rows
            if (playerMapRow < Constants.NORMALVISIBLEPLAYERROW) //top
            {
                _minVisibleRow = 0;
                _maxVisibleRow = Constants.VISIBLEROWCOUNT;
            }
            else if (playerMapRow > ((g_CurrentMap.MapRows - 1) - Constants.NORMALVISIBLEPLAYERROW)) //bottom
            {
                _minVisibleRow = g_CurrentMap.MapRows - Constants.VISIBLEROWCOUNT;
                _maxVisibleRow = g_CurrentMap.MapRows - 1;
            }

            var drawBoundries = new DrawBoundries(_minVisibleCol, _maxVisibleCol, _minVisibleRow, _maxVisibleRow);
            
            return drawBoundries;
        }

        private class DrawBoundries
        {
            public int MinVisibleCol { get; private set; }
            public int MaxVisibleCol { get; private set; }
            public int MinVisibleRow { get; private set; }
            public int MaxVisibleRow { get; private set; }

            public DrawBoundries(int _minVisibleCol, int _maxVisibleCol, int _minVisibleRow, int _maxVisibleRow)
            {
                MinVisibleCol = _minVisibleCol;
                MaxVisibleCol = _maxVisibleCol;
                MinVisibleRow = _minVisibleRow;
                MaxVisibleRow = _maxVisibleRow;
            }
        }
    }
}