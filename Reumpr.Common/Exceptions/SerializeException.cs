using System;
using System.Runtime.Serialization;

namespace tetryds.Reumpr.Common.Exceptions
{
    [Serializable]
    public class SerializeException : Exception
    {
        public SerializeException()
        {
        }

        public SerializeException(string message) : base(message)
        {
        }

        public SerializeException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected SerializeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}