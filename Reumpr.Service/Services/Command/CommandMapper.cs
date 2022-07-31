using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace tetryds.Reumpr.Service
{
    public class CommandMapper
    {
        public List<Command> GetCommands(object obj, BindingFlags flags)
        {
            return obj.GetType().GetMethods(flags)
                .Where(m => m.GetCustomAttribute<CommandAttribute>() != null)
                .Select(m => new Command(obj, m))
                .ToList();
        }
    }
}
