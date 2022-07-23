using System;
using System.Runtime.Serialization;

namespace tetryds.Reumpr
{
    [Serializable]
    public class MissingClientException : ConnectionException
    {
        public readonly Guid Id;

        public MissingClientException(Guid id)
        {
            Id = id;
        }

        public MissingClientException(Guid id, string message) : base(message)
        {
            Id = id;
        }

        public MissingClientException(Guid id, string message, Exception innerException) : base(message, innerException)
        {
            Id = id;
        }

        protected MissingClientException(Guid id, SerializationInfo info, StreamingContext context) : base(info, context)
        {
            Id = id;
        }
    }
}