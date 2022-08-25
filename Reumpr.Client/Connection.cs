//using System;
//using System.Collections.Concurrent;
//using System.Threading;
//using tetryds.Reumpr;

//namespace Reumpr.Client
//{
//    public class Connection
//    {
//        readonly ConcurrentDictionary<string, Action<Handler>> actionMap;
//        readonly Gateway<RawMessage> gateway;

//        CancellationTokenSource tokenSource;

//        ParameterParser parser;

//        public ServerApp(Gateway<RawMessage> gateway, ParameterParser parser)
//        {
//            this.gateway = gateway;
//            this.parser = parser;
//            actionMap = new ConcurrentDictionary<string, Action<Handler>>();

//            tokenSource = new CancellationTokenSource();
//        }
//    }
//}
