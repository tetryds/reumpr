using System.Collections.Generic;

namespace tetryds.Reumpr
{
    public interface IMessageDelimiter
    {
        void CheckDelimiters(byte[] data, int count, List<int> delimiterIndexes);
    }
}
