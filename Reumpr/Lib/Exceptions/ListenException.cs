using System;
using System.Runtime.Serialization;

namespace tetryds.Reumpr
{
    [Serializable]
    public class ListenException : Exception
    {
        public readonly int Port;

        public ListenException(int port, string message) : base(message)
        {
            Port = port;
        }

        public ListenException(int port, string message, Exception innerException) : base(message, innerException)
        {
            Port = port;
        }

        protected ListenException(int port, SerializationInfo info, StreamingContext context) : base(info, context)
        {
            Port = port;
        }
    }
}