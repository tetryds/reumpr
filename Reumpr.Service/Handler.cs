using System;
using System.IO;
using System.Runtime.CompilerServices;
using tetryds.Reumpr.Collections;

namespace tetryds.Reumpr.Service
{
    public class Handler
    {
        public Guid Id { get; }
        public RawMessage Request { get; }
        public bool IsOpen { get; private set; } = true;

        readonly object locker = new object();

        public event Action<object, MessageStatus> Replied;

        public Handler(Guid id, RawMessage message)
        {
            Id = id;
            Request = message;
        }

        public void Reply(object response)
        {
            lock (locker)
            {
                EnsureOpen();
                Replied?.Invoke(response, MessageStatus.Ok);
            }
        }

        public void ReplyClose(object response)
        {
            lock (locker)
            {
                EnsureOpen();
                Replied?.Invoke(response, MessageStatus.OkClose);
                IsOpen = false;
            }
        }

        public void ReplyError(Exception exception)
        {
            lock (locker)
            {
                EnsureOpen();
                Console.WriteLine(exception);
                Replied?.Invoke(exception.ToString(), MessageStatus.Error);
                IsOpen = false;
            }
        }

        public void Close()
        {
            lock (locker)
            {
                EnsureOpen();
                Replied?.Invoke(null, MessageStatus.Close);
                IsOpen = false;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EnsureOpen()
        {
            if (!IsOpen)
                throw new HandlerException("Cannot perform operation, handler is closed");
        }
    }
}
