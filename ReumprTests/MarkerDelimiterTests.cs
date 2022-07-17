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
            byte[] data1 = new byte[] { 0xFF, 0xAA, 0xFF, 0xAA, 0xFF, 0xAA, 0xFF, 0xAA, 0xFF, 0xAA, 0xFF};
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
        public void ShiftBuffer()
        {
            byte[] buffer = new byte[] { 0xFF, 0xAA, 0xBB, 0xCC, 0x00, 0x11, 0x22 };
            byte[] expected = new byte[] { 0x00, 0x11, 0x22, 0xCC, 0x00, 0x11, 0x22 };

            MarkerDelimiter.ShiftBuffer(buffer, 4);

            CollectionAssert.AreEqual(expected, buffer);
        }
    }
}
