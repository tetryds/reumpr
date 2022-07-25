using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using tetryds.Reumpr;

namespace tetryds.Reumpr.Tests
{
    [TestClass]
    public class SizeDelimiterTests
    {
        [TestMethod]
        public void FindDelimitersSingle()
        {
            byte[] msg = new byte[] { 0x00, 0x08, 0x00, 0x00, 0x00, 0x00, 0xFF, 0x00, 0x00, 0x00 };

            SizeDelimiter sizeDelimiter = new SizeDelimiter();
            (byte[] delimiter, _) = sizeDelimiter.GetDelimiter(msg);
            
            List<byte> data = new List<byte>();
            data.AddRange(delimiter);
            data.AddRange(msg);

            List<int> expectedIndexes = new List<int>() { msg.Length + delimiter.Length + 1 };

            List<int> indexes = new List<int>();
            sizeDelimiter.CheckDelimiters(data.ToArray(), data.Count, indexes);

            CollectionAssert.AreEqual(expectedIndexes, indexes);
        }

        [TestMethod]
        public void FindDelimitersMultiple()
        {
            SizeDelimiter sizeDelimiter = new SizeDelimiter();
            List<byte> data = new List<byte>();
            List<int> expectedIndexes = new List<int>();
            for (int i = 0; i < 4; i++)
            {
                byte[] msg = new byte[] { 0x00, 0x08, 0x00, 0x00, 0x00, 0x00, 0xFF, 0x00, 0x00, 0x00 };
                (byte[] delimiter, _) = sizeDelimiter.GetDelimiter(msg);
                data.AddRange(delimiter);
                data.AddRange(msg);
                expectedIndexes.Add((i + 1) * (msg.Length + delimiter.Length) + 1);
            }


            List<int> indexes = new List<int>();
            sizeDelimiter.CheckDelimiters(data.ToArray(), data.Count, indexes);

            CollectionAssert.AreEqual(expectedIndexes, indexes);
        }
    }
}
