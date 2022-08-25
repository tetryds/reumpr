using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace tetryds.Reumpr.Service
{
    public class Command
    {
        readonly object Obj;
        readonly MethodInfo Method;

        // TODO: Create actual url definition
        public string Name => Method.Name;
        public ParameterInfo[] Parameters { get; }

        public Command(object obj, MethodInfo mInfo)
        {
            Obj = obj;
            Method = mInfo;
            Parameters = mInfo.GetParameters();
        }

        public object Invoke(object[] parameters)
        {
            return Method.Invoke(Obj, parameters);
        }
    }
}
