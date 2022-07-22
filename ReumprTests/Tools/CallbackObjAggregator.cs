using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace tetryds.Reumpr.Tests.Tools
{
    public class CallbackObjCollection<T>
    {
        readonly List<T> objects;
        readonly int pollingPeriod;

        public CallbackObjCollection(int pollingPeriod)
        {
            objects = new List<T>();
            this.pollingPeriod = pollingPeriod;
        }

        public void Add(T item)
        {
            lock(objects)
                objects.Add(item);
        }

        public void Reset()
        {
            lock (objects)
                objects.Clear();
        }

        public int GetCount()
        {
            lock (objects)
                return objects.Count;
        }

        public List<T> GetObjects()
        {
            lock (objects)
                return objects.ToList();
        }

        public bool Wait(int count, int timeoutMs)
        {
            bool expired = false;
            void expire(object o) => expired = true;
            using (Timer timer = new Timer(expire, null, timeoutMs, Timeout.Infinite))
            {
                while (!expired)
                {
                    if (GetCount() >= count)
                        return true;
                    Thread.Sleep(pollingPeriod);
                }
            }
            return false;
        }
    }
}
