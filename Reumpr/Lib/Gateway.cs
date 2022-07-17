using System;
using System.Collections.Concurrent;

namespace tetryds.Reumpr
{
    public class Gateway<T>
    {
        private ConcurrentQueue<T> recv;
        private ConcurrentQueue<T> send;


        public void SendMessage(T msg)
        {
            send.Enqueue(msg);
        }

        public bool TryGetMessage(out T msg)
        {
            return recv.TryDequeue(out msg);
        }
    }
}
