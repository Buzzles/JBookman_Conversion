using System;
using OpenTK.Graphics.OpenGL;
using JBookman_Conversion.EngineBits.Rendering;
using OpenTK;
using JBookman_Conversion.EngineBits;

namespace JBookman_Conversion.GameStates
{
    public class PlayerDrawer
    {
        internal Primitive GetPlayerPrimitive(Map currentMap, Player player, int textureId)
        {
            var playerBoundries = GetPlayerBoundries(currentMap, player);

            var playerPrimitive = new Primitive
            {
                X = playerBoundries.FinalVisiblePlayerCol,
                Y = playerBoundries.FinalVisiblePlayerRow,
                Z = 1.0f,
                TextureId = textureId,
                TileId = 0
            };

            return playerPrimitive;
        }

        private static PlayerBoundries GetPlayerBoundries(Map currentMap, Player player)
        {
            int playerMapCol = MapUtils.SectorToCols(player.GetSector(), currentMap.MapCols);
            int playerMapRow = MapUtils.SectorToRow(player.GetSector(), currentMap.MapRows);

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
            else if (playerMapCol > ((currentMap.MapCols - 1) - Constants.NORMALVISIBLEPLAYERCOL))
            {
                FinalVisiblePlayerCol =
                    Constants.VISIBLECOLUMNCOUNT - ((currentMap.MapCols - 1) - playerMapCol) - 1;
                //-1 on end is to take into account the visible display is 25 tiles wide, but range is 0 to 24.
            }
            //if location is top of last possible uppermost viewport centre line
            if (playerMapRow < Constants.NORMALVISIBLEPLAYERROW)
            {
                FinalVisiblePlayerRow =
                    Constants.NORMALVISIBLEPLAYERROW - (Constants.NORMALVISIBLEPLAYERROW - playerMapRow);
            }
            //else if location is below last possible lowermost viewport centre line
            else if (playerMapRow > ((currentMap.MapRows - 1) - Constants.NORMALVISIBLEPLAYERROW))
            {
                FinalVisiblePlayerRow =
                    Constants.VISIBLEROWCOUNT - ((currentMap.MapRows - 1) - playerMapRow) - 1;

            }

            return new PlayerBoundries
            {
                FinalVisiblePlayerCol = FinalVisiblePlayerCol,
                FinalVisiblePlayerRow = FinalVisiblePlayerRow
            };
        }

        private class PlayerBoundries
        {
            public float FinalVisiblePlayerCol { get; set; }
            public float FinalVisiblePlayerRow { get; set; }
        }
    }
}