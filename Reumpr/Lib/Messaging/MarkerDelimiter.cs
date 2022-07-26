using System;
using System.Collections.Generic;

namespace tetryds.Reumpr
{
    public class MarkerDelimiter : IMessageDelimiter
    {
        //Working without partial overflow overlap repeating
        readonly byte[] delimiter;
        readonly int length;

        readonly byte[] overflowBuffer;
        int overflowLength;
        int overflowFill;

        public int DelimiterSize => delimiter.Length;
        public DelimiterPos DelimiterPos => DelimiterPos.After;

        public MarkerDelimiter(byte[] delimiter)
        {
            this.delimiter = delimiter ?? throw new ArgumentNullException(nameof(delimiter));
            if (delimiter.Length == 0) throw new ArgumentException("Parameter cannot be empty", nameof(delimiter));

            length = delimiter.Length;

            overflowBuffer = new byte[Math.Max(1, length - 1)];
            overflowLength = overflowBuffer.Length;
            //Overflow fill starts at length to prevent initial buffer from matching
            overflowFill = overflowLength;
        }

        public byte[] GetDelimiter(byte[] message)
        {
            return delimiter;
        }

        public void CheckDelimiters(byte[] data, int count, List<int> delimiterIndexes)
        {
            if (count > data.Length) throw new ArgumentException("Count cannot be greater than data length");
            delimiterIndexes.Clear();

            // Since the buffer is always a single byte smaller than the delimiter, we can start from scratch every time
            int delimiterIndex = 0;

            int start = overflowFill - overflowLength;
            // Iterate and read both buffers together as if they were one big buffer
            for (int i = start; i < count; i++)
            {
                byte current;
                if (i < 0)
                    current = overflowBuffer[overflowLength + i];
                else
                    current = data[i];

                if (current != delimiter[delimiterIndex])
                {
                    if (delimiterIndex > 0)
                    {
                        i -= delimiterIndex;
                        delimiterIndex = 0;
                    }
                    // Nothing found, add it to overflow fill
                    overflowFill = Math.Max(0, overflowFill - 1);
                }
                else
                {
                    delimiterIndex++;
                    // Match found
                    if (delimiterIndex == length)
                    {
                        // Match found, adjust overflow fill to prevent it from spilling to the next marker check
                        int distanceToEnd = count - delimiterIndex;

                        overflowFill = Math.Min(count, overflowLength);

                        delimiterIndexes.Add(i - length + 1);
                        delimiterIndex = 0;
                    }
                    else
                    {
                        // Nothing found, add it to overflow fill
                        overflowFill = Math.Max(0, overflowFill - 1);
                    }
                }
            }

            ShiftOverflowBuffer(data, count);
            // Marked msg always starts at 0
        }

        private void ShiftOverflowBuffer(byte[] data, int count)
        {
            //If count is smaller than overflow length, shift to make room for the upcoming data
            if (overflowLength > count)
            {
                ShiftBuffer(overflowBuffer, count);
            }
            int toShift = Math.Max(0, overflowLength - count);

            //Check how much data is going to be filled on the overflow array
            int toFill = overflowLength - toShift;

            //Fill overflow with trailing data from the data buffer
            for (int i = toFill; i > 0; i--)
            {
                overflowBuffer[overflowLength - i] = data[count - i];
            }
        }

        public static void ShiftBuffer(byte[] buffer, int shift)
        {
            Array.ConstrainedCopy(buffer, shift, buffer, 0, buffer.Length - shift);
        }
    }
}
