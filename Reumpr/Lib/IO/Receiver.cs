using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using tetryds.Reumpr.Collections;

namespace tetryds.Reumpr
{
    public class Receiver<T>
    {
        readonly MessageProcessor<T> messageProcessor;
        readonly int bufferSize;
        ConcurrentHash<Guid> liveClients;

        public event Action<Guid, T> MessageReceived;

        public event Action<Guid, Exception> ReadErrorOcurred;

        public Receiver(MessageProcessor<T> messageProcessor, int bufferSize)
        {
            this.messageProcessor = messageProcessor ?? throw new ArgumentNullException(nameof(messageProcessor));
            this.bufferSize = bufferSize > 0 ? bufferSize : throw new ArgumentException("Buffer size has to be a positive non-zero number", nameof(bufferSize));

            liveClients = new ConcurrentHash<Guid>();
        }

        public bool ListenTcpClient(Guid id, TcpClient client)
        {
            if (liveClients.Contains(id)) return false;

            StartListener(id, client);
            liveClients.TryAdd(id);
            return true;
        }

        private void StartListener(Guid id, TcpClient client)
        {
            byte[] buffer = new byte[bufferSize];
            NetworkStream clientStream = client.GetStream();
            ReceiveState receiveState = new ReceiveState(id, clientStream, buffer);
            client.GetStream().BeginRead(buffer, 0, bufferSize, DoListen, receiveState);
        }

        private void DoListen(IAsyncResult ar)
        {
            ReceiveState receiveState = (ReceiveState)ar.AsyncState;
            Guid id = receiveState.Id;
            NetworkStream stream = receiveState.Stream;
            byte[] buffer = receiveState.Buffer;

            List<T> messages = new List<T>();

            try
            {
                int read = stream.EndRead(ar);

                messageProcessor.GetMessages(buffer, read, messages);
                foreach (T message in messages)
                {
                    Task.Run(() => MessageReceived?.Invoke(id, message));
                }

                stream.BeginRead(buffer, 0, bufferSize, DoListen, receiveState);
            }
            catch (Exception e)
            {
                //This kills the reader, so let's remove it from the live clients hash
                liveClients.Remove(id);
                ReadErrorOcurred?.Invoke(id, e);
            }
        }

        private class ReceiveState
        {
            public readonly Guid Id;
            public readonly NetworkStream Stream;
            public readonly byte[] Buffer;

            public ReceiveState(Guid id, NetworkStream stream, byte[] buffer)
            {
                Id = id;
                Stream = stream;
                Buffer = buffer;
            }
        }
    }
}
