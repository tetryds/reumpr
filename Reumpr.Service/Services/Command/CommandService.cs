﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using tetryds.Reumpr.Common;
using tetryds.Reumpr.Common.Exceptions;

namespace tetryds.Reumpr.Service
{
    public class CommandService
    {
        readonly ConcurrentDictionary<string, Command> commands = new ConcurrentDictionary<string, Command>();
        readonly ParameterParser parser;

        public CommandService(ParameterParser parser)
        {
            this.parser = parser;
        }

        public void RegisterCommand(string url, Command command)
        {
            if (!commands.TryAdd(url, command))
                throw new CommandException($"Cannot register command, url '{url}' already registered");
        }

        public void Invoke(ServerHandler handler)
        {
            RawMessage request = handler.Request;
            byte[][] payload = request.Payload;

            if (payload.Length < 1)
                throw new CommandException("Url not present in payload");

            string url = Encoding.UTF8.GetString(payload[0]);

            if (!commands.TryGetValue(url, out Command command))
                throw new CommandException($"Url not mapped '{url}'");

            ParameterInfo[] parameters = command.Parameters;

            if (payload.Length - 1 != parameters.Length)
                throw new CommandException($"Cannot invoke command, received '{payload.Length - 1}' parameters, expected '{parameters.Length}'");

            object[] parameterObjs = new object[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
            {
                // First entry is URL, parameters start at index 1
                parameterObjs[i] = parser.Parse(payload[i + 1], parameters[i].ParameterType);
            }

            object ret = command.Invoke(parameterObjs);
            handler.ReplyClose(ret);
        }
    }
}
