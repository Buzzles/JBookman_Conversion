using JBookman_Conversion.EngineBits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageBox = System.Windows.Forms.MessageBox;

namespace JBookman_Conversion.EngineParts
{
    public class Engine
    {
        internal InputHandler InputHandler { get; private set; }

        internal StateHandler StateHandler { get; private set; }

        internal Player _player;
        internal Map _currentMap;

        public Engine()
        {

        }

        public Player GetPlayer()
        {
            return _player;
        }

        public Map GetMap()
        {
            return _currentMap;
        }

        // TEMP, use DI? Or roll own initialiser?
        public void InitialiseEngine()
        {
            InputHandler = new InputHandler(_currentMap, _player);
            StateHandler = new StateHandler();
        }

        public void InitGame()
        {
        
        }

        internal void InitMap(string mapPathAndName, int firstMapType)
        {
            //  Create new map instance as a loader then use it load the map (not ideal)
            var g_FirstMap = new Map();
            var g_CurrentMap = new Map();

            g_FirstMap = Map.ReadMapFile(mapPathAndName);

            //if firstmap has loaded, set it to be current map, otherwise it's failed to load
            if (g_FirstMap != null)
            {
                _currentMap = g_FirstMap;
            }
            else
            {
                MessageBox.Show("Map load failure");
            }

            MessageBox.Show("Firstmap loaded rows: " + g_FirstMap.MapRows);
            MessageBox.Show("Firstmap loaded tile 1,1: " + g_FirstMap.m_MapSectors[1, 1].TileNumberId);
        }

        internal void InitPlayer(int startSector)
        {
            _player = new Player();

            _player.SetGold(25);
            _player.SetHitPoints(10);
            _player.SetMaxHitPoints(10);
            _player.SetKeys(1);
            _player.SetSector(startSector);
        }
    }
}