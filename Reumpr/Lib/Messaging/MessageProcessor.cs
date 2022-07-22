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

        public void GetMessages(byte[] buffer, int count, List<T> messages)
        {
            messages.Clear();
            delimiter.CheckDelimiters(buffer, count, delimiterIndexes);

            int start = 0;
            foreach (int index in delimiterIndexes)
            {
                ShiftBytesRead(buffer, start, index);
                T message = messageParser.Parse(bytesRead.ToArray());
                messages.Add(message);
                bytesRead.Clear();
                start = index + delimiter.DelimiterSize;
            }

            ShiftBytesRead(buffer, start, count);
        }

        private void ShiftBytesRead(byte[] buffer, int start, int index)
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
