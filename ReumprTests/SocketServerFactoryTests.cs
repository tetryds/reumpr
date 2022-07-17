using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using tetryds.Reumpr;

namespace tetryds.ReumprTests
{
    [TestClass]
    public class SocketServerFactoryTests
    {
        List<IDisposable> disposables = new List<IDisposable>();

        [TestMethod]
        public void CreateListenerAndConnect()
        {
            const int Port = 3000;
            CallbackObjReader<TcpClient> socketReader = new CallbackObjReader<TcpClient>();

            SocketServerFactory.Listen(Port, socketReader.SetValue);

            TcpClient client = new TcpClient();
            client.Connect("localhost", Port);

            socketReader.Wait(10);
            Assert.IsNotNull(socketReader.Value);
        }

        [TestMethod]
        public void CreateListenerAndConnectMultipleClients()
        {
            const int PollingPeriod = 1;
            const int Port = 3001;
            const int ClientCount = 10;

            CallbackObjCollection<TcpClient> connectedClients = new CallbackObjCollection<TcpClient>(PollingPeriod);

            SocketServerFactory.Listen(Port, connectedClients.Add);

            for (int i = 0; i < ClientCount; i++)
            {
                TcpClient client = new TcpClient();
                client.Connect("localhost", Port);
            }

            Assert.IsTrue(connectedClients.Wait(ClientCount, 10));
            Assert.AreEqual(connectedClients.GetCount(), ClientCount);
        }

        [TestCleanup]
        public void Teardown()
        {
            disposables.ForEach(d => d.Dispose());
        }
    }
}
