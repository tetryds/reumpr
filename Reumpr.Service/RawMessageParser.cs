using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace tetryds.Reumpr.Service
{
    public class RawMessageParser : IMessageParser<RawMessage>
    {
        public RawMessage Parse(byte[] data)
        {
            RawMessage message = new RawMessage();

            int shift = 0;
            message.Status = BitConverter.ToInt32(data, shift);
            shift += sizeof(int);
            message.Id = BitConverter.ToInt32(data, shift);
            shift += sizeof(int);
            int nameSize = BitConverter.ToInt32(data, shift);
            shift += sizeof(int);
            message.Name = Encoding.UTF8.GetString(data, shift, nameSize);
            shift += nameSize;

            List<byte[]> payload = new List<byte[]>();
            while (shift < data.Length)
            {
                int payloadSize = BitConverter.ToInt32(data, shift);
                shift += sizeof(int);
                byte[] payloadElement = new byte[payloadSize];
                Array.Copy(data, shift, payloadElement, 0, payloadSize);
                payload.Add(payloadElement);
                shift += payloadSize;
            }

            message.Payload = payload.ToArray();

            return message;
        }

        public byte[] Serialize(RawMessage msg)
        {
            byte[][] data = new byte[4 + msg.Payload.Length * 2][];

            byte[] status = BitConverter.GetBytes(msg.Status);
            byte[] id = BitConverter.GetBytes(msg.Id);
            byte[] name = Encoding.UTF8.GetBytes(msg.Name);
            byte[] nameSize = BitConverter.GetBytes(name.Length);

            data[0] = status;
            data[1] = id;
            data[2] = nameSize;
            data[3] = name;

            for (int i = 0; i < msg.Payload.Length; i++)
            {
                data[i * 2 + 4] = BitConverter.GetBytes(msg.Payload[i].Length);
                data[i * 2 + 5] = msg.Payload[i];
            }

            // This can be optimized if needed by manually creating
            // an array and using the fast Array.copy api
            return data.SelectMany(d => d).ToArray();
        }
    }
}
