using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using tetryds.Reumpr;
using tetryds.Reumpr.Service;

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

            Guid id = gateway.ConnectTo("localhost", Port);

            int recv = 0;
            Thread listener = new Thread(() =>
            {
                while (true)
                {
                    (Guid remote, RawMessage msg) = gateway.GetMessage();
                    Console.WriteLine($"Recv {recv}, {DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond}");
                    recv++;
                    Console.WriteLine(msg.Id);
                    Console.WriteLine(msg.Name);
                    Console.WriteLine(msg.Status);
                    Console.WriteLine(parser.Parse(msg.Payload[0], typeof(float)));
                }
            });
            listener.IsBackground = true;
            listener.Start();

            int sent = 0;
            while (true)
            {
                float entry1 = float.Parse(Console.ReadLine());
                float entry2 = float.Parse(Console.ReadLine());
                if (entry1 == 0) break;

                RawMessage message = new RawMessage()
                {
                    Id = 10,
                    Name = "Command",
                    Status = MessageStatus.Ok,
                    Payload = new byte[][] { parser.Serialize("Sum"), parser.Serialize(entry1), parser.Serialize(entry2) }
                };

                Console.WriteLine($"Sent {sent}, {DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond}");
                gateway.SendMessage(id, message);
                sent++;
            }
        }

        private static MessageProcessor<RawMessage> GetProcessor()
        {
            RawMessageParser parser = new RawMessageParser();
            SizeDelimiter delimiter = new SizeDelimiter();
            return new MessageProcessor<RawMessage>(parser, delimiter);
        }
    }
}
