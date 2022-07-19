using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using tetryds.Reumpr;

namespace tetryds.ReumprTests
{
    [TestClass]
    public class MarkerDelimiterTests
    {
        [TestMethod]
        public void FindDelimitersSingle()
        {
            byte[] delimiter = new byte[] { 0xFF };
            byte[] data = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0x00, 0x00, 0x00 };

            List<int> expectedIndexes = new List<int>() { 5 };
            MarkerDelimiter markerDelimiter = new MarkerDelimiter(delimiter);

            List<int> indexes = new List<int>();
            markerDelimiter.CheckDelimiters(data, data.Length, indexes);

            CollectionAssert.AreEqual(expectedIndexes, indexes);
        }

        [TestMethod]
        public void FindDelimitersMultiple()
        {
            byte[] delimiter = new byte[] { 0xFF };
            byte[] data = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0x00, 0xFF, 0x00, 0x00, 0x00, 0xFF };

            List<int> expectedIndexes = new List<int>() { 5, 7, 11 };
            MarkerDelimiter markerDelimiter = new MarkerDelimiter(delimiter);

            List<int> indexes = new List<int>();
            markerDelimiter.CheckDelimiters(data, data.Length, indexes);

            CollectionAssert.AreEqual(expectedIndexes, indexes);
        }

        [TestMethod]
        public void FindDelimitersMultiple2()
        {
            byte[] delimiter = new byte[] { 0x00 };
            byte[] data = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

            List<int> expectedIndexes = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 };
            MarkerDelimiter markerDelimiter = new MarkerDelimiter(delimiter);

            List<int> indexes = new List<int>();
            markerDelimiter.CheckDelimiters(data, data.Length, indexes);

            CollectionAssert.AreEqual(expectedIndexes, indexes);
        }

        [TestMethod]
        public void FindDelimitersMultipleSequential()
        {
            byte[] delimiter = new byte[] { 0xFF };
            byte[] data = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };

            List<int> expectedIndexes = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
            MarkerDelimiter markerDelimiter = new MarkerDelimiter(delimiter);

            List<int> indexes = new List<int>();
            markerDelimiter.CheckDelimiters(data, data.Length, indexes);

            CollectionAssert.AreEqual(expectedIndexes, indexes);
        }

        [TestMethod]
        public void FindDelimitersMultipleSequentialLong()
        {
            byte[] delimiter = new byte[] { 0xFF, 0xFF };
            byte[] data = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };

            List<int> expectedIndexes = new List<int>() { 0, 2, 4, 6, 8, 10 };
            MarkerDelimiter markerDelimiter = new MarkerDelimiter(delimiter);

            List<int> indexes = new List<int>();
            markerDelimiter.CheckDelimiters(data, data.Length, indexes);

            CollectionAssert.AreEqual(expectedIndexes, indexes);
        }

        [TestMethod]
        public void FindDelimitersLong()
        {
            byte[] delimiter = new byte[] { 0xFF, 0xAA };
            byte[] data = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0xAA, 0x00, 0x00 };

            List<int> expectedIndexes = new List<int>() { 5 };
            MarkerDelimiter markerDelimiter = new MarkerDelimiter(delimiter);

            List<int> indexes = new List<int>();
            markerDelimiter.CheckDelimiters(data, data.Length, indexes);

            CollectionAssert.AreEqual(expectedIndexes, indexes);
        }

        [TestMethod]
        public void FindDelimitersPartial()
        {
            byte[] delimiter = new byte[] { 0xFF, 0xAA };
            byte[] data = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0x00, 0xFF, 0xAA };

            List<int> expectedIndexes = new List<int>() { 7 };
            MarkerDelimiter markerDelimiter = new MarkerDelimiter(delimiter);

            List<int> indexes = new List<int>();
            markerDelimiter.CheckDelimiters(data, data.Length, indexes);

            CollectionAssert.AreEqual(expectedIndexes, indexes);
        }

        [TestMethod]
        public void FindDelimitersPartialOverlap()
        {
            byte[] delimiter = new byte[] { 0xFF, 0xAA };
            byte[] data = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0xAA };

            List<int> expectedIndexes = new List<int>() { 7 };
            MarkerDelimiter markerDelimiter = new MarkerDelimiter(delimiter);

            List<int> indexes = new List<int>();
            markerDelimiter.CheckDelimiters(data, data.Length, indexes);

            CollectionAssert.AreEqual(expectedIndexes, indexes);
        }

        [TestMethod]
        public void FindDelimitersPartialOverlap2()
        {
            byte[] delimiter = new byte[] { 0xFF, 0xAA, 0xBB };
            byte[] data = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0xAA, 0xFF, 0xAA, 0xBB };

            List<int> expectedIndexes = new List<int>() { 8 };
            MarkerDelimiter markerDelimiter = new MarkerDelimiter(delimiter);

            List<int> indexes = new List<int>();
            markerDelimiter.CheckDelimiters(data, data.Length, indexes);

            CollectionAssert.AreEqual(expectedIndexes, indexes);
        }

        [TestMethod]
        public void FindDelimitersPartialOverlapRepeating()
        {
            byte[] delimiter = new byte[] { 0xFF, 0xAA, 0xFF, 0xBB };
            byte[] data = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0xAA, 0xFF, 0xAA, 0xFF, 0xBB };

            List<int> expectedIndexes = new List<int>() { 8 };
            MarkerDelimiter markerDelimiter = new MarkerDelimiter(delimiter);

            List<int> indexes = new List<int>();
            markerDelimiter.CheckDelimiters(data, data.Length, indexes);

            CollectionAssert.AreEqual(expectedIndexes, indexes);
        }

        [TestMethod]
        public void FindDelimitersPartialOverlapRepeating2()
        {
            byte[] delimiter = new byte[] { 0xFF, 0xAA, 0xFF, 0xAA, 0xFF, 0xBB };
            byte[] data = new byte[] { 0xFF, 0xAA, 0xFF, 0xAA, 0xFF, 0xAA, 0xFF, 0xAA, 0xFF, 0xAA, 0xFF, 0xAA, 0xFF, 0xBB };

            List<int> expectedIndexes = new List<int>() { 8 };
            MarkerDelimiter markerDelimiter = new MarkerDelimiter(delimiter);

            List<int> indexes = new List<int>();
            markerDelimiter.CheckDelimiters(data, data.Length, indexes);

            CollectionAssert.AreEqual(expectedIndexes, indexes);
        }

        [TestMethod]
        public void FindDelimitersPartialOverlapRepeating3()
        {
            byte[] delimiter = new byte[] { 0xFF, 0xAA, 0xFF, 0xAA, 0xFF, 0xBB };
            byte[] data = new byte[] { 0xFF, 0xAA, 0xFF, 0xAA, 0xFF, 0xAA, 0xFF, 0xAA, 0xFF, 0xAA, 0xFF, 0xAA, 0xFF, 0xAA, 0xFF, 0xBB };

            List<int> expectedIndexes = new List<int>() { 10 };
            MarkerDelimiter markerDelimiter = new MarkerDelimiter(delimiter);

            List<int> indexes = new List<int>();
            markerDelimiter.CheckDelimiters(data, data.Length, indexes);

            CollectionAssert.AreEqual(expectedIndexes, indexes);
        }

        [TestMethod]
        public void FindDelimitersMissing()
        {
            byte[] delimiter = new byte[] { 0xFF };
            byte[] data = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

            List<int> expectedIndexes = new List<int>();
            MarkerDelimiter markerDelimiter = new MarkerDelimiter(delimiter);

            List<int> indexes = new List<int>();
            markerDelimiter.CheckDelimiters(data, data.Length, indexes);

            CollectionAssert.AreEqual(expectedIndexes, indexes);
        }

        [TestMethod]
        public void FindDelimitersPartialOverflow()
        {
            byte[] delimiter = new byte[] { 0xFF, 0xAA };
            byte[] data1 = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0x00, 0xFF };
            byte[] data2 = new byte[] { 0xAA, 0x00, 0x00, 0x00, 0xFF, 0xAA };

            List<int> expectedIndexes1 = new List<int>();
            List<int> expectedIndexes2 = new List<int>() { -1, 4 };
            MarkerDelimiter markerDelimiter = new MarkerDelimiter(delimiter);

            List<int> indexes = new List<int>();
            markerDelimiter.CheckDelimiters(data1, data1.Length, indexes);

            CollectionAssert.AreEqual(expectedIndexes1, indexes);

            markerDelimiter.CheckDelimiters(data2, data2.Length, indexes);
            CollectionAssert.AreEqual(expectedIndexes2, indexes);
        }

        [TestMethod]
        public void FindDelimitersPartialOverflowOverlapRepeating()
        {
            byte[] delimiter = new byte[] { 0xFF, 0xAA, 0xFF, 0xAA, 0xFF, 0xBB };
            byte[] data1 = new byte[] { 0xFF, 0xAA, 0xFF, 0xAA, 0xFF, 0xAA, 0xFF, 0xAA, 0xFF, 0xAA, 0xFF };
            byte[] data2 = new byte[] { 0xAA, 0xFF, 0xBB };

            List<int> expectedIndexes1 = new List<int>();
            List<int> expectedIndexes2 = new List<int>() { -3 };
            MarkerDelimiter markerDelimiter = new MarkerDelimiter(delimiter);

            List<int> indexes = new List<int>();
            markerDelimiter.CheckDelimiters(data1, data1.Length, indexes);

            CollectionAssert.AreEqual(expectedIndexes1, indexes);

            markerDelimiter.CheckDelimiters(data2, data2.Length, indexes);
            CollectionAssert.AreEqual(expectedIndexes2, indexes);
        }

        [TestMethod]
        public void FindDelimitersPartialOverflowOverlapRepeating2()
        {
            byte[] delimiter = new byte[] { 0xFF, 0xAA, 0xFF, 0xAA, 0xFF, 0xBB };
            byte[] data1 = new byte[] { 0xFF, 0xAA, 0xFF, 0xAA, 0xFF, 0xAA, 0xFF };
            byte[] data2 = new byte[] { 0xAA, 0xFF, 0xBB };

            List<int> expectedIndexes1 = new List<int>();
            List<int> expectedIndexes2 = new List<int>() { -3 };
            MarkerDelimiter markerDelimiter = new MarkerDelimiter(delimiter);

            List<int> indexes = new List<int>();
            markerDelimiter.CheckDelimiters(data1, data1.Length, indexes);

            CollectionAssert.AreEqual(expectedIndexes1, indexes);

            markerDelimiter.CheckDelimiters(data2, data2.Length, indexes);
            CollectionAssert.AreEqual(expectedIndexes2, indexes);
        }

        [TestMethod]
        public void FindDelimitersPartialOverflowOverlapRepeatingSplit()
        {
            //byte[] delimiter = new byte[] { 0xAA, 0xFF, 0xAA, 0xFF, 0xAA, 0xFF, 0xAA, 0xBB };
            //byte[] data1 = new byte[] { 0xAA, 0xFF, 0xAA, 0xFF, 0xAA, 0xFF, 0xAA, 0xFF };
            //byte[] data2 = new byte[] { 0xAA, 0xFF, 0xAA, 0xFF };
            //byte[] data3 = new byte[] { 0xAA, 0xBB, 0xAA, 0xFF, 0xAA, 0xFF, 0xAA, 0xFF };

            byte[] delimiter = new byte[] { 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D };
            byte[] data1 = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08 };
            byte[] data2 = new byte[] { 0x09, 0x0A, 0x0B, 0x0C };
            byte[] data3 = new byte[] { 0x0D, 0x0E, 0x0F, 0x10, 0x11, 0x12, 0x13, 0x14 };

            List<int> expectedIndexes1 = new List<int>();
            List<int> expectedIndexes2 = new List<int>();
            List<int> expectedIndexes3 = new List<int>() { -5 };
            MarkerDelimiter markerDelimiter = new MarkerDelimiter(delimiter);

            List<int> indexes = new List<int>();
            markerDelimiter.CheckDelimiters(data1, data1.Length, indexes);

            CollectionAssert.AreEqual(expectedIndexes1, indexes);

            markerDelimiter.CheckDelimiters(data2, data2.Length, indexes);
            CollectionAssert.AreEqual(expectedIndexes2, indexes);

            markerDelimiter.CheckDelimiters(data3, data3.Length, indexes);
            CollectionAssert.AreEqual(expectedIndexes3, indexes);
        }

        [TestMethod]
        public void FindDelimitersPartialOverflowOverlapRepeatingSplit2()
        {
            byte[] delimiter = new byte[] { 0xFF, 0xAA, 0xFF, 0xAA, 0xFF, 0xAA, 0xBB };
            byte[] data1 = new byte[] { 0xAA, 0xFF, 0xAA, 0xFF, 0xAA, 0xFF, 0xAA, 0xFF };
            byte[] data2 = new byte[] { 0xAA, 0xFF, 0xAA, 0xFF };
            byte[] data3 = new byte[] { 0xAA, 0xBB, 0xAA, 0xFF, 0xAA, 0xFF, 0xAA, 0xFF };

            List<int> expectedIndexes1 = new List<int>();
            List<int> expectedIndexes2 = new List<int>();
            List<int> expectedIndexes3 = new List<int>() { -5 };
            MarkerDelimiter markerDelimiter = new MarkerDelimiter(delimiter);

            List<int> indexes = new List<int>();
            markerDelimiter.CheckDelimiters(data1, data1.Length, indexes);

            CollectionAssert.AreEqual(expectedIndexes1, indexes);

            markerDelimiter.CheckDelimiters(data2, data2.Length, indexes);
            CollectionAssert.AreEqual(expectedIndexes2, indexes);

            markerDelimiter.CheckDelimiters(data3, data3.Length, indexes);
            CollectionAssert.AreEqual(expectedIndexes3, indexes);
        }

        [TestMethod]
        public void FindDelimitersPulverized()
        {
            byte[] delimiter = new byte[] { 0xAA, 0xAA };
            List<byte[]> data = new List<byte[]>
            {
                new byte[] { 0xAA, 0xFF },
                new byte[] { 0xAA, 0xBB},
                new byte[] { 0xAA, 0xFF },
                new byte[] { 0xAA, 0xBB},
                new byte[] { 0xAA, 0xFF },
                new byte[] { 0xAA, 0xBB},
                new byte[] { 0xAA, 0xFF },
                new byte[] { 0xAA, 0xBB},
                new byte[] { 0xAA, 0xAA },
                //TODO: Fix this scenario where a match has to fill the overflow fill to prevent the next byte array from matching
                new byte[] { 0xAA, 0xBB},
                new byte[] { 0xAA, 0xFF },
                new byte[] { 0xAA, 0xBB},
                new byte[] { 0xAA, 0xFF },
                new byte[] { 0xAA, 0xBB},
                new byte[] { 0xAA, 0xFF },
                new byte[] { 0xAA, 0xBB},
                new byte[] { 0xCC, 0xAA },
                new byte[] { 0xAA, 0xBB},
            };

            MarkerDelimiter markerDelimiter = new MarkerDelimiter(delimiter);
            List<List<int>> expectedIndexes = new List<List<int>>()
            {
                new List<int>(),
                new List<int>(),
                new List<int>(),
                new List<int>(),
                new List<int>(),
                new List<int>(),
                new List<int>(),
                new List<int>(),
                new List<int>(){ 0 },
                new List<int>(),
                new List<int>(),
                new List<int>(),
                new List<int>(),
                new List<int>(),
                new List<int>(),
                new List<int>(),
                new List<int>(),
                new List<int>(){ -1 },
            };

            List<int> indexes = new List<int>();

            for (int i = 0; i < data.Count; i++)
            {
                markerDelimiter.CheckDelimiters(data[i], data[i].Length, indexes);
                if (expectedIndexes[i].Count != indexes.Count)
                    Console.WriteLine(i);
                CollectionAssert.AreEqual(expectedIndexes[i], indexes);
            }
        }

        [TestMethod]
        public void FindDelimitersOverflowFill()
        {
            byte[] delimiter = new byte[] { 0xAA, 0xAA, 0xAA, 0xAA };
            List<byte[]> data = new List<byte[]>
            {
                new byte[] { 0xAA, 0xAA, 0xAA, 0xAA },
                new byte[] { 0xAA, 0xAA, 0xAA, 0xAA },
                new byte[] { 0xAA, 0xAA, 0xAA, 0xAA },
                new byte[] { 0xAA, 0xAA, 0xAA, 0xAA },
                new byte[] { 0xAA, 0xAA, 0xAA, 0xAA }
            };

            MarkerDelimiter markerDelimiter = new MarkerDelimiter(delimiter);
            List<List<int>> expectedIndexes = new List<List<int>>()
            {
                new List<int>(){ 0 },
                new List<int>(){ 0 },
                new List<int>(){ 0 },
                new List<int>(){ 0 },
                new List<int>(){ 0 }
            };

            List<int> indexes = new List<int>();

            for (int i = 0; i < data.Count; i++)
            {
                markerDelimiter.CheckDelimiters(data[i], data[i].Length, indexes);
                if (expectedIndexes[i].Count != indexes.Count)
                    Console.WriteLine(i);
                CollectionAssert.AreEqual(expectedIndexes[i], indexes, $"Index {i}");
            }
        }

        [TestMethod]
        public void FindDelimitersOverflowFill2()
        {
            byte[] delimiter = new byte[] { 0xAA, 0xAA, 0xAA, 0xAA };
            List<byte[]> data = new List<byte[]>
            {
                new byte[] { 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA },
                new byte[] { 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA },
                new byte[] { 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA },
                new byte[] { 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA },
                new byte[] { 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA }
            };

            MarkerDelimiter markerDelimiter = new MarkerDelimiter(delimiter);
            List<List<int>> expectedIndexes = new List<List<int>>()
            {
                new List<int>(){ 0 },
                new List<int>(){ -2, 2 },
                new List<int>(){ 0 },
                new List<int>(){ -2, 2 },
                new List<int>(){ 0 }
            };

            List<int> indexes = new List<int>();

            for (int i = 0; i < data.Count; i++)
            {
                markerDelimiter.CheckDelimiters(data[i], data[i].Length, indexes);
                if (expectedIndexes[i].Count != indexes.Count)
                    Console.WriteLine(i);
                CollectionAssert.AreEqual(expectedIndexes[i], indexes, $"Index {i}");
            }
        }

        [TestMethod]
        public void ShiftBuffer()
        {
            byte[] buffer = new byte[] { 0xFF, 0xAA, 0xBB, 0xCC, 0x00, 0x11, 0x22 };
            byte[] expected = new byte[] { 0x00, 0x11, 0x22, 0xCC, 0x00, 0x11, 0x22 };

            MarkerDelimiter.ShiftBuffer(buffer, 4);

            CollectionAssert.AreEqual(expected, buffer);
        }
    }
}
