using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace tetryds.Reumpr.Tests.Tools
{
    public class UTF8Parser : IMessageParser<string>
    {
        public string Parse(byte[] data)
        {
            return Encoding.UTF8.GetString(data);
        }

        public byte[] Serialize(string msg)
        {
            return Encoding.UTF8.GetBytes(msg);
        }
    }
}
