using System;
using OpenTK.Input;

namespace JBookman_Conversion.EngineBits.Abstract
{
    public interface IUpdatable
    {
        //void Update();
        UpdateResult Update(KeyboardState keyboardState);
    }
}
