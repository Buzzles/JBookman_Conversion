﻿using JBookman_Conversion.EngineBits.Abstract;
using System;

namespace JBookman_Conversion.GameStates
{
    public class MenuState : IGameState, IDrawable, IUpdatable
    {
        private MenuRenderer _menuRenderer;

        public MenuState()
        {
            _menuRenderer = new MenuRenderer();
        }

        public void Draw(float dt)
        {
            _menuRenderer.DrawMenu();
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