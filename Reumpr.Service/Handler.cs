using System;
using System.IO;

namespace tetryds.Reumpr.Service
{
    public class Handler
    {
        public Guid Requestor { get; }
        public bool IsOpen { get; private set; }

        public void Reply(RawMessage response)
        {

        }

        public void Close()
        {

        }
    }
}
