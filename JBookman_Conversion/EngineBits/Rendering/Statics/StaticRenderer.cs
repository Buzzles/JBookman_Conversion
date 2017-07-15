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
                        X = drawCol,
                        Y = drawRow,
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
    }
}