using System;
using System.Collections;
using System.Collections.Generic;

namespace Data
{
    public abstract class Component
    {
        public ICollection<Component> Components { get; set; }

        public abstract void Update();
    }
}