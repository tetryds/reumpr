using System;
using System.Collections.Generic;

namespace tetryds.Reumpr
{
    public class SizeDelimiter : IMessageDelimiter
    {
        byte[] sizeBuffer;

        public int DelimiterSize => sizeof(int);

        // Must start with len
        int countLeft = 0;
        int readSizes = 0;

        bool firstRead = true;

        // How much of the size buffer has been read
        // How much is left for the message to be read

        public SizeDelimiter()
        {
            sizeBuffer = new byte[sizeof(int)];
        }

        public void CheckDelimiters(byte[] data, int count, List<int> delimiterIndexes)
        {
            if (count < 0) throw new ArgumentException("Argument cannot be lower than zero", nameof(count));
            if (delimiterIndexes is null) throw new ArgumentNullException(nameof(delimiterIndexes));
            if (count == 0) return;

            delimiterIndexes.Clear();

            if (countLeft > count)
            {
                countLeft -= count;
                return;
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    int remaining = count - i;
                    if (countLeft <= remaining)
                    {
                        int advance = Math.Min(countLeft, remaining);
                        i += advance;
                        countLeft -= advance;
                        remaining -= advance;
                    }

                    if (countLeft == 0)
                    {
                        // countLeft == 0 and readSizes == 0 -> assign delimiter, read sizes buffer
                        // countLeft == 0 and readSizes < DelimiterSize -> read sizes buffer
                        // countLeft == 0 and readSizes == DelimiterSize -> assign count left from read sizes buffer, reset read sizes
                        // countLeft > 0 -> skip "count left" bytes
                        if (readSizes == 0 && !firstRead)
                        {
                            delimiterIndexes.Add(i + 1);
                        }
                        firstRead = false;

                        if (remaining == 0) break;

                        if (readSizes < DelimiterSize)
                        {
                            sizeBuffer[readSizes++] = data[i];
                        }
                        if (readSizes == DelimiterSize)
                        {
                            readSizes = 0;
                            countLeft = BitConverter.ToInt32(sizeBuffer, 0);
                        }
                    }

                }
            }
        }

        public (byte[], DelimiterPos) GetDelimiter(byte[] message)
        {
            return (BitConverter.GetBytes(message.Length), DelimiterPos.Before);
        }
    }
}
