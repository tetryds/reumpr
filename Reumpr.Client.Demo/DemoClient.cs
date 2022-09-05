using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using tetryds.Reumpr;
using tetryds.Reumpr.Client;
using tetryds.Reumpr.Common;

namespace Reumpr.Client.Demo
{
    class DemoClient
    {
        Connection connection;
        ParameterParser parser;
        int timeoutMs;

        public DemoClient(Connection connection, ParameterParser parser, int timeoutMs)
        {
            this.connection = connection;
            this.parser = parser;
            this.timeoutMs = timeoutMs;
        }

        public float RpcSum(float value1, float value2)
        {
            Handler handler = connection.SendCommand("Sum", value1, value2).Wait(timeoutMs);
            return parser.Parse<float>(handler.Received[0].Payload[0]);
        }

        public float RpcRawSum(float value1, float value2)
        {
            Handler handler = connection.SendCommand("RawSum", value1, value2).Wait(timeoutMs);
            return parser.Parse<float>(handler.Received[0].Payload[0]);
        }
    }
}
