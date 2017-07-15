using System;
using System.Collections.Generic;
using System.Text;

namespace Data
{
    public class GameObject
    {
        private List<Component> _components { get; set; }

        public GameObject()
        {
            _components = new List<Component>();
        }

        public void AddComponent(Component newComponent)
        {
            _components.Add(newComponent);
        }

        public void RemoveComponent(Component componentToRemove)
        {
            _components.Remove(componentToRemove);
        }

        public void FlushComponents()
        {
            _components.Clear();
        }
    }
}
