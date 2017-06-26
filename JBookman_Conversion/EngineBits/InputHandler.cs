﻿using OpenTK.Input;
using MessageBox = System.Windows.Forms.MessageBox;
using OpenTK;
using JBookman_Conversion.EngineBits.Consts;

namespace JBookman_Conversion.EngineBits
{
    public class InputHandler
    {
        private Map _currentMap;

        private Player _player;

        private const float moveAmount = 0.1f;
        private KeyboardState _lastKeyState, _keyboardState;

        public InputHandler(Map currentMap, Player player)
        {
            _currentMap = currentMap;
            _player = player;
        }

        private void MovePlayerUp()
        {
            g_iDirection dir = g_iDirection.NORTH;
            if (CharacterCanMove(dir, _player.GetSector()))
                _player.SetSector(_player.GetSector() - _currentMap.MapCols);
        }
        private void MovePlayerDown()
        {
            g_iDirection dir = g_iDirection.SOUTH;
            if (CharacterCanMove(dir, _player.GetSector()))
                _player.SetSector(_player.GetSector() + _currentMap.MapCols);
        }
        private void MovePlayerRight()
        {
            g_iDirection dir = g_iDirection.EAST;
            if (CharacterCanMove(dir, _player.GetSector()))
                _player.SetSector(_player.GetSector() + 1);
        }
        private void MovePlayerLeft()
        {
            g_iDirection dir = g_iDirection.WEST;
            if (CharacterCanMove(dir, _player.GetSector()))
                _player.SetSector(_player.GetSector() - 1);
        }

        private bool CharacterCanMove(g_iDirection direction, int currentSector)
        {
            bool move = true;

            var currentRow = MapUtils.SectorToRow(currentSector, _currentMap.MapRows);
            var currentCol = MapUtils.SectorToCols(currentSector, _currentMap.MapCols);

            if (direction == g_iDirection.NORTH)
            {
                if (currentRow < 1)
                {
                    move = false;
                    MessageBox.Show("North, \ntop of map, \nmove=false, \nSectorToRow=" + currentRow);
                }
                else if (PlayerBlocked(currentSector, direction))
                {
                    move = false;
                }
            }
            if (direction == g_iDirection.SOUTH)
            {
                if (currentRow >= (_currentMap.MapRows - 1))
                {
                    move = false;
                    MessageBox.Show("South, \nBottom of map, \nmove=false, \nSectorToRow=" + currentRow);
                }
                else if (PlayerBlocked(currentSector, direction))
                {
                    move = false;
                }
            }

            if (direction == g_iDirection.EAST)
            {
                if (currentCol >= (_currentMap.MapCols - 1))
                {
                    move = false;
                    MessageBox.Show("East, \nRight of map, \nmove=false, \nSectorToCol=" + currentCol);
                }
                else if (PlayerBlocked(currentSector, direction))
                {
                    move = false;
                }
            }
            if (direction == g_iDirection.WEST)
            {
                if (currentCol <= 0)
                {
                    move = false;
                    MessageBox.Show("West, \nLeft of map, \nmove=false, \nSectorToCol=" + currentCol);
                }
                else if (PlayerBlocked(currentSector, direction))
                {
                    move = false;
                }
            }

            return move;
        }

        private bool PlayerBlocked(int iPlayerSector, g_iDirection direction)
        {
            bool blocked = false;
            int iSector = iPlayerSector;

            // Based on direction travelling, get locaion of next tile
            if (direction == g_iDirection.NORTH)
            {
                iSector = iSector - _currentMap.MapCols;
            }
            else if (direction == g_iDirection.SOUTH)
            {
                iSector = iSector + _currentMap.MapCols;
            }
            else if (direction == g_iDirection.EAST)
            {
                iSector = iSector + 1;
            }
            else if (direction == g_iDirection.WEST)
            {
                iSector = iSector - 1;
            }

            int y = MapUtils.SectorToRow(iSector, _currentMap.MapRows);
            int x = MapUtils.SectorToCols(iSector, _currentMap.MapCols);

            int item = _currentMap.m_MapSectors[y, x].TileNumberId;

            g_iMapTypeEnum mapType = (g_iMapTypeEnum)_currentMap.MapTypeId;

            //is tile set to be impassable?
            if (_currentMap.m_MapSectors[y, x].Impassable == true)
            {
                blocked = true;
            }

            return blocked;
        }

        internal void HandleKeyboardDown(FrameEventArgs e, Game gameContext, Engine engineContext)
        {
            _keyboardState = gameContext.Keyboard.GetState();

            if (KeyPress(Key.Escape))
            {
                gameContext.Exit();
            }

            if (KeyPress(Key.Up))
            {
                MovePlayerUp();
            }

            if (KeyPress(Key.Down))
            {
                MovePlayerDown();
            }

            if (KeyPress(Key.Left))
            {
                MovePlayerLeft();
            }
            if (KeyPress(Key.Right))
            {
                MovePlayerRight();
            }
            //  _player location + viewport test code
            if (KeyPress(Key.Number1))
            {
                _player.SetSector(85);
            }
            if (KeyPress(Key.Number2))
            {
                _player.SetSector(842);
            }
            if (KeyPress(Key.Number3))
            {
                _player.SetSector(1530);
            }
            if (KeyPress(Key.Number4))
            {
                _player.SetSector(820);
            }
            if (KeyPress(Key.Number5))
            {
                _player.SetSector(1558);
            }
            if (KeyPress(Key.Number0))
            {
                _player.SetSector(0);
            }

            // State change!
            var engine = engineContext;

            if (KeyPress(Key.F1))
            {
                engine.StateHandler.MoveNext(ProcessAction.GoToMenu);
            }

            if (KeyPress(Key.F2))
            {
                engine.StateHandler.MoveNext(ProcessAction.ToWorld);
            }

            if (KeyPress(Key.F3))
            {
                engine.StateHandler.MoveNext(ProcessAction.BattleStart);
            }

            // Store for next update method
            _lastKeyState = _keyboardState;
        }

        private bool KeyPress(Key key)
        {
            return (_keyboardState[key] && (_keyboardState[key] != _lastKeyState[key]));
        }
    }
}
