using JBookman_Conversion.EngineBits.Abstract;
using System;
using JBookman_Conversion.EngineBits.Consts;
using JBookman_Conversion.EngineBits;
using OpenTK.Input;

namespace JBookman_Conversion.GameStates
{
    public class WorldState : IGameState, IDrawable, IUpdatable
    {
        internal Player _player;
        internal Map _currentMap;

        internal WorldInputHandler _inputHandler;

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
        }

        public ProcessState ProcessState => ProcessState.World;

        public void Draw(Renderer renderer)
        {
            var mapTileSetId = renderer.MainTileSetTextureId;
            var playerTileSetId = renderer.PlayerTileSetTextureId;

            var primitives = StaticRenderer.GetPrimitivesForStaticRenderer(_currentMap, mapTileSetId, playerTileSetId, _player);
            var playerPrimitive = StaticPlayerRenderer.GetPlayerPrimitive(_currentMap, _player, playerTileSetId);

            //var primis = GetRenderablePrimitives();

            renderer.BeginRender();
            renderer.RenderPrimitives(primitives.ToArray());

            StaticPlayerRenderer.RenderPlayerPrimitive(playerPrimitive);

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