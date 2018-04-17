using System;
using System.Text;
using System.Threading;

namespace GzipApp
{
    public class OrderedBuffer
    {
        private object _locker = new object();
        private LinkedListNode<Block> head;
        private bool closing;
        private int count = 0;
        private readonly int max_size;
        private int waiting_index = -1;

        public OrderedBuffer(int max_size)
        {
            this.max_size = max_size;
        }

        public void Close()
        {
            lock (_locker)
            {
                closing = true;
                Monitor.PulseAll(_locker); 
            } 
        }

        private void InsertSorted(ref LinkedListNode<Block> head, Block value)
        {
            var new_element = new LinkedListNode<Block>(value);

            if (head == null)
            {
                head = new_element;
                return;
            }

            if (value.BlockNumber < head.Value.BlockNumber)
            {
                new_element.Next = head;
                head = new_element;
                return;
            }

            LinkedListNode<Block> cur = head;
            LinkedListNode<Block> prev = null;

            while (cur != null)
            {
                if (new_element.Value.BlockNumber < cur.Value.BlockNumber)
                    break;

                prev = cur;
                cur = cur.Next;
            }

            prev.Next = new_element;
            new_element.Next = cur;

            return;
        }

        private void DeleteHead(ref LinkedListNode<Block> head)
        {
            head = head.Next;
        }

        public void Add(int index, Block item)
        {
            lock (_locker)
            {
                while (count >= max_size)
                    Monitor.Wait(_locker);

                this.InsertSorted(ref this.head, item);
                count++;

                if (waiting_index != -1 && waiting_index == index)
                    Monitor.PulseAll(_locker);
                
                if (count == 1)
                    Monitor.PulseAll(_locker);
            }
        }

        public bool TryGet(int index, out Block value)
        {
            lock (_locker)
            {
                while (count == 0)
                {
                    if (closing)
                    {
                        value = null;
                        return false;
                    }

                    Monitor.Wait(_locker);
                }

                waiting_index = index;

                while (head.Value.BlockNumber != waiting_index)
                {
                    Monitor.Wait(_locker);
                }

                waiting_index = -1;
                
                value = head.Value;
                this.DeleteHead(ref head);
                count--;

                if (count == max_size - 1)
                    Monitor.PulseAll(_locker);

                return true;
            }
        }
    }
}
