using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Reumpr.Service;
using tetryds.Reumpr;
using tetryds.Reumpr.Common;
using tetryds.Reumpr.Service;

namespace Reumpr.Service.Demo
{
    class ServerProgram
    {
        private const int Port = 35464;

        static void Main(string[] args)
        {
            //With default gateway
            //With size delimiter (can be default)
            Gateway<RawMessage> gateway = new Gateway<RawMessage>(GetProcessor, 4096);
            gateway.Connected += id => Console.WriteLine($"Connected {id}");
            gateway.Disconnected += id => Console.WriteLine($"Disconnected {id}");
            gateway.ErrorOcurred += e => Console.WriteLine(e);
            gateway.Start();

            ParameterParser parameterParser = new ParameterParser(null, null);
            ServerApp app = new ServerApp(gateway, parameterParser);

            //With default commands
            ModuleFinder finder = new ModuleFinder();

            List<Type> moduleTypes = finder.GetModuleTypes(typeof(ServerProgram).Assembly);
            List<object> instances = finder.GetInstances(moduleTypes);
            List<Command> commands = instances.SelectMany(i => finder.GetCommands(i, ModuleFinder.MatchAll)).ToList();
            Console.WriteLine(string.Join(", ", commands.Select(c => c.Name)));
            CommandService commandService = new CommandService(parameterParser);

            foreach (Command command in commands)
            {
                commandService.RegisterCommand(command.Name, command);
            }

            app.Register("Command", commandService.Invoke);

            gateway.ListenTo(Port);

            Thread runner = new Thread(app.Run);
            runner.Start();

            Console.ReadLine();

            app.Cancel();
            runner.Join();
        }

        private static MessageProcessor<RawMessage> GetProcessor()
        {
            RawMessageParser parser = new RawMessageParser();
            SizeDelimiter delimiter = new SizeDelimiter();
            return new MessageProcessor<RawMessage>(parser, delimiter);
        }
    }
}
