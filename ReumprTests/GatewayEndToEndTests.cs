﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using tetryds.Reumpr;
using tetryds.Reumpr.Collections;
using tetryds.Reumpr.Tests.Tools;

namespace tetryds.Reumpr.Tests
{
    [TestClass]
    public class GatewayEndToEndTests
    {
        private const string Delimiter = "##";

        List<IDisposable> disposables = new List<IDisposable>();

        [TestMethod]
        public void ConnectAndCommunicateToSelf()
        {
            const int Port = 35648;
            const int BufferSize = 4096;
            const int MsgCount = 100;
            const int RecvTimeoutMs = 100;

            Gateway<string> gateway = new Gateway<string>(GetProcessor, BufferSize);
            disposables.Add(gateway);

            gateway.Start();
            gateway.ListenTo(Port);
            Guid serverId = gateway.ConnectTo("localhost", Port);

            string[] sent = new string[MsgCount];

            for (int i = 0; i < MsgCount; i++)
            {
                string msg = Path.GetRandomFileName();

                gateway.SendMessage(serverId, msg);
                sent[i] = msg;
            }

            List<string> received = new List<string>(MsgCount);
            while (received.Count < MsgCount && gateway.TryGetMessage(out Package<string> item, RecvTimeoutMs))
            {
                received.Add(item.Msg);
            }

            Assert.AreEqual(MsgCount, received.Count);
            CollectionAssert.AreEqual(sent, received);
        }

        [TestCleanup]
        public void Teardown()
        {
            disposables.ForEach(d => d.Dispose());
        }

        private MessageProcessor<string> GetProcessor()
        {
            UTF8Parser parser = new UTF8Parser();
            //MarkerDelimiter delimiter = new MarkerDelimiter(parser.Serialize(Delimiter));
            SizeDelimiter delimiter = new SizeDelimiter();
            return new MessageProcessor<string>(parser, delimiter);
        }
    }
}
