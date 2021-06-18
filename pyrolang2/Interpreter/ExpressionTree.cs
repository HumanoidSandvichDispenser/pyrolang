using System;
using System.Collections.Generic;
using System.Text;

namespace pyrolang2.Interpreter
{
    public enum ExpressionType
    {
        None,
        Value,
        Keyword,
        Operation,
        Call,
        Declaration,
        Assignment,
        FuncDefinition,
        FuncImplementation,
    }

    public class ExpressionTree
    {
        public ExpressionType Type { get; set; }
        public object[] Children { get; private set; }
        public int Line { get; }
        public int Column { get; }

        public ExpressionTree(ExpressionType type, object[] children, int line, int column)
        {
            Type = type;
            Children = children;
            Line = line;
            Column = column;
        }
    }
}
