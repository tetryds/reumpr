using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;

namespace tetryds.Reumpr
{
    public class MessageProcessor<T>
    {
        readonly IMessageParser<T> messageParser;
        readonly IMessageDelimiter delimiter;

        List<int> delimiterIndexes;
        List<byte> bytesRead;

        public MessageProcessor(IMessageParser<T> messageParser, IMessageDelimiter delimiter)
        {
            this.messageParser = messageParser;
            this.delimiter = delimiter;

            delimiterIndexes = new List<int>();
            bytesRead = new List<byte>();
        }

        public byte[][] GetAllBytes(T message)
        {
            byte[] msg = messageParser.Serialize(message);
            (byte[] del, DelimiterPos pos) = delimiter.GetDelimiter(msg);

            byte[][] data = new byte[2][];
            if (pos == DelimiterPos.After)
            {
                data[0] = msg;
                data[1] = del;
            }
            else
            {
                data[0] = del;
                data[1] = msg;
            }
            return data;
        }

        public void GetMessages(byte[] buffer, int count, List<T> messages)
        {
            messages.Clear();
            int skip = delimiter.CheckDelimiters(buffer, count, delimiterIndexes);
            if (skip > 0)
            {
                Array.ConstrainedCopy(buffer, skip, buffer, 0, buffer.Length - skip);
                count -= skip;
            }

            int start = 0;
            foreach (int index in delimiterIndexes)
            {
                AdjustReadBytes(buffer, start, index);
                T message = messageParser.Parse(bytesRead.ToArray());
                messages.Add(message);
                bytesRead.Clear();
                start = index + delimiter.DelimiterSize;
            }

            AdjustReadBytes(buffer, start, count);
        }

        private void AdjustReadBytes(byte[] buffer, int start, int index)
        {
            if (index > 0)
            {
                for (int i = start; i < index; i++)
                {
                    bytesRead.Add(buffer[i]);
                }
            }
            else
            {
                int shift = -index;
                for (int i = 0; i < shift; i++)
                {
                    bytesRead.RemoveAt(bytesRead.Count - 1);
                }
            }
        }
    }
}
