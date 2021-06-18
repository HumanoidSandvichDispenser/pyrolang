using System;
using System.Collections.Generic;
using System.Text;

namespace pyrolang2.Interpreter
{
    public class Node
    {
        public int Line { get; }
        public int Column { get; }

        public Node(int line, int column)
        {
            Line = line;
            Column = column;
        }

        public string ToString(int level)
        {
            string header = GetType().Name;
            string children = "";
            string indent = "└─";
            string whitespace = new string(' ', level * 2);
            var properties = GetType().GetProperties();
            foreach (var property in properties)
            {
                if (property.PropertyType == typeof(Node))
                {
                    Node node = property.GetValue(this, null) as Node;
                    children += $"{property.Name} = {node.ToString(level + 1)}\n";
                }
                else if (property.PropertyType == typeof(Token))
                {
                    //property.PropertyType.
                }
            }
            return "";
        }
    }

    public class TokenNode : Node
    {
        public Token ChildToken { get; }

        public TokenNode(Token childToken, int line, int column) : base(line, column)
        {
            ChildToken = childToken;
        }
    }

    public class KeywordNode : Node
    {
        public string Keyword { get; }

        public KeywordNode(string keyword, int line, int column) : base(line, column)
        {
            Keyword = keyword;
        }
    }

    public class FuncCallNode : Node
    {
        public List<Node> Arguments { get; }

        public FuncCallNode(List<Node> arguments, int line, int column) : base(line, column)
        {
            Arguments = arguments;
        }
    }

    public class FuncDefNode : Node
    {
        public List<Node> Parameters { get; }

        public FuncDefNode(List<Node> parameters, int line, int column) : base(line, column)
        {
            Parameters = parameters;
        }
    }

    public class FuncImplNode : Node
    {
        public FuncDefNode Definition { get; }
        public List<Node> Implementation { get; }

        public FuncImplNode(FuncDefNode definition, List<Node> implementation, int line, int column)
            : base(line, column)
        {
            Definition = definition;
            Implementation = implementation;
        }

    }

    public class OperationNode : Node
    {
        public Token Operation { get; }
        public Node Left { get; }
        public Node Right { get; }
        
        public OperationNode(Token operation, Node left, Node right, int line, int column)
            : base(line, column)
        {
            Operation = operation;
            Left = left;
            Right = right;
        }
    }

    public class DeclarationNode : Node
    {
        public string Type { get; }
        public string Identifier { get; }
        public Node Value { get; internal set; }
        
        public DeclarationNode(string type, string identifier, Node value, int line, int column)
            : base(line, column)
        {
            Type = type;
            Identifier = identifier;
            Value = value;
        }
    }

    public class AssignmentNode : Node
    {
        public string Identifier { get; }
        public Node Value { get; }

        public AssignmentNode(string identifier, Node value, int line, int column)
            : base(line, column)
        {
            Identifier = identifier;
            Value = value;
        }
    }

    public class ReturnNode : Node
    {
        public Node ReturnValue { get; }

        public ReturnNode(Node returnValue, int line, int column) : base(line, column)
        {
            ReturnValue = returnValue;
        }
    }
}
