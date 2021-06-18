using System;
using System.Collections.Generic;
using pyrolang2.Interpreter;

namespace pyrolang2
{
    class Program
    {
        static void Main(string[] args)
        {
            string script =
                "fn(int x, int y) { return x + y; }";
            Iterator<char> chars = new Iterator<char>(script.ToCharArray());
            Lexer lexer = new Lexer();
            List<Token> tokens = new List<Token>(lexer.Lex(chars));
            try
            {
                foreach (Token token in tokens)
                {
                    Console.WriteLine($"{token.Type} {token.Value} @ {token.Line}:{token.Column}");
                }
                List<Node> nodes = new List<Node>(Parser.Parse(new Iterator<Token>(tokens.ToArray())));
                Console.WriteLine(nodes);
            }
            catch (InterpreterException e)
            {
                Console.WriteLine("** INTERPRETER ERROR **");
                Console.WriteLine(e.Message);
                Console.WriteLine($"\n~~~~~ SOURCE FILE ~~~~~ @ {e.Line}:{e.Column}");
                Console.WriteLine(script.Split("\n")[e.Line - 1]);
                Console.WriteLine(new String(' ', e.Column - 1) + "^");
            }
        }
    }
}
