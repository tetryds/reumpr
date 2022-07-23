//using System;
//using System.Collections.Generic;

//namespace tetryds.Reumpr
//{
//    public class IntSizeDelimiter : IMessageDelimiter
//    {
//        byte[] sizeBuffer;

//        public int DelimiterSize => sizeof(int);

//        // Must start with len
//        int countLeft = 0;
//        int readSizes = 0;

//        public IntSizeDelimiter()
//        {
//            sizeBuffer = new byte[sizeof(int)];
//        }

//        public void CheckDelimiters(byte[] data, int count, List<int> delimiterIndexes)
//        {
//            delimiterIndexes.Clear();

//            if (countLeft >= count)
//            {
//                countLeft -= count;
//                return;
//            }
//            else
//            {
//                //while(readSizes < DelimiterSize)
//                //{
//                //    int nextCount = countLeft + 1;
//                //    delimiterIndexes.Add(nextCount);
//                //    sizeBuffer[readSizes] = data[nextCount];
//                //}
//            }
//            //Read size, then read message, then read size, and so forth.
//        }

//        public (byte[], DelimiterPos) GetDelimiter(byte[] message)
//        {
//            return (BitConverter.GetBytes(message.Length), DelimiterPos.Before);
//        }
//    }
//}
