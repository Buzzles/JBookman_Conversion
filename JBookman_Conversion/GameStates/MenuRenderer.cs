using System;
using OpenTK.Graphics.OpenGL;
using OpenTK;
using JBookman_Conversion.EngineBits;

namespace JBookman_Conversion.GameStates
{
    internal class MenuRenderer
    {
        private int _mapTileSetId;
        private Matrix4 _moveMatrix;

        public void Initialise(Map currentMap, int mapTileSet, Matrix4 moveMatrix)
        {
            _mapTileSetId = mapTileSet;
            _moveMatrix = moveMatrix;
        }

        internal void DrawMenu()
        {
            if (!Initialised())
            {
                throw new InvalidOperationException("Menu renderer not initialised");
            }

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            Matrix4 modelview = Matrix4.LookAt(0.0f, 0.0f, 10.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f);

            modelview = _moveMatrix * modelview;

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            GL.LoadMatrix(ref modelview);
            
            DrawAxis();

            GL.Enable(EnableCap.Texture2D);

            DrawTiles(_mapTileSetId);

            GL.Disable(EnableCap.Texture2D);
        }

        private bool Initialised()
        {
            //if (_mapTileSetId == 0)
            //{
            //    return false;
            //}

            if (_moveMatrix == null)
            {
                return false;
            }

            return true;
        }

        private static void DrawAxis()
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

        private static void DrawTiles(int tileSetId)
        {
            var drawBoundries = GetDrawBoundries();

            int tile;

            GL.BindTexture(TextureTarget.Texture2D, tileSetId); //set texture

            int drawRow = 0;

            for (int currRow = drawBoundries.MinVisibleRow; currRow <= drawBoundries.MaxVisibleRow; currRow++)
            {
                int drawCol = 0;

                for (int currCol = drawBoundries.MinVisibleCol; currCol <= drawBoundries.MaxVisibleCol; currCol++)
                {
                    tile = 0; // default tile for now

                    GL.PushMatrix();

                    // Matrices applied from right = last transform operation applied first!
                    var translateVector = new Vector3(drawCol, -drawRow, 0);
                    GL.Translate(translateVector);

                    DrawTile(tile);

                    GL.PopMatrix();

                    drawCol++;
                }

                drawRow++;
            }
        }

        private static DrawBoundries GetDrawBoundries()
        {
            //var _minVisibleCol = 0 - Constants.NORMALVISIBLEPLAYERCOL;
            var _maxVisibleCol = 0 + Constants.NORMALVISIBLEPLAYERCOL;

            //var _minVisibleRow = 0 - Constants.NORMALVISIBLEPLAYERROW;
            var _maxVisibleRow = 0 + Constants.NORMALVISIBLEPLAYERROW;
            
            var drawBoundries = new DrawBoundries(0, _maxVisibleCol, 0, _maxVisibleRow);

            return drawBoundries;
        }

        private static void DrawTile(int tilesetTileNumber)
        {
            //calulate tilenumber's row and column value on tileset
            // int numberofcolumns = 2;
            int row;
            int column;
            column = tilesetTileNumber % Constants.TILESETCOLUMNCOUNT;
            float texture_size = 1.0f / Constants.TILESETCOLUMNCOUNT;
            //0.5 = size
            row = (int)((tilesetTileNumber * texture_size) + 0.00001f);
            // MessageBox.Show("tile number: " + tile +" row: "+row+" col: "+column +" texturesize:"+texture_size);
            float s1 = texture_size * (column + 0);
            float s2 = texture_size * (column + 1);
            float t1 = 1 - (texture_size * (row + 0));
            float t2 = 1 - (texture_size * (row + 1));

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
        }
    }
}