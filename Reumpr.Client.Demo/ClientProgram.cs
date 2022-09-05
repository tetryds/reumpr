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
    class ClientProgram
    {
        private const int Port = 35464;

        static void Main(string[] args)
        {
            //With default gateway
            //With size delimiter (can be default)
            Gateway<RawMessage> gateway = new Gateway<RawMessage>(GetProcessor, 4096);
            gateway.Connected += i => Console.WriteLine($"Connected {i}");
            gateway.Start();
            ParameterParser parser = new ParameterParser(null, null);

            Connection connection = new Connection(gateway, parser);
            connection.ConnectTo("localhost", Port);

            List<Handler> handlers = new List<Handler>();

            DemoClient client = new DemoClient(connection, parser, 100);

            Random random = new Random(10);


            int count = 0;
            void LogCount()
            {
                Console.WriteLine(count);
                count = 0;
            }

            Timer timer = new Timer(o => LogCount(), null, 0, 1_000);
            while (true)
            {
                //float value1 = float.Parse(Console.ReadLine());
                float value1 = 981267.423f;
                float value2 = 2983746.123f;

                float sum = client.RpcRawSum(value1, value2);

                Console.WriteLine($"Result: {sum}");
                count++;
            }

            //while (true)
            //{
            //    if (Console.ReadLine() != "") break;

            //    Console.Write($"Result: {client.RpcGetClientCount()}");
            //}
        }

        private static MessageProcessor<RawMessage> GetProcessor()
        {
            RawMessageParser parser = new RawMessageParser();
            SizeDelimiter delimiter = new SizeDelimiter();
            return new MessageProcessor<RawMessage>(parser, delimiter);
        }
    }
}
