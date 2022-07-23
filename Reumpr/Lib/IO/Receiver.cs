using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using tetryds.Reumpr.Collections;

namespace tetryds.Reumpr
{
    public class Receiver<T>
    {
        readonly Func<MessageProcessor<T>> messageProcessorProvider;
        readonly int bufferSize;
        ConcurrentHash<Guid> liveClients;

        public event Action<Package<T>> MessageReceived;

        public event Action<Guid> ClientDisconnected;
        public event Action<ClientReceiveException> ReadErrorOcurred;

        public Receiver(Func<MessageProcessor<T>> messageProcessorProvider, int bufferSize)
        {
            this.messageProcessorProvider = messageProcessorProvider ?? throw new ArgumentNullException(nameof(messageProcessorProvider));
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
            MessageProcessor<T> messageProcessor = messageProcessorProvider();
            ReceiveState receiveState = new ReceiveState(id, clientStream, messageProcessor, buffer);
            clientStream.BeginRead(buffer, 0, bufferSize, DoListen, receiveState);
        }

        private void DoListen(IAsyncResult ar)
        {
            ReceiveState receiveState = (ReceiveState)ar.AsyncState;
            Guid id = receiveState.Id;
            NetworkStream stream = receiveState.Stream;
            byte[] buffer = receiveState.Buffer;
            MessageProcessor<T> messageProcessor = receiveState.messageProcessor;

            List<T> messages = new List<T>();

            try
            {
                int read = stream.EndRead(ar);

                messageProcessor.GetMessages(buffer, read, messages);
                foreach (T message in messages)
                {
                    MessageReceived?.Invoke(new Package<T>(id, message));
                }

                stream.BeginRead(buffer, 0, bufferSize, DoListen, receiveState);
            }
            catch (IOException)
            {
                liveClients.Remove(id);
                ClientDisconnected?.Invoke(id);
            }
            catch (Exception e)
            {
                liveClients.Remove(id);
                ReadErrorOcurred?.Invoke(new ClientReceiveException(id, "Error listening for data", e));
            }
        }

        private class ReceiveState
        {
            public readonly Guid Id;
            public readonly NetworkStream Stream;
            public readonly MessageProcessor<T> messageProcessor;
            public readonly byte[] Buffer;

            public ReceiveState(Guid id, NetworkStream stream, MessageProcessor<T> messageProcessor, byte[] buffer)
            {
                Id = id;
                Stream = stream;
                this.messageProcessor = messageProcessor;
                Buffer = buffer;
            }
        }
    }
}
