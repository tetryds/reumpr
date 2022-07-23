using System.Collections.Generic;
using System.Threading;
using tetryds.Reumpr;

namespace tetryds.Reumpr.Tests.Tools
{
    public class DummyBeforeDelimiter : IMessageDelimiter
    {
        byte[] delimiter;

        public int DelimiterSize => delimiter.Length;

        public DummyBeforeDelimiter(byte[] delimiter)
        {
            this.delimiter = delimiter;
        }

        public void CheckDelimiters(byte[] data, int count, List<int> delimiterIndexes)
        {
            throw new System.NotImplementedException();
        }

        public (byte[], DelimiterPos) GetDelimiter(byte[] message)
        {
            return (delimiter, DelimiterPos.Before);
        }
    }
}
