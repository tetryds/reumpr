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

        long messagesRead = 0;

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
            byte[] del = delimiter.GetDelimiter(msg);
            DelimiterPos pos = delimiter.DelimiterPos;
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
            DelimiterPos pos = delimiter.DelimiterPos;
            //TODO: Move delimiter pos to a property and read from it both here and on get all bytes
            if (pos == DelimiterPos.After)
                GetMessagesAfter(buffer, count, messages);
            else
                GetMessagesBefore(buffer, count, messages);
        }

        public void GetMessagesBefore(byte[] buffer, int count, List<T> messages)
        {
            messages.Clear();
            delimiter.CheckDelimiters(buffer, count, delimiterIndexes);

            int start = 0;
            foreach (int index in delimiterIndexes)
            {
                AdjustReadBytes(buffer, start, index);
                bytesRead.RemoveRange(0, delimiter.DelimiterSize);
                T message = messageParser.Parse(bytesRead.ToArray());
                messagesRead++;
                messages.Add(message);
                bytesRead.Clear();
                start = index;
            }

            AdjustReadBytes(buffer, start, count);
        }

        public void GetMessagesAfter(byte[] buffer, int count, List<T> messages)
        {
            messages.Clear();
            delimiter.CheckDelimiters(buffer, count, delimiterIndexes);

            int start = 0;
            foreach (int index in delimiterIndexes)
            {
                AdjustReadBytes(buffer, start, index);
                T message = messageParser.Parse(bytesRead.ToArray());
                messagesRead++;
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
