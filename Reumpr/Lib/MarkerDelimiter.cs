using System.Collections.Generic;

namespace tetryds.Reumpr
{
    public class MarkerDelimiter : IMessageDelimiter
    {
        //Working without partial overflow overlap repeating
        readonly byte[] delimiter;
        readonly int length;
        int delimiterIndex = 0;

        public MarkerDelimiter(byte[] delimiter)
        {
            this.delimiter = delimiter;
            length = delimiter.Length;
        }

        public void CheckDelimiters(byte[] data, int count, List<int> delimiterIndexes)
        {
            delimiterIndexes.Clear();
            for (int i = 0; i < count; i++)
            {
                if (data[i] != delimiter[delimiterIndex])
                {
                    //Roll back to account for partial matches when delimiter has repeating pattern
                    if (delimiterIndex > 0)
                    {
                        i -= delimiterIndex - 1;
                        delimiterIndex = 0;
                    }
                }

                if (data[i] == delimiter[delimiterIndex])
                {
                    delimiterIndex++;
                    if (delimiterIndex == length)
                    {
                        delimiterIndexes.Add(i - length + 1);
                        delimiterIndex = 0;
                    }
                }
            }
        }
    }
}
