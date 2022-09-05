using System;
using System.Collections.Concurrent;
using System.Threading;
using tetryds.Reumpr;
using tetryds.Reumpr.Collections;
using tetryds.Reumpr.Common;

namespace tetryds.Reumpr.Client
{
    public class Router
    {
        readonly Gateway<RawMessage> gateway;

        Random randomizer;
        ConcurrentDictionary<int, Handler> handlerMap;

        CancellationTokenSource tokenSource;
        Thread route;

        public event Action<Exception> OnError;

        public Router(Gateway<RawMessage> gateway)
        {
            this.gateway = gateway;

            randomizer = new Random();
            handlerMap = new ConcurrentDictionary<int, Handler>();
        }

        public void Start()
        {
            tokenSource = new CancellationTokenSource();
            route = new Thread(DoReadMessages)
            {
                IsBackground = true,
                Priority = ThreadPriority.BelowNormal
            };
            route.Start();
        }

        public Handler SendMessage(Guid remoteId, RawMessage message, bool autoId)
        {
            if (autoId)
                message.Id = randomizer.Next();

            Handler handler = new Handler(message.Id, message);
            if (!handlerMap.TryAdd(message.Id, handler))
                throw new MessageIdException(message.Id, $"Cannot send message with duplicated id '{message.Id}'");

            gateway.SendMessage(remoteId, message);

            return handler;
        }

        private void DoReadMessages()
        {
            CancellationToken token = tokenSource.Token;
            try
            {
                while (!token.IsCancellationRequested)
                {
                    Package<RawMessage> package = gateway.GetMessage(token);
                    if (!handlerMap.TryRemove(package.Message.Id, out Handler handler))
                        OnError.Invoke(new MessageIdException(package.Message.Id, $"Received message with unknown id '{package.Message.Id}'"));

                    handler.AddResponse(package.Message);
                }
            }
            catch (OperationCanceledException) { }
        }
    }
}
