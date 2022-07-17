using System;
using System.Collections.Generic;

namespace tetryds.Reumpr
{
    public class MarkerDelimiter : IMessageDelimiter
    {
        //Working without partial overflow overlap repeating
        readonly byte[] delimiter;
        readonly int length;
        int delimiterIndex = 0;

        readonly byte[] overflowBuffer;
        int overflowLength;
        int overflowIndex = 0;

        public MarkerDelimiter(byte[] delimiter)
        {
            this.delimiter = delimiter;
            length = delimiter.Length;

            overflowBuffer = new byte[Math.Max(1, length)];
            overflowLength = overflowBuffer.Length;
            //Overflow starts empty
            overflowIndex = overflowLength;
        }

        public void CheckDelimiters(byte[] data, int count, List<int> delimiterIndexes)
        {
            delimiterIndexes.Clear();
            for (int i = overflowLength - overflowIndex + 1; i < overflowLength + count - 1; i++)
            {
                byte current;
                if (i >= overflowLength - 1)
                    current = data[i - overflowLength + 1];
                else
                    current = overflowBuffer[i];

                if (current != delimiter[delimiterIndex])
                {
                    //Roll back to account for partial matches when delimiter has repeating pattern
                    if (delimiterIndex > 0)
                    {
                        i -= delimiterIndex - 1;
                        delimiterIndex = 0;
                    }
                }

                if (current == delimiter[delimiterIndex])
                {
                    delimiterIndex++;
                    if (delimiterIndex == length)
                    {
                        //TODO: Remember to reset overflow index to prevent matching more than you should
                        delimiterIndexes.Add(i - length + 1 - overflowLength + 1);
                        delimiterIndex = 0;
                        overflowIndex = overflowLength;
                    }
                }
                //Increment overflow index but clamp at buffer size
                overflowIndex = Math.Max(overflowIndex - 1, 0);

            }

            //There should be enough room for data within the overflow buffer
            //If count is bigger than overflow, it should be filled by the trailing data
            //It it is smaller, it should be shifted to make room for the data
            //Compute how many bytes to fill on the overflow buffer
            //Shift overflow to make room for data, update overflow index

            //If count is smaller than overflow length, shift to make room for the data
            int toShift = Math.Max(0, overflowLength - count);
            for (int i = 0; i < overflowLength - toShift; i++)
            {
                overflowBuffer[i] = overflowBuffer[i + toShift];
            }

            //Check how much data is going to be filled on the overflow array
            int toFill = overflowLength - toShift;

            //Fill overflow with trailing data from the data buffer
            for (int i = toFill - 1; i > 0; i--)
            {
                overflowBuffer[overflowLength - i] = data[count - i];
            }

        }

        public static void ShiftBuffer(byte[] buffer, int shift)
        {
            for (int i = 0; i < buffer.Length - shift; i++)
            {
                buffer[i] = buffer[i + shift];
            }
        }
    }
}
