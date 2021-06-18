using System;
using System.Collections.Generic;
using System.Text;

namespace pyrolang2.Interpreter
{
    public enum TokenType
    {
        None,
        Symbol,
        String,
        Keyword,
        Number,
        Boolean,
        Punctuation,
        Grouping,
        Arithmetic,
        Assignment,
        Relational,
        End
    }

    public struct Token
    {
        public static Token EndToken => new Token(TokenType.End, ";", -1, -1);
        public TokenType Type { get; set; }
        public string Value { get; set; }
        public int Line { get; set; }
        public int Column { get; set; }

        public Token(TokenType type, string value, int line, int col)
        {
            Type = type;
            Value = value;
            Line = line;
            Column = col;
        }

        public bool CompareTypeValue(Token token)
        {
            return Type == token.Type && Value == token.Value;
        }

        public override bool Equals(object o)
        {
            return base.Equals(o);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(Token left, Token right) => left.Equals(right);

        public static bool operator !=(Token left, Token right) => !left.Equals(right);
    }
}
