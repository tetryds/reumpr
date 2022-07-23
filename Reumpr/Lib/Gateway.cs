using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Threading;
using tetryds.Reumpr.Collections;

namespace tetryds.Reumpr
{
    public class Gateway<T> : IDisposable
    {
        readonly Dispatcher<T> dispatcher;
        readonly Receiver<T> receiver;

        ConcurrentDictionary<int, TcpListener> listeners;
        BlockingCollection<Package<T>> received;

        public event Action<Guid> ClientDisconnected;
        public event Action<Exception> ErrorOcurred;

        public Gateway(Func<MessageProcessor<T>> messageProcessorProvider, int recvBufferSize)
        {
            if (messageProcessorProvider == null) throw new ArgumentNullException(nameof(messageProcessorProvider));

            listeners = new ConcurrentDictionary<int, TcpListener>();
            received = new BlockingCollection<Package<T>>();

            dispatcher = new Dispatcher<T>(messageProcessorProvider);
            dispatcher.CrashOcurred += ErrorOcurred;

            receiver = new Receiver<T>(messageProcessorProvider, recvBufferSize);
            receiver.MessageReceived += received.Add;

            receiver.ReadErrorOcurred += (e) =>
            {
                RemoveClient(e.Id);
                ClientDisconnected?.Invoke(e.Id);
            };

            receiver.ClientDisconnected += c =>
            {
                RemoveClient(c);
                ClientDisconnected?.Invoke(c);
            };
        }

        public void Start()
        {
            dispatcher.Start();
        }

        public void SendMessage(Guid id, T msg)
        {
            dispatcher.SendMessage(id, msg);
        }

        public bool TryGetMessage(out Package<T> package) => received.TryTake(out package);
        public bool TryGetMessage(out Package<T> package, int timeoutMs) => received.TryTake(out package, timeoutMs);
        public bool TryGetMessage(out Package<T> package, int timeoutMs, CancellationToken cancellationToken) => received.TryTake(out package, timeoutMs, cancellationToken);
        public Package<T> GetMessage() => received.Take();
        public Package<T> GetMessage(CancellationToken cancellationToken) => received.Take(cancellationToken);

        public Guid ConnectTo(string hostname, int port)
        {
            TcpClient client = new TcpClient(hostname, port);
            Guid guid = Guid.NewGuid();
            RegisterClient(guid, client);
            return guid;
        }

        public void ListenTo(int port)
        {
            // Lock on listeners to allow ListenTo to be called from any thread.
            lock (listeners)
            {
                if (listeners.ContainsKey(port))
                    throw new ListenException(port, $"Already listening to port '{port}'");
                TcpListener listener = SocketServerFactory.Listen(port, RegisterNewClient, HandleListenerError);
                listeners.TryAdd(port, listener);
            }
        }

        public void StopListening(int port)
        {
            if (!listeners.TryGetValue(port, out TcpListener listener))
                throw new ListenException(port, $"Not listening to port '{port}'");

            listener.Stop();
            RemoveListener(port);
        }

        private void RegisterNewClient(TcpClient client)
        {
            RegisterClient(Guid.NewGuid(), client);
        }

        private void RegisterClient(Guid id, TcpClient client)
        {
            dispatcher.RegisterTcpClient(id, client);
            receiver.ListenTcpClient(id, client);
        }

        private void RemoveClient(Guid id)
        {
            if (dispatcher.TryRemoveTcpClient(id, out TcpClient client))
                client.Close();
        }

        private void HandleListenerError(ListenException listenException)
        {
            RemoveListener(listenException.Port);
            ErrorOcurred?.Invoke(listenException);
        }

        private void RemoveListener(int port)
        {
            listeners.TryRemove(port, out TcpListener _);
        }

        public void Dispose()
        {
            dispatcher.Dispose();
        }
    }
}
