using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace tetryds.Reumpr
{
    public static class SocketServerFactory
    {
        public static TcpListener Listen(int port, Action<TcpClient> onConnect)
        {
            if (onConnect == null) throw new ArgumentNullException(nameof(onConnect));

            TcpListener listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            listener.BeginAcceptTcpClient(DoAccept, new ConnectState(listener, onConnect));
            return listener;
        }

        private static void DoAccept(IAsyncResult ar)
        {
            ConnectState state = (ConnectState)ar.AsyncState;
            TcpListener listener = state.Listener;
            TcpClient client = listener.EndAcceptTcpClient(ar);

            listener.BeginAcceptTcpClient(DoAccept, state);
            state.OnConnect(client);
        }

        private class ConnectState
        {
            public TcpListener Listener { get; }
            public Action<TcpClient> OnConnect { get; }

            public ConnectState(TcpListener listener, Action<TcpClient> onConnect)
            {
                Listener = listener;
                OnConnect = onConnect;
            }
        }
    }
}
