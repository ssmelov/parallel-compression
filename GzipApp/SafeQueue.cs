using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace GzipApp
{
    public class SafeQueue<T>
    {
        private readonly Queue<T> queue = new Queue<T>();
        private readonly int max_size;
        private bool closing;
    
        public SafeQueue(int max_size)
        {
            this.max_size = max_size; 
        }

        public void Close()
        {
            lock (queue)
            {
                closing = true;
                Monitor.PulseAll(queue); 
            }
        }

        public void Enqueue(T item)
        {
            lock (queue)
            {
                while (queue.Count >= max_size)
                    Monitor.Wait(queue);
                
                queue.Enqueue(item);

                if (queue.Count == 1)
                    Monitor.PulseAll(queue); 
            }
        }

        public bool TryDequeue(out T value)
        {
            lock (queue)
            {
                while (queue.Count == 0)
                {
                    if (closing)
                    {
                        value = default(T);
                        return false;
                    }

                    Monitor.Wait(queue);
                }

                value = queue.Dequeue();

                if (queue.Count == max_size - 1)
                    Monitor.PulseAll(queue);

                return true; 
            }
        }
    }
}
