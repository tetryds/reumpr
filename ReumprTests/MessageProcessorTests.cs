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
    public class MessageProcessorTests
    {
        MessageProcessor<string> processor;
        UTF8Parser parser;

        [TestInitialize]
        public void SetUp()
        {
            parser = new UTF8Parser();
            MarkerFinder delimiter = new MarkerFinder(parser.Serialize("ZZ"));
            processor = new MessageProcessor<string>(parser, delimiter);
        }

        [TestMethod]
        public void ProcessSingle()
        {
            byte[] data = parser.Serialize("Bom DiaZZ");
            List<string> messages = new List<string>();
            processor.GetMessages(data, data.Length, messages);

            List<string> expected = new List<string> { "Bom Dia" };

            CollectionAssert.AreEqual(expected, messages);
        }

        [TestMethod]
        public void ProcessMultiple()
        {
            byte[] data = parser.Serialize("Bom DiaZZHolaZZQue TalZZ");
            List<string> messages = new List<string>();
            processor.GetMessages(data, data.Length, messages);

            List<string> expected = new List<string> { "Bom Dia", "Hola", "Que Tal" };

            CollectionAssert.AreEqual(expected, messages);
        }

        [TestMethod]
        public void ProcessSplit()
        {
            byte[] data1 = parser.Serialize("Bom ");
            byte[] data2 = parser.Serialize("DiaZZ");

            List<string> messages = new List<string>();
            List<string> expected = new List<string>();

            processor.GetMessages(data1, data1.Length, messages);
            CollectionAssert.AreEqual(expected, messages);

            expected = new List<string> { "Bom Dia" };
            processor.GetMessages(data2, data2.Length, messages);
            CollectionAssert.AreEqual(expected, messages);
        }

        [TestMethod]
        public void ProcessSplitMany()
        {
            byte[] data1 = parser.Serialize("Bom ");
            byte[] data2 = parser.Serialize("DiaZZHola");
            byte[] data3 = parser.Serialize("ZZQue TalZZ");

            List<string> messages = new List<string>();
            List<string> expected = new List<string>();

            processor.GetMessages(data1, data1.Length, messages);
            CollectionAssert.AreEqual(expected, messages);

            expected = new List<string> { "Bom Dia" };
            processor.GetMessages(data2, data2.Length, messages);
            CollectionAssert.AreEqual(expected, messages);

            expected = new List<string> { "Hola", "Que Tal" };
            processor.GetMessages(data3, data3.Length, messages);
            CollectionAssert.AreEqual(expected, messages);
        }

        [TestMethod]
        public void ProcessSingleNegativeIndex()
        {
            byte[] data1 = parser.Serialize("Bom DiaZ");
            byte[] data2 = parser.Serialize("Z");

            List<string> messages = new List<string>();
            List<string> expected = new List<string>();

            processor.GetMessages(data1, data1.Length, messages);
            CollectionAssert.AreEqual(expected, messages);

            expected = new List<string> { "Bom Dia" };
            processor.GetMessages(data2, data2.Length, messages);
            CollectionAssert.AreEqual(expected, messages);
        }

        [TestMethod]
        public void ProcessMultipleNegativeIndex()
        {
            byte[] data1 = parser.Serialize("Bom DiaZ");
            byte[] data2 = parser.Serialize("ZHolaZ");
            byte[] data3 = parser.Serialize("ZQue TalZZ");

            List<string> messages = new List<string>();
            List<string> expected = new List<string>();

            processor.GetMessages(data1, data1.Length, messages);
            CollectionAssert.AreEqual(expected, messages);

            expected = new List<string> { "Bom Dia" };
            processor.GetMessages(data2, data2.Length, messages);
            CollectionAssert.AreEqual(expected, messages);

            expected = new List<string> { "Hola", "Que Tal" };
            processor.GetMessages(data3, data3.Length, messages);
            CollectionAssert.AreEqual(expected, messages);
        }
    }
}
