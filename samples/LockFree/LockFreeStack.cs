using System;
using System.Threading;

namespace LockFree
{
    public class LockFreeStack<T> : IStack<T>
    {
        private Node<T> head;

        public void Push(T obj)
        {
            var newHead = new Node<T> { Value = obj };
            if (head != null)
                Interlocked.Exchange(ref newHead.Next, head);
            Interlocked.Exchange(ref head, newHead);
        }

        public T Pop()
        {
            T value;
            Node<T> prevHead = null;
            if (head == null)
            {
                throw new NullReferenceException();
            }
            Interlocked.Exchange(ref prevHead, head);
            value = prevHead.Value;
            Interlocked.Exchange(ref head, head.Next);
            return value;
        }
    }
}