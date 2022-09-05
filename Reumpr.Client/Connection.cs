using System;
using System.Collections.Concurrent;
using System.Threading;
using tetryds.Reumpr;
using tetryds.Reumpr.Common;

namespace tetryds.Reumpr.Client
{
    public class Connection
    {
        readonly ConcurrentDictionary<string, Action<Handler>> actionMap;
        readonly Gateway<RawMessage> gateway;

        ParameterParser parser;

        Router router;

        Guid remoteId;

        public Connection(Gateway<RawMessage> gateway, ParameterParser parser)
        {
            this.gateway = gateway;
            this.parser = parser;

            router = new Router(gateway);
            actionMap = new ConcurrentDictionary<string, Action<Handler>>();
        }

        public void ConnectTo(string hostname, int port)
        {
            remoteId = gateway.ConnectTo(hostname, port);
            router.Start();
        }

        public Handler SendCommand(string url, params object[] objs)
        {
            RawMessage message = new RawMessage()
            {
                Name = "Command",
                Status = MessageStatus.Ok,
                Payload = ParseCommandPayload(url, objs)
            };

            return router.SendMessage(remoteId, message, true);
        }

        private byte[][] ParseCommandPayload(string url, object[] objs)
        {
            object[] payloadObjs = new object[1 + objs.Length];
            payloadObjs[0] = url;
            objs.CopyTo(payloadObjs, 1);
            return parser.SerializeMany(payloadObjs);
        }
    }
}
