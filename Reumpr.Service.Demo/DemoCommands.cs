using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Reumpr.Service;
using tetryds.Reumpr;
using tetryds.Reumpr.Service;

namespace Reumpr.Service.Demo
{
    [ReumprModule]
    class DemoCommands
    {
        [Command]
        public string Echo(string value)
        {
            Console.WriteLine($"Echo {value}");
            return value;
        }

        [Command]
        public void Log(string value)
        {
            Console.WriteLine($"Log {value}");
        }

        [Command]
        public float Sum(float value1, float value2)
        {
            Console.WriteLine($"Sum {value1} + {value2}");
            return value1 + value2;
        }
    }
}
