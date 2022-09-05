using System;
using System.Runtime.Serialization;

namespace tetryds.Reumpr.Client
{
    [Serializable]
    internal class MessageIdException : Exception
    {
        public readonly int Id;

        public MessageIdException(int id, string message) : base(message)
        {
            Id = id;
        }

        public MessageIdException(int id, string message, Exception innerException) : base(message, innerException)
        {
            Id = id;
        }

        protected MessageIdException(int id, SerializationInfo info, StreamingContext context) : base(info, context)
        {
            Id = id;
        }
    }
}