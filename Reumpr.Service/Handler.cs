using System;
using System.IO;

namespace tetryds.Reumpr.Service
{
    public class Handler
    {
        public Guid Id { get; }
        public RawMessage Request { get; }
        public bool IsOpen { get; private set; }

        //TODO: maybe reply object instead of raw message
        public void Reply(RawMessage response)
        {

        }

        //TODO: maybe reply object instead of raw message
        public void ReplyClose(RawMessage response)
        {

        }

        public void Close()
        {

        }
    }
}
