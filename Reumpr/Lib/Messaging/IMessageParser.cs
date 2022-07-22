using System;
using System.Collections.Concurrent;

namespace tetryds.Reumpr
{
    public interface IMessageParser<T>
    {
        T Parse(byte[] data);
        byte[] Serialize(T msg);
    }
}
