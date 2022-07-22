using System.Collections.Generic;

namespace tetryds.Reumpr
{
    public interface IMessageDelimiter
    {
        int DelimiterSize { get; }
        void CheckDelimiters(byte[] data, int count, List<int> delimiterIndexes);
    }
}
