using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using tetryds.Reumpr;
using tetryds.Reumpr.Tests.Tools;

namespace tetryds.Reumpr.Tests
{
    [TestClass]
    public class SizeDelimiterTests
    {
        [TestMethod]
        public void FindDelimitersSingle()
        {
            byte[] msg = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A };

            SizeDelimiter sizeDelimiter = new SizeDelimiter();
            byte[] delimiter = sizeDelimiter.GetDelimiter(msg);

            List<byte> data = new List<byte>();
            data.AddRange(delimiter);
            data.AddRange(msg);

            List<int> expectedIndexes = new List<int>() { msg.Length + delimiter.Length };

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
                byte[] msg = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A };
                byte[] delimiter = sizeDelimiter.GetDelimiter(msg);
                data.AddRange(delimiter);
                data.AddRange(msg);
                expectedIndexes.Add((i + 1) * (msg.Length + delimiter.Length));
            }

            List<int> indexes = new List<int>();
            sizeDelimiter.CheckDelimiters(data.ToArray(), data.Count, indexes);

            CollectionAssert.AreEqual(expectedIndexes, indexes);
        }

        [TestMethod]
        public void FindDelimitersChunked()
        {
            SizeDelimiter sizeDelimiter = new SizeDelimiter();
            List<byte> data = new List<byte>();
            for (int i = 0; i < 4; i++)
            {
                byte[] msg = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A };
                byte[] delimiter = sizeDelimiter.GetDelimiter(msg);
                data.AddRange(delimiter);
                data.AddRange(msg);
            }

            List<List<byte>> chunked = data.ChunkBy(13);
            List<int> expectedIndexes = new List<int> { 1, 2, 3, 4 };

            List<int> indexes = new List<int>();
            for (int i = 0; i < chunked.Count; i++)
            {
                List<int> chunkIndexes = new List<int>();
                List<byte> chunk = chunked[i];
                sizeDelimiter.CheckDelimiters(chunk.ToArray(), chunk.Count, chunkIndexes);
                indexes.AddRange(chunkIndexes);
            }
            //TODO: Fix partially read messages
            CollectionAssert.AreEqual(expectedIndexes, indexes);
        }
    }
}
