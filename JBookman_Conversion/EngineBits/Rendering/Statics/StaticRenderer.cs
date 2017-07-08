using JBookman_Conversion.EngineBits.Rendering;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;
using System.Linq;

namespace JBookman_Conversion.EngineBits
{
    public static class StaticRenderer
    {
        public static List<Primitive> GetPrimitivesForStaticRenderer(Map currentMap, int glTextureId, int glPlayerTextureId, Player player)
        {
            var primitiveList = new List<Primitive>();

            var playerSector = player.GetSector();
            var mapTilesToRender = GetMapTiles(currentMap, playerSector, glTextureId);

            primitiveList.AddRange(mapTilesToRender);

            ////// Get Player primitive
            ////var playerPrimitive = StaticPlayerRenderer.GetPlayerPrimitive(currentMap, player, glPlayerTextureId);
            ////primitiveList.Add(playerPrimitive);

            return primitiveList;
        }

        private static List<Primitive> GetMapTiles(Map currentMap, int playerSector, int glTextureId)
        {
            var tilePrimitives = new List<Primitive>();

            var drawBoundries = GetDrawBoundries(currentMap, playerSector);

            int drawRow = 0;
            for (int currRow = drawBoundries.MinVisibleRow; currRow <= drawBoundries.MaxVisibleRow; currRow++)
            {
                int drawCol = 0;

                for (int currCol = drawBoundries.MinVisibleCol; currCol <= drawBoundries.MaxVisibleCol; currCol++)
                {
                    //get the map tile value
                    var currentMapSector = currentMap.m_MapSectors[currRow, currCol];
                    var tilePrimitive = new Primitive
                    {
                        X = currCol,
                        Y = currCol,
                        Z = 0,
                        TileId = currentMapSector.TileNumberId,
                        Rotation = currentMapSector.rotationAngle,
                        TextureId = glTextureId
                    };

                    tilePrimitives.Add(tilePrimitive);

                    drawCol++;
                }
                drawRow++;
            }
        
            return tilePrimitives;
        }

        ////internal static void Render(Map g_CurrentMap, int glMapTileSetTextureId, int m_iPlayerTileSet, Player m_Player, Matrix4 m_moveMatrix)
        ////{
        ////    GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        ////    Matrix4 modelview = Matrix4.LookAt(0.0f, 0.0f, 10.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f);

        ////    modelview = m_moveMatrix * modelview;

        ////    GL.MatrixMode(MatrixMode.Modelview);
        ////    GL.LoadIdentity();
        ////    GL.LoadMatrix(ref modelview);

        ////    //  move down by 10;
        ////    //  GL.Translate(0.0f, -0.0f, -10.0f);
        ////    //  GL.Rotate(180, Vector3.UnitY);

        ////    drawAxis();

        ////    GL.Enable(EnableCap.Texture2D);

        ////    //DrawTiles(g_CurrentMap, glMapTileSetTextureId, m_Player);

        ////    StaticPlayerRenderer.DrawPlayer(g_CurrentMap, m_Player, m_iPlayerTileSet);

        ////    GL.Disable(EnableCap.Texture2D);
        ////}

        internal static void RenderPrimitives(Primitive[] primitives, Matrix4 moveMatrix)
        {
            // Setup Rendering
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            Matrix4 modelview = Matrix4.LookAt(0.0f, 0.0f, 10.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f);

            modelview = moveMatrix * modelview;

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            GL.LoadMatrix(ref modelview);

            //  move down by 10;
            //  GL.Translate(0.0f, -0.0f, -10.0f);
            //  GL.Rotate(180, Vector3.UnitY);

            drawAxis();

            GL.Enable(EnableCap.Texture2D);

            // Render the prims!
            var primitivesGroupedByTextureId = primitives.GroupBy(p => p.TextureId);

            foreach (var grouping in primitivesGroupedByTextureId)
            {
                // Bind texture for these primitives
                var groupTextureKey = grouping.Key;
                GL.BindTexture(TextureTarget.Texture2D, groupTextureKey); //set texture

                foreach (var primitive in grouping)
                {
                    DrawPrimitive(primitive);
                }
            }

            //StaticPlayerRenderer.DrawPlayer(g_CurrentMap, m_Player, m_iPlayerTileSet);

            GL.Disable(EnableCap.Texture2D);
        }

        private static void DrawPrimitive(Primitive primitive)
        {
            //Proper GL way, translate grid, then draw at new 0.0
            GL.PushMatrix();

            // Matrices applied from right = last transform operation applied first!
            var translateVector = new Vector3(primitive.X, -primitive.Y, primitive.Z);
            GL.Translate(translateVector);

            if (primitive.Rotation != 0)
            {
                // Make origin in middle of texture
                GL.Translate(0.5, -0.5, 0.0);

                var rotateVex0 = new Vector3(0, 0, 1);
                GL.Rotate(primitive.Rotation, rotateVex0);

                //move origin back
                GL.Translate(-0.5, 0.5, 0.0);
            }

            if(primitive.TileId.HasValue)
            {
                DrawTile(primitive.TileId.Value);
            }

            GL.PopMatrix();
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

        ////private static void DrawTiles(Map currentMap, int tileSetId, Player player)
        ////{
        ////    var playerSector = player.GetSector();
        ////    var drawBoundries = GetDrawBoundries(currentMap, playerSector);

        ////    int tile;

        ////    GL.BindTexture(TextureTarget.Texture2D, tileSetId); //set texture

        ////    int drawRow = 0;

        ////    // Draw row by row so: outer == rows, inner == cols
        ////    // drawcol and drawrow == screen drawing, currRow and currCol == where we are on map.
        ////    // Drawing is always from 0,0, but we could be at map co-ord 50,50 or something.
        ////    for (int currRow = drawBoundries.MinVisibleRow; currRow <= drawBoundries.MaxVisibleRow; currRow++)
        ////    {
        ////        int drawCol = 0;

        ////        for (int currCol = drawBoundries.MinVisibleCol; currCol <= drawBoundries.MaxVisibleCol; currCol++)
        ////        {
        ////            //get the map tile value
        ////            var currentMapSector = currentMap.m_MapSectors[currRow, currCol];
        ////            tile = currentMapSector.TileNumberId;                    

        ////            //Proper GL way, translate grid, then draw at new 0.0
        ////            GL.PushMatrix();

        ////            // Matrices applied from right = last transform operation applied first!
        ////            var translateVector = new Vector3(drawCol, -drawRow, 0);
        ////            GL.Translate(translateVector);

        ////            if(currentMapSector.rotationAngle != 0)
        ////            {
        ////                // Make origin in middle of texture
        ////                GL.Translate(0.5, -0.5, 0.0);

        ////                var rotateVex0 = new Vector3(0, 0, 1);
        ////                GL.Rotate(currentMapSector.rotationAngle, rotateVex0);

        ////                //move origin back
        ////                GL.Translate(-0.5, 0.5, 0.0);
        ////            }

        ////            DrawTile(tile);

        ////            GL.PopMatrix();

        ////            drawCol++;
        ////        }
        ////        drawRow++;
        ////    }
        ////    //end of drawtiles
        ////}

        private static DrawBoundries GetDrawBoundries(Map g_CurrentMap, int playerSector)
        {
            int _minVisibleCol, _maxVisibleCol, _minVisibleRow, _maxVisibleRow;

            int playerMapCol = MapUtils.SectorToCols(playerSector, g_CurrentMap.MapCols);
            int playerMapRow = MapUtils.SectorToRow(playerSector, g_CurrentMap.MapRows);

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