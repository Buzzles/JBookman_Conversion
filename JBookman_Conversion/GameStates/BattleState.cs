using JBookman_Conversion.EngineBits;
using JBookman_Conversion.EngineBits.Abstract;
using JBookman_Conversion.EngineBits.Consts;
using JBookman_Conversion.EngineBits.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JBookman_Conversion.GameStates
{
    public class BattleState : IGameState, IDrawable
    {
        public ProcessState ProcessState => ProcessState.Battle;

        private MenuRenderer _menuRenderer; //temp

        public BattleState()
        {
            _menuRenderer = new MenuRenderer();
        }

        public void Draw(Renderer renderer)
        {
            var textureId = renderer.MainTileSetTextureId;
            _menuRenderer.DrawMenu(textureId);

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
