using System;
using System.Runtime.Serialization;

namespace tetryds.Reumpr
{
    [Serializable]
    public class ClientReceiveException : ConnectionException
    {
        public readonly Guid Id;

        public ClientReceiveException(Guid id, string message) : base(message)
        {
            Id = id;
        }

        public ClientReceiveException(Guid id, string message, Exception innerException) : base(message, innerException)
        {
            Id = id;
        }

        protected ClientReceiveException(Guid id, SerializationInfo info, StreamingContext context) : base(info, context)
        {
            Id = id;
        }
    }
}