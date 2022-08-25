using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace tetryds.Reumpr.Service
{
    public class ModuleFinder
    {
        public const BindingFlags MatchAll = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

        public List<Type> GetModuleTypes(Assembly assembly)
        {
            return assembly.GetTypes().Where(a => a.IsDefined(typeof(ReumprModuleAttribute))).ToList();
        }

        public List<object> GetInstances(List<Type> types)
        {
            return types.Select(t => Activator.CreateInstance(t)).ToList();
        }

        public List<Command> GetCommands(object obj, BindingFlags flags)
        {
            return obj.GetType().GetMethods(flags)
                .Where(m => m.GetCustomAttribute<CommandAttribute>() != null)
                .Select(m => new Command(obj, m))
                .ToList();
        }
    }
}
