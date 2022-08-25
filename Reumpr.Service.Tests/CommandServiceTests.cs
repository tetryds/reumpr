using System;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using tetryds.Reumpr.Service;

namespace Reumpr.Service.Tests
{
    [TestClass]
    public class CommandServiceTests
    {
        [TestMethod]
        public void InvokeCommand()
        {
            ParameterParser parser = new ParameterParser(null, null);
            CommandService service = new CommandService(parser);

            CommandTest testModule = new CommandTest();
            bool invoked = false;
            testModule.TestInvoked += () => invoked = true;

            const string MethodName = "Test";

            Command command = new Command(testModule, typeof(CommandTest).GetMethod(MethodName));
            service.RegisterCommand(MethodName, command);

            RawMessage rawMessage = new RawMessage()
            {
                Payload = new byte[1][] { Encoding.UTF8.GetBytes(MethodName) }
            };
            Handler handler = new Handler(Guid.Empty, rawMessage);

            service.Invoke(handler);

            Assert.IsTrue(invoked);
            Assert.IsFalse(handler.IsOpen);
        }

        [TestMethod]
        public void InvokeCommandParameters()
        {
            ParameterParser parser = new ParameterParser(null, null);
            CommandService service = new CommandService(parser);

            CommandTest testModule = new CommandTest();

            int invokedI = default(int);
            string invokedS = default(string);
            float invokedF = default(float);
            testModule.TestParametersInvoked += (a, b, c) => { invokedI = a; invokedS = b; invokedF = c; };

            const string MethodName = "TestParameters";

            Command command = new Command(testModule, typeof(CommandTest).GetMethod(MethodName));
            service.RegisterCommand(MethodName, command);

            int expectedI = 10;
            string expectedS = "Bom dia";
            float expectedF = 12.4f;
            byte[][] payload = new byte[4][];
            payload[0] = Encoding.UTF8.GetBytes(MethodName);
            payload[1] = parser.Serialize(expectedI);
            payload[2] = parser.Serialize(expectedS);
            payload[3] = parser.Serialize(expectedF);

            RawMessage rawMessage = new RawMessage() { Payload = payload };
            Handler handler = new Handler(Guid.Empty, rawMessage);

            service.Invoke(handler);

            Assert.AreEqual(expectedI, invokedI);
            Assert.AreEqual(expectedS, invokedS);
            Assert.AreEqual(expectedF, invokedF);
            Assert.IsFalse(handler.IsOpen);
        }

        [TestMethod]
        public void InvokeCommandParameterAndReturn()
        {
            ParameterParser parser = new ParameterParser(null, null);
            CommandService service = new CommandService(parser);

            CommandTest testModule = new CommandTest();

            int invokedI = default(int);
            string invokedS = default(string);
            float invokedF = default(float);
            testModule.TestParametersInvoked += (a, b, c) => { invokedI = a; invokedS = b; invokedF = c; };

            const string MethodName = "TestParameterAndReturn";

            Command command = new Command(testModule, typeof(CommandTest).GetMethod(MethodName));
            service.RegisterCommand(MethodName, command);

            string message = "Bom dia";
            byte[][] payload = new byte[2][];
            payload[0] = Encoding.UTF8.GetBytes(MethodName);
            payload[1] = parser.Serialize(message);

            RawMessage rawMessage = new RawMessage() { Payload = payload };
            Handler handler = new Handler(Guid.Empty, rawMessage);

            object ret = default(object);
            MessageStatus status = default(MessageStatus);
            handler.Replied += (r, s) => { ret = r; status = s; };

            service.Invoke(handler);

            Assert.AreEqual(message, ret);
            Assert.AreEqual(MessageStatus.OkClose, status);
            Assert.IsFalse(handler.IsOpen);
        }

        [TestMethod]
        public void InvokeCommandWrongParameters()
        {
            ParameterParser parser = new ParameterParser(null, null);
            CommandService service = new CommandService(parser);

            CommandTest testModule = new CommandTest();

            const string MethodName = "TestParameters";

            Command command = new Command(testModule, typeof(CommandTest).GetMethod(MethodName));
            service.RegisterCommand(MethodName, command);

            string message = "Bom dia";
            byte[][] payload = new byte[2][];
            payload[0] = Encoding.UTF8.GetBytes(MethodName);
            payload[1] = parser.Serialize(message);

            RawMessage rawMessage = new RawMessage() { Payload = payload };
            Handler handler = new Handler(Guid.Empty, rawMessage);

            Assert.ThrowsException<CommandException>(() => service.Invoke(handler));
        }

        [TestMethod]
        public void InvokeCommandWrongUrl()
        {
            ParameterParser parser = new ParameterParser(null, null);
            CommandService service = new CommandService(parser);

            CommandTest testModule = new CommandTest();

            const string MethodName = "TestParameters";

            Command command = new Command(testModule, typeof(CommandTest).GetMethod(MethodName));
            service.RegisterCommand(MethodName, command);

            string message = "Bom dia";
            byte[][] payload = new byte[2][];
            payload[0] = Encoding.UTF8.GetBytes("IncorrectMethodName");
            payload[1] = parser.Serialize(message);

            RawMessage rawMessage = new RawMessage() { Payload = payload };
            Handler handler = new Handler(Guid.Empty, rawMessage);

            Assert.ThrowsException<CommandException>(() => service.Invoke(handler));
        }

        [TestMethod]
        public void InvokeCommandMissingUrl()
        {
            ParameterParser parser = new ParameterParser(null, null);
            CommandService service = new CommandService(parser);

            CommandTest testModule = new CommandTest();

            const string MethodName = "TestParameters";

            Command command = new Command(testModule, typeof(CommandTest).GetMethod(MethodName));
            service.RegisterCommand(MethodName, command);

            byte[][] payload = new byte[0][];

            RawMessage rawMessage = new RawMessage() { Payload = payload };
            Handler handler = new Handler(Guid.Empty, rawMessage);

            Assert.ThrowsException<CommandException>(() => service.Invoke(handler));
        }
    }
}
