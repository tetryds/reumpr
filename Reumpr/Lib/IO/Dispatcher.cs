using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Threading;

namespace tetryds.Reumpr
{
    public class Dispatcher<T> : IDisposable
    {
        readonly ConcurrentDictionary<Guid, TcpClient> clientMap;
        readonly BlockingCollection<(Guid, T)> outbound;
        readonly CancellationTokenSource canceller;
        readonly Thread worker;
        readonly IMessageParser<T> messageParser;
        readonly byte[] delimiter;

        public event Action<Exception> ErrorOcurred;
        public event Action<Exception> CrashOcurred;

        public Dispatcher(IMessageParser<T> messageParser, byte[] delimiter)
        {
            this.messageParser = messageParser ?? throw new ArgumentNullException(nameof(messageParser));
            this.delimiter = delimiter ?? throw new ArgumentNullException(nameof(delimiter));

            clientMap = new ConcurrentDictionary<Guid, TcpClient>();
            outbound = new BlockingCollection<(Guid, T)>();
            canceller = new CancellationTokenSource();

            worker = new Thread(DoDispatch)
            {
                Priority = ThreadPriority.BelowNormal,
                Name = "Reumpr Dispatcher",
                IsBackground = true,
            };
        }

        public void Start()
        {
            worker.Start();
        }

        public bool RegisterTcpClient(Guid id, TcpClient client)
        {
            return clientMap.TryAdd(id, client);
        }

        public void SendMessage(Guid id, T msg)
        {
            if (!worker.IsAlive)
                throw new DispatcherException("Cannot send message, dispatcher has not been started");
            outbound.Add((id, msg));
        }

        private void DoDispatch()
        {
            CancellationToken cancel = canceller.Token;

            try
            {
                while (!cancel.IsCancellationRequested)
                {
                    (Guid id, T msg) = outbound.Take(cancel);
                    if (clientMap.TryGetValue(id, out TcpClient client))
                    {
                        // TODO: This currently does not support message size delimiter
                        // because the delimiter is always sent after and knows nothing about the message
                        // maybe message parser should have a delimiter embedded
                        byte[] data = messageParser.Serialize(msg);
                        NetworkStream stream = client.GetStream();
                        stream.Write(data, 0, data.Length);
                        stream.Write(delimiter, 0, delimiter.Length);
                    }
                    else
                    {
                        ErrorOcurred?.Invoke(new ClientNotRegisteredException(id));
                    }
                }
            }
            catch (OperationCanceledException) { /* it was me! */ }
            catch (Exception e)
            {
                CrashOcurred?.Invoke(e);
            }
        }

        public void Dispose()
        {
            canceller.Cancel();
        }
    }
}
