using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace tetryds.Reumpr
{
    public static class SocketServerFactory
    {
        public static TcpListener Listen(int port, Action<TcpClient> onConnect, Action<ListenException> onError)
        {
            if (onConnect == null) throw new ArgumentNullException(nameof(onConnect));

            TcpListener listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            listener.BeginAcceptTcpClient(DoAccept, new ConnectState(listener, onConnect, onError, port));
            return listener;
        }

        private static void DoAccept(IAsyncResult ar)
        {
            ConnectState state = (ConnectState)ar.AsyncState;
            TcpListener listener = state.Listener;
            try
            {
                TcpClient client = listener.EndAcceptTcpClient(ar);

                listener.BeginAcceptTcpClient(DoAccept, state);
                state.OnConnect(client);
            }
            catch (IOException) { /* It was me! */}
            catch (Exception e)
            {
                ListenException listenException = new ListenException(state.Port, "Error accepting", e);
                state.OnError?.Invoke(listenException);
            }
        }

        private class ConnectState
        {
            public TcpListener Listener { get; }
            public Action<TcpClient> OnConnect { get; }
            public Action<ListenException> OnError { get; }
            public int Port { get; }

            public ConnectState(TcpListener listener, Action<TcpClient> onConnect, Action<ListenException> onError, int port)
            {
                Listener = listener;
                OnConnect = onConnect;
                OnError = onError;
                Port = port;
            }
        }
    }
}
