using System;
using System.Collections.Generic;

namespace tetryds.Reumpr
{
    public enum SizeDelimiterState
    {
        ReadingSize,
        ReadingMessage
    }

    public class SizeDelimiter : IMessageDelimiter
    {
        const int BufferSize = sizeof(int);

        readonly byte[] sizeBuffer = new byte[BufferSize];
        int sizeIndex = 0;

        int bytesLeft = 0;

        SizeDelimiterState state = SizeDelimiterState.ReadingSize;

        public int DelimiterSize => sizeBuffer.Length;
        public DelimiterPos DelimiterPos => DelimiterPos.Before;

        public byte[] GetDelimiter(byte[] message)
        {
            return BitConverter.GetBytes(message.Length);
        }

        public void CheckDelimiters(byte[] data, int count, List<int> delimiterIndexes)
        {
            delimiterIndexes.Clear();
            int pos = 0;
            while (pos < count)
            {
                if (state == SizeDelimiterState.ReadingMessage)
                {
                    int slice = Math.Min(bytesLeft, count - pos);
                    //Compensate for i++
                    pos += slice;
                    bytesLeft -= slice;
                    if (bytesLeft == 0)
                    {
                        state = SizeDelimiterState.ReadingSize;
                        delimiterIndexes.Add(pos);
                    }
                }
                else if (state == SizeDelimiterState.ReadingSize)
                {
                    int toRead = Math.Min(BufferSize - sizeIndex, count - pos);
                    for (; pos < count && sizeIndex < BufferSize; sizeIndex++, pos++)
                    {
                        sizeBuffer[sizeIndex] = data[pos];
                    }

                    if (sizeIndex == BufferSize)
                    {
                        state = SizeDelimiterState.ReadingMessage;
                        sizeIndex = 0;
                        bytesLeft = BitConverter.ToInt32(sizeBuffer, 0);
                    }
                }
            }
        }
    }
}
