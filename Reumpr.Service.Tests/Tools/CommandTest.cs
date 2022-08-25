using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;

namespace tetryds.Reumpr.Service
{
    [ReumprModule]
    public class CommandTest
    {
        public event Action TestInvoked;
        public event Action<int, string, float> TestParametersInvoked;

        [Command]
        public void Test()
        {
            TestInvoked?.Invoke();
        }

        [Command]
        public void TestParameters(int a, string b, float c)
        {
            TestParametersInvoked?.Invoke(a, b, c);
        }

        [Command]
        public string TestParameterAndReturn(string msg)
        {
            return msg;
        }
    }
}
