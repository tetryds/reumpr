using System.Threading;

namespace tetryds.Reumpr.Tests.Tools
{
    public class CallbackObjReader<T>
    {
        public T Value { get; private set; }
        AutoResetEvent resetEvent = new AutoResetEvent(false);

        public void SetValue(T read)
        {
            Value = read;
            resetEvent.Set();
        }

        public void Reset()
        {
            Value = default(T);
        }

        public bool Wait(int timeoutMs)
        {
            return resetEvent.WaitOne(timeoutMs);
        }
    }
}
