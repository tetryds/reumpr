using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace tetryds.Reumpr.Collections
{
    public struct Package<T>
    {
        public readonly Guid Id;
        public readonly T Msg;

        public Package(Guid id, T msg)
        {
            Id = id;
            Msg = msg;
        }

        public void Deconstruct(out Guid id, out T msg)
        {
            id = Id;
            msg = Msg;
        }
    }
}
