﻿using JBookman_Conversion.EngineBits.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JBookman_Conversion.EngineBits
{
    public class DirectRenderStackStateManager
    {
        private Stack<IGameState> _states;

        private List<IUpdatable> _updatables;

        private List<IDrawable> _drawables;

        public DirectRenderStackStateManager()
        {
            _states = new Stack<IGameState>();
            _updatables = new List<IUpdatable>();
            _drawables = new List<IDrawable>();
        }

        public void Switch(IGameState newState)
        {
            // ??
        }

        public IGameState Peek()
        {
            return _states.Peek();
        }

        public IGameState Pop()
        {
            return _states.Pop();
        }

        public void Push(IGameState state)
        {
            _states.Push(state);
        }

        public void Draw(float dt)
        {
            for (var i = 0; i < _drawables.Count(); i++)
            {
                _drawables[i].Draw(dt);
            }
        }

        public void Update(float dt)
        {
            // Foreach?
            for (var i = 0; i < _updatables.Count(); i++)
            {
                _updatables[i].Update();
            }
        }

    }
}
