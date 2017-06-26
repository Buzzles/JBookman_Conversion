using JBookman_Conversion.EngineBits.Abstract;
using System;
using JBookman_Conversion.EngineBits.Consts;

namespace JBookman_Conversion.GameStates
{
    public class WorldState : IGameState, IDrawable, IUpdatable
    {
        public ProcessState ProcessState => ProcessState.World;

        public void Draw(float dt)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }
    }
}
