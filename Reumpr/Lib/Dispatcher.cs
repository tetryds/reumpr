using System;
using System.Collections.Concurrent;

namespace tetryds.Reumpr
{
    public class Dispatcher<T>
    {
        IProducerConsumerCollection<T> send;


        //public void SendMessage(T msg)
        //{
        //    send.TryAdd
        //    send.Enqueue(msg);
        //}

        //public bool TryGetMessage(out T msg)
        //{
        //    return recv.TryDequeue(out msg);
        //}
    }
}
