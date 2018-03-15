using JBookman_Conversion.EngineBits;
using JBookman_Conversion.EngineBits.Abstract;
using JBookman_Conversion.EngineBits.Consts;
using JBookman_Conversion.GameStates.BattleComponents;
using System;

namespace JBookman_Conversion.GameStates
{
    public class BattleState : IGameState, IDrawable
    {
        public ProcessState ProcessState => ProcessState.Battle;

        private BattleRenderer _battleRenderer; //temp

        public BattleState()
        {
            _battleRenderer = new BattleRenderer();
        }

        public void Draw(Renderer renderer)
        {
            var textureId = renderer.MainTileSetTextureId;
            _battleRenderer.DrawBattle(textureId);

            //renderer.BeginRender();
            //var primitives = new Primitive[0];
            //renderer.RenderPrimitives(primitives.ToArray());
            //renderer.EndRender();
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
    }
}
