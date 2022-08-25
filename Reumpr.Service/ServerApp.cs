using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using tetryds.Reumpr.Collections;

namespace tetryds.Reumpr.Service
{
    public class ServerApp
    {
        readonly ConcurrentDictionary<string, Action<Handler>> actionMap;
        readonly Gateway<RawMessage> gateway;

        CancellationTokenSource tokenSource;

        ParameterParser parser;

        public ServerApp(Gateway<RawMessage> gateway, ParameterParser parser)
        {
            this.gateway = gateway;
            this.parser = parser;
            actionMap = new ConcurrentDictionary<string, Action<Handler>>();

            tokenSource = new CancellationTokenSource();
        }

        public void Register(string name, Action<Handler> action)
        {
            if (!actionMap.TryAdd(name, action))
                throw new ArgumentException($"Service '{name}' is already registered", nameof(name));
        }

        public void Run()
        {
            CancellationToken token = tokenSource.Token;
            try
            {
                while (!token.IsCancellationRequested)
                {
                    if (gateway.TryGetMessage(out Package<RawMessage> package, Timeout.Infinite, token))
                    {
                        Handler handler = new Handler(package.Id, package.Message);
                        handler.Replied += (o, s) => SendMessage(o, s, handler);
                        try
                        {
                            ProcessRequest(handler);
                        }
                        catch (Exception exception)
                        {
                            handler.ReplyError(exception);
                        }
                    }
                }
            }
            catch (OperationCanceledException) { }
        }

        public void Cancel()
        {
            tokenSource.Cancel();
        }

        private void ProcessRequest(Handler handler)
        {
            string name = handler.Request.Name;
            if (!actionMap.TryGetValue(name, out Action<Handler> action))
                throw new ServiceException($"Unable to process request, service '{name}' not found");

            action.Invoke(handler);
        }

        private void SendMessage(object reply, MessageStatus status, Handler handler)
        {
            RawMessage message = handler.Request.CopyHeader();
            message.Payload = new byte[1][] { parser.Serialize(reply) };
            message.Status = status;

            gateway.SendMessage(handler.Id, message);
        }
    }
}
