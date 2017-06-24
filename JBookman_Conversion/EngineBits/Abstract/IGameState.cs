using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JBookman_Conversion.EngineBits.Abstract
{
    public interface IGameState
    {
        void Entering(); // Called after being added to the top of the stack

        void Leaving(); // Called just before popping off the stack

        void Obscuring(); // When new state is about to be added to the stack ontop of this
        void Revealing(); // When this one becomes top of the stack
    }
}
