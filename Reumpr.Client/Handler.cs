using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using tetryds.Reumpr.Common;
using tetryds.Reumpr.Common.Exceptions;

namespace tetryds.Reumpr.Client
{
    public class Handler
    {
        public int Id { get; }
        public RawMessage Original { get; }
        public List<RawMessage> Received { get; }
        public bool IsOpen { get; private set; } = true;

        ManualResetEvent closeEvent;

        public Handler(int id, RawMessage original)
        {
            Id = id;
            Original = original;
            Received = new List<RawMessage>();

            closeEvent = new ManualResetEvent(false);
        }

        public void AddResponse(RawMessage response)
        {
            Received.Add(response);
            if (response.Status != MessageStatus.Ok)
            {
                IsOpen = false;
                closeEvent.Set();
            }
        }

        public Handler Wait(int timeoutMs)
        {
            if (!closeEvent.WaitOne(timeoutMs))
                throw new Exception("Waiting timed out");
            if (Received[Received.Count - 1].Status == MessageStatus.Error)
                throw new Exception("Message error");

            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EnsureOpen()
        {
            if (!IsOpen)
                throw new Exception("Cannot perform operation, handler is closed");
        }

        public static bool WaitAll(IEnumerable<Handler> handlers, int timeoutMs)
        {
            CancellationTokenSource tokenSource = new CancellationTokenSource(timeoutMs);
            while (!tokenSource.Token.IsCancellationRequested && !handlers.All(h => !h.IsOpen)) { }
            return !tokenSource.IsCancellationRequested;
        }
    }
}
