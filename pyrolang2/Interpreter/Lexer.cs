using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace pyrolang2.Interpreter
{
    public class Lexer
    {
        public readonly char DECIMAL_POINT = '.';
        public readonly char DECIMAL_SUBSEPARATOR = ',';

        private readonly HashSet<char> WHITESPACE = new HashSet<char> { ' ', '\n' };
        private readonly HashSet<char> ARITHMETIC = new HashSet<char> { '+', '-', '*', '/' };
        private readonly HashSet<char> GROUPING = new HashSet<char> { '(', ')', '{', '}' };
        private readonly HashSet<char> PUNCTUATION = new HashSet<char> { '.', ',' };
        private readonly Regex REGEX_NUMBER = new Regex("[.0-9]");
        private readonly Regex REGEX_IDENTIFIER_START = new Regex("[_a-zA-Z]");
        private readonly Regex REGEX_IDENTIFIER = new Regex("[_a-zA-Z0-9]");
        private readonly HashSet<string> KEYWORDS = new HashSet<string> { "if", "else", "fn", "return" };

        public Lexer()
        {

        }

        public IEnumerable<Token> Lex(Iterator<char> iterator)
        {
            while (iterator.GetNext() != '\0')
            {
                char curChar = iterator.MoveNext();
                int line = iterator.Line;
                int col = iterator.Column;
                if (WHITESPACE.Contains(curChar))
                {
                    continue;
                }
                else if (ARITHMETIC.Contains(curChar))
                {
                    yield return new Token(TokenType.Arithmetic, curChar.ToString(),
                        line, col);
                }
                else if (GROUPING.Contains(curChar))
                {
                    yield return new Token(TokenType.Grouping, curChar.ToString(),
                        line, col);
                }
                else if (curChar == '=')
                {
                    yield return new Token(TokenType.Assignment, "=",
                        line, col);
                }
                else if (curChar == '"')
                {
                    yield return new Token(TokenType.String, ScanString(iterator),
                        line, col);
                }
                else if (REGEX_NUMBER.IsMatch(curChar.ToString()))
                {
                    yield return new Token(TokenType.Number, ScanNumber(iterator,
                        curChar.ToString()), line, col);
                }
                else if (REGEX_IDENTIFIER_START.IsMatch(curChar.ToString()))
                {
                    string identifier = ScanRegexMatch(iterator,
                        REGEX_IDENTIFIER, curChar.ToString());
                    TokenType type = TokenType.Symbol;
                    if (KEYWORDS.Contains(identifier))
                    {
                        type = TokenType.Keyword;
                    }
                    yield return new Token(type, identifier, line, col);
                }
                else if (PUNCTUATION.Contains(curChar))
                {
                    yield return new Token(TokenType.Punctuation, curChar.ToString(),
                        line, col);
                }
                else if (curChar == ';')
                {
                    yield return new Token(TokenType.End, ";",
                        line, col);
                }
                else
                {
                    throw new InterpreterException("Unknown character",
                        line, col);
                }
            }
        }

        private string ScanString(Iterator<char> iterator, string begin = "")
        {
            while (iterator.GetNext() != '\0')
            {
                char c = iterator.MoveNext();
                if (c != '"')
                {
                    if (c == '\\')
                    {
                        char escape = iterator.MoveNext();
                        if (escape == '\0')
                        {
                            throw new InterpreterException("Unexpected EOL",
                                iterator.Line, iterator.Column);
                        }
                        else if (escape == 'n')
                        {
                            begin += '\n';
                        }
                    }
                    else
                    {
                        begin += c;
                    }
                }
                else
                {
                    return begin;
                }
            }
            throw new InterpreterException("Unexpected EOL",
                iterator.Line, iterator.Column);
        }

        public string ScanNumber(Iterator<char> iterator, string begin = "")
        {
            bool hasPoint = false;
            //bool isDecimal = true;
            //bool isHex = false;
            //bool isBinary = false;

            if (begin == ".")
            {
                hasPoint = true;
            }

            while (iterator.GetNext() != '\0')
            {
                char c = iterator.GetNext();
                if (c == '.')
                {
                    if (hasPoint)
                    {
                        throw new InterpreterException("Unexpected decimal point",
                            iterator.Line, iterator.Column);
                    }
                    hasPoint = true;
                    begin += c;
                }
                else if (c == ',')
                {
                    continue;
                }
                else if (REGEX_NUMBER.IsMatch(c.ToString()))
                {
                    begin += c;
                }
                else
                {
                    break;
                }
                iterator.MoveNext();
            }
            return begin;
        }

        public string ScanRegexMatch(Iterator<char> iterator, Regex regex, string begin = "")
        {
            while (iterator.GetNext() != '\0')
            {
                char c = iterator.GetNext();
                if (regex.IsMatch(c.ToString()))
                {
                    begin += c;
                    iterator.MoveNext();
                }
                else
                {
                    break;
                }
            }
            return begin;
        }
    }
}
