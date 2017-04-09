using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Audio;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace JBookman_Conversion.Engine
{
    public class Renderer
    {
        private PlayerRenderer _playerRenderer;

        public Renderer()
        {
            _playerRenderer = new PlayerRenderer();
        }

        internal void RenderFrame()
        {
            //throw new NotImplementedException();
        }
    }
}
