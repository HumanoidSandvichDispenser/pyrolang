using System;
using System.Collections.Generic;
using System.Text;

namespace pyrolang2.Interpreter
{
    public class InterpreterException : Exception
    {
        public int Line { get; set; }
        public int Column { get; set; }

        public InterpreterException(string message, int line, int col) : base(message)
        {
            Line = line;
            Column = col;
        }
    }
}
