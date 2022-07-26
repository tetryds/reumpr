using System.Collections.Generic;

namespace tetryds.Reumpr
{
    public interface IMessageDelimiter
    {
        int DelimiterSize { get; }
        DelimiterPos DelimiterPos { get; }
        byte[] GetDelimiter(byte[] message);
        void CheckDelimiters(byte[] data, int count, List<int> delimiterIndexes);
    }
}
