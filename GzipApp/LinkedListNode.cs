
namespace GzipApp
{
    public class LinkedListNode<T>
    {
        public LinkedListNode<T> Next { get; set; }

        public T Value { get; set; }

        public LinkedListNode(T val)
        {
            this.Value = val;
        }
    }
}
