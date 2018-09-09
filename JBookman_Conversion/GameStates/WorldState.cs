using JBookman_Conversion.EngineBits;
using JBookman_Conversion.EngineBits.Abstract;
using JBookman_Conversion.EngineBits.Consts;
using JBookman_Conversion.GameStates.WorldComponents;
using OpenTK.Input;
using System;

namespace JBookman_Conversion.GameStates
{
    public class WorldState : IGameState, IDrawable, IUpdatable
    {
        internal Player _player;
        internal Map _currentMap;

        internal WorldInputHandler _inputHandler;

        internal WorldDrawer _worldDrawer;
        internal PlayerDrawer _playerDrawer;

        internal UpdateResult _updateResult;

        public WorldState()
        {
        }

        public WorldState(Map initialMap, Player player)
        {
            _currentMap = initialMap;
            _player = player;

            _updateResult = new UpdateResult();

            _inputHandler = new WorldInputHandler(_currentMap, _player);

            _worldDrawer = new WorldDrawer();
            _playerDrawer = new PlayerDrawer();
        }

        public ProcessState ProcessState => ProcessState.World;

        public void Draw(Renderer renderer)
        {
            var mapTileSetId = renderer.MainTileSetTextureId;
            var playerTileSetId = renderer.PlayerTileSetTextureId;

            var primitives = _worldDrawer.GetPrimitivesToRender(_currentMap, mapTileSetId, playerTileSetId, _player);
            var playerPrimitive = _playerDrawer.GetPlayerPrimitive(_currentMap, _player, playerTileSetId);

            renderer.BeginRender();
            renderer.RenderPrimitives(primitives.ToArray());

            // TODO: Move player rendering code to actual Renderer.cs
            renderer.RenderPlayerPrimitive(playerPrimitive);
            //renderer.RenderPrimitives(new[] { playerPrimitive });

            renderer.EndRender();
        }

        public void Entering()
        {
            throw new NotImplementedException();
        }

        public void Leaving()
        {
            throw new NotImplementedException();
        }

        public void Obscuring()
        {
            throw new NotImplementedException();
        }

        public void Revealing()
        {
            throw new NotImplementedException();
        }

        public void Update(KeyboardState keyboardState)
        {
            _inputHandler.HandleKeyboardDown(keyboardState);
        }
    }
}