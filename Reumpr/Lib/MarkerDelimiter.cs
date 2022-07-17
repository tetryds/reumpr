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

        public MarkerDelimiter(byte[] delimiter)
        {
            this.delimiter = delimiter ?? throw new ArgumentNullException(nameof(delimiter));
            if (delimiter.Length == 0) throw new ArgumentException("Parameter cannot be empty", nameof(delimiter));

            length = delimiter.Length;

            overflowBuffer = new byte[Math.Max(1, length - 1)];
            overflowLength = overflowBuffer.Length;
            //Overflow starts empty
        }

        public void CheckDelimiters(byte[] data, int count, List<int> delimiterIndexes)
        {
            if (count > data.Length) throw new ArgumentException("Count cannot be greater than data length");
            delimiterIndexes.Clear();

            delimiterIndex = 0;

            for (int i = -overflowLength; i < count; i++)
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
                }
                else
                {
                    delimiterIndex++;
                    if (delimiterIndex == length)
                    {
                        delimiterIndexes.Add(i - length + 1);
                        delimiterIndex = 0;
                    }
                }
            }

            ShiftOverflowBuffer(data, count);
        }

        private void ShiftOverflowBuffer(byte[] data, int count)
        {
            //If count is smaller than overflow length, shift to make room for the data
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

        public static void IterateCombinedArrays(byte[] array1, byte[] array2, int start, int count, Action<int, byte> callback)
        {
            if (start > array1.Length) throw new ArgumentException("Start cannot be bigger than first array length");
            if (array1.Length + array2.Length < start + count) throw new ArgumentException($"Attempting to iterate more items than available");

            int index = start;
            for (int i = start; i < array1.Length; i++)
            {
                callback(index, array1[i]);
                index++;
                if (index - start == count) return;
            }

            for (int i = 0; i < array2.Length; i++)
            {
                callback(index, array2[i]);
                index++;
                if (index - start == count) return;
            }
        }

        public static void ShiftBuffer(byte[] buffer, int shift)
        {
            Array.ConstrainedCopy(buffer, shift, buffer, 0, buffer.Length - shift);
        }
    }
}
