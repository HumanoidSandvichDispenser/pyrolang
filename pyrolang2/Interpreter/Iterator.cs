using System;
using System.Collections.Generic;
using System.Text;

namespace pyrolang2.Interpreter
{
    public class Iterator<T> where T : struct
    {
        public int Line { get; private set; } = 1;
        public int Column { get; private set; } = 0;

        private int Index { get; set; } = -1;
        private List<T> Elements { get; set; }

        public Iterator(T[] elements)
        {
            Elements = new List<T>(elements);
        }

        public T GetNext(int offset = 0)
        {
            if (Index + offset + 1 < Elements.Count)
            {
                return Elements[Index + 1];
            }
            return default;
        }

        public T MoveNext()
        {
            T next = GetNext();
            Index++;
            if (next is char)
            {
                char cNext = (char)Convert.ChangeType(next, typeof(char));
                if (cNext == '\n')
                {
                    Line++;
                    Column = 1;
                }
                else
                {
                    Column++;
                }
            }
            return next;
        }
    }
}
