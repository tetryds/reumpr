using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Threading;
using tetryds.Reumpr.Collections;

namespace tetryds.Reumpr
{
    public class Dispatcher<T> : IDisposable
    {
        readonly MessageProcessor<T> messageProcessor;

        readonly ConcurrentDictionary<Guid, TcpClient> clientMap;
        readonly BlockingCollection<Package<T>> outbound;
        readonly CancellationTokenSource canceller;
        readonly Thread worker;

        public event Action<MissingClientException> MissingClient;
        public event Action<Exception> CrashOcurred;

        public Dispatcher(Func<MessageProcessor<T>> messageProcessorProvider)
        {
            messageProcessor = messageProcessorProvider?.Invoke() ?? throw new ArgumentNullException(nameof(messageProcessorProvider));

            clientMap = new ConcurrentDictionary<Guid, TcpClient>();
            outbound = new BlockingCollection<Package<T>>();
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

        public bool TryRemoveTcpClient(Guid id, out TcpClient client)
        {
            return clientMap.TryRemove(id, out client);
        }

        public void SendMessage(Guid id, T msg)
        {
            if (!worker.IsAlive)
                throw new ConnectionException("Cannot send message, dispatcher has not been started");
            outbound.Add(new Package<T>(id, msg));
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
                        NetworkStream networkStream = client.GetStream();
                        byte[][] data = messageProcessor.GetAllBytes(msg);
                        for (int i = 0; i < data.Length; i++)
                        {
                            networkStream.Write(data[i], 0, data[i].Length);
                        }
                    }
                    else
                    {
                        MissingClient?.Invoke(new MissingClientException(id));
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
