using System;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using tetryds.Reumpr.Service;

namespace Reumpr.Service.Tests
{
    [TestClass]
    public class RawMessageParserTests
    {
        [TestMethod]
        public void SerializeDesserialize()
        {
            RawMessage message = new RawMessage
            {
                Id = 10,
                Name = "Bom dia",
                Status = 0,
                Payload = GeneratePayload(10)
            };

            RawMessageParser parser = new RawMessageParser();

            byte[] data = parser.Serialize(message);
            RawMessage deserialized = parser.Parse(data);
        }

        [TestMethod]
        public void SerializeDesserializeEmptyPayload()
        {
            RawMessage message = new RawMessage
            {
                Id = 10,
                Name = "Bom dia",
                Status = 0,
                Payload = GeneratePayload(0)
            };

            RawMessageParser parser = new RawMessageParser();

            byte[] data = parser.Serialize(message);
            RawMessage deserialized = parser.Parse(data);
        }

        private byte[][] GeneratePayload(int count)
        {
            byte[][] payload = new byte[count][];
            for (int i = 0; i < payload.Length; i++)
            {
                payload[i] = Encoding.UTF8.GetBytes(Path.GetRandomFileName());
            }
            return payload;
        }
    }
}
