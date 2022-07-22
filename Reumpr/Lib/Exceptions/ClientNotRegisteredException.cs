using System;
using System.Runtime.Serialization;

namespace tetryds.Reumpr
{
    [Serializable]
    internal class ClientNotRegisteredException : DispatcherException
    {
        public readonly Guid Id;

        public ClientNotRegisteredException()
        {
        }

        public ClientNotRegisteredException(Guid id)
        {
            Id = id;
        }

        public ClientNotRegisteredException(string message) : base(message)
        {
        }

        public ClientNotRegisteredException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ClientNotRegisteredException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}