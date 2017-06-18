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

namespace JBookman_Conversion.EngineBits
{
    public class PlayerRenderer
    {
        private Player _currentPlayer;

        public PlayerRenderer()
        {

        }

        public void SetPlayer(Player player)
        {
            if (IsPlayerInitialised())
            {
                throw new InvalidOperationException("Player is already initialised");
            }
            _currentPlayer = player;
        }

        public bool IsPlayerInitialised()
        {
            return _currentPlayer != null;
        }

        public void RenderPlayer()
        {
            if (!IsPlayerInitialised())
            {
                throw new InvalidOperationException("Player is not initialised");
            }

            DrawPlayer(null, _currentPlayer, 0);
        }

        public static void DrawPlayer(Map g_CurrentMap, Player m_Player, int m_iPlayerTileSet)
        {
            /* int playerMapCol = m_Player.GetSector() % m_MapCols;
            int playerMapRow = (int)m_Player.GetSector() / m_MapRows;*/

            var playerBoundries = GetPlayerBoundries(g_CurrentMap, m_Player);
            
            //drawing the bastard.
            GL.BindTexture(TextureTarget.Texture2D, m_iPlayerTileSet); //set texture

            GL.Enable(EnableCap.Blend);
            // GL.BlendFunc(BlendingFactorSrc.SrcAlpha,BlendingFactorDest.OneMinusSrcAlpha);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.LoadIdentity();

            // TODO: TRANSLATE FIRST
            GL.Begin(PrimitiveType.Quads);
            //quad1
            //bottomleft
            GL.TexCoord2(0, 0);
            GL.Vertex3(playerBoundries.FinalVisiblePlayerCol, -(playerBoundries.FinalVisiblePlayerRow) - 1.0f, 1.0f);
            //top left
            GL.TexCoord2(0, 1);
            GL.Vertex3(playerBoundries.FinalVisiblePlayerCol, -(playerBoundries.FinalVisiblePlayerRow), 1.0f);
            //top right
            GL.TexCoord2(1, 1);
            GL.Vertex3(playerBoundries.FinalVisiblePlayerCol + 1.0f, -(playerBoundries.FinalVisiblePlayerRow), 1.0f);
            //bottom right
            GL.TexCoord2(1, 0);
            GL.Vertex3(playerBoundries.FinalVisiblePlayerCol + 1.0f, -(playerBoundries.FinalVisiblePlayerRow) - 1.0f, 1.0f);

            GL.End();

            GL.Disable(EnableCap.Blend);

            //end of DrawPlayer()
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
