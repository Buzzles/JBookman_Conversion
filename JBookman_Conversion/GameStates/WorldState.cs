using JBookman_Conversion.EngineBits.Abstract;
using System;
using JBookman_Conversion.EngineBits.Consts;
using JBookman_Conversion.EngineBits;

namespace JBookman_Conversion.GameStates
{
    public class WorldState : IGameState, IDrawable, IUpdatable
    {
        internal Player _player;
        internal Map _currentMap;

        public WorldState()
        {
        }

        public WorldState(Map initialMap, Player player)
        {
            _currentMap = initialMap;
            _player = player;
        }

        public ProcessState ProcessState => ProcessState.World;

        public void Draw(Renderer renderer)
        {
            var mapTileSetId = renderer.MainTileSetTextureId;
            var playerTileSetId = renderer.PlayerTileSetTextureId;

            var primitives = StaticRenderer.GetPrimitivesForStaticRenderer(_currentMap, mapTileSetId, playerTileSetId, _player);

            StaticRenderer.RenderPrimitives(primitives.ToArray(), renderer.MoveMatrix);

            var playerPrimitive = StaticPlayerRenderer.GetPlayerPrimitive(_currentMap, _player, playerTileSetId);
            StaticPlayerRenderer.RenderPlayerPrimitive(playerPrimitive);

            //StaticRenderer.Render(_currentMap, m_iCurrentTileSet, m_iPlayerTileSet, _player, m_moveMatrix);
            //throw new NotImplementedException();

            //var primis = GetRenderablePrimitives();
            //renderer.RenderPrimitives(primis);
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

        public void Update()
        {
        }
    }
}
