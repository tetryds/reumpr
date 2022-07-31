using System;
using System.Collections.Generic;
using System.IO;

namespace tetryds.Reumpr.Service
{
    public class RawMessage
    {
        public int Status;
        public int Id;
        public string Name;
        public byte[][] Payload;

        public RawMessage CopyHeader()
        {
            return new RawMessage
            {
                Status = Status,
                Id = Id,
                Name = Name
            };
        }

        public RawMessage CopyHeader(byte[][] payload)
        {
            return new RawMessage
            {
                Status = Status,
                Id = Id,
                Name = Name,
                Payload = payload
            };
        }

        public override bool Equals(object obj)
        {
            return obj is RawMessage message &&
                   Status == message.Status &&
                   Id == message.Id &&
                   Name == message.Name &&
                   PayloadEquals(message.Payload);
        }

        public override int GetHashCode()
        {
            var hashCode = -1771310190;
            hashCode = hashCode * -1521134295 + Status.GetHashCode();
            hashCode = hashCode * -1521134295 + Id.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + EqualityComparer<byte[][]>.Default.GetHashCode(Payload);
            return hashCode;
        }

        private bool PayloadEquals(byte[][] other)
        {
            if (Payload.Length != other.Length) return false;

            for (int i = 0; i < Payload.Length; i++)
            {
                if (Payload[i].Length != other[i].Length) return false;

                for (int j = 0; j < Payload[i].Length; j++)
                {
                    if (Payload[i][j] != other[i][j]) return false;
                }
            }

            return true;
        }
    }
}
