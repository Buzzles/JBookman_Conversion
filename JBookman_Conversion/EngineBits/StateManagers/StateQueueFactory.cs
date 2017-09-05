using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JBookman_Conversion.EngineBits.StateManagers
{
    public static class StateQueueFactory
    {
        private static Queue<StateQueueItem> _queue;

        public static void AddToQueue(StateQueueItem newItem)
        {
            var queue = GetQueue();

            if (newItem != null)
            {
                _queue.Enqueue(newItem);
            }
        }

        public static StateQueueItem GetNext()
        {
            var queue = GetQueue();

            if (queue.Any())
            {
                return queue.Dequeue();
            }

            return null;
        }


        private static Queue<StateQueueItem> GetQueue()
        {
            if (_queue == null)
            {
                _queue = new Queue<StateQueueItem>();
            }

            return _queue;
        }
    }
}