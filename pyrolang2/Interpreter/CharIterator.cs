using System;
using System.Collections.Generic;
using System.Text;

namespace pyrolang2.Interpreter
{
    public class CharIterator : Iterator<char>
    {
        public CharIterator(char[] chars) : base(chars)
        {

        }
    }
}
