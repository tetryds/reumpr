using System;
using System.Net.Sockets;
using System.Threading;

namespace tetryds.Reumpr
{
    public class ClientHandler
    {
        NetworkStream client;

        public ClientHandler(NetworkStream client)
        {
            this.client = client;
            
        }
    }
}
