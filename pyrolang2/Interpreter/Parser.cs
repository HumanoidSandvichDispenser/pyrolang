using System;
using System.Collections.Generic;
using System.Text;

namespace pyrolang2.Interpreter
{
    public class Parser
    {
        public Iterator<Token> Tokens { get; }
        public List<Token> StopAt { get; }

        public Parser(Iterator<Token> tokens, List<Token> stopAt)
        {
            Tokens = tokens;
            StopAt = stopAt;
        }

        public static IEnumerable<Node> Parse(Iterator<Token> tokens)
        {
            Parser parser = new Parser(tokens, new List<Token> { Token.EndToken });
            while (parser.Tokens.GetNext() != default)
            {
                Node expression = parser.NextExpression(default);
                if (expression != default)
                {
                    yield return expression;
                }
                parser.Tokens.MoveNext();
            }
        }

        public Node NextExpression(Node previous = default)
        {
            FailIfAtEOF();
            Token token = Tokens.GetNext();
            if (StopAt.Exists(x => token.CompareTypeValue(x)))
            {
                Console.WriteLine("STOP!!!");
                return previous;
            }
            Console.WriteLine($"{token.Type} - {token.Value}");
            Tokens.MoveNext();
            if (previous == default &&
                token.Type == TokenType.Symbol ||
                token.Type == TokenType.String ||
                token.Type == TokenType.Number)
            {
                return NextExpression(new TokenNode(token, token.Line, token.Column));
            }
            else if (previous is TokenNode && token.Type == TokenType.Symbol)
            {
                Token previousToken = (previous as TokenNode).ChildToken;
                return NextExpression(new DeclarationNode(previousToken.Value,
                    token.Value, null, token.Line, token.Column));
            }
            else if (previous == default &&
                token.Type == TokenType.Keyword)
            {
                if (token.Value == "return")
                {
                    Node next = NextExpression();
                    return new ReturnNode(next, token.Line, token.Column);
                }
                else
                {
                    return NextExpression(new KeywordNode(
                        token.Value, token.Line, token.Column));
                }
            }
            else if (token.Type == TokenType.Arithmetic)
            {
                Node next = NextExpression();
                if (previous == default || next == default)
                {
                    throw new InterpreterException("Expected operands on both sides",
                        token.Line, token.Column);
                }
                return new OperationNode(token, previous, next, token.Line, token.Column);
            }
            else if (token.Type == TokenType.Grouping &&
                token.Value == "(")
            {
                if (previous is TokenNode)
                {
                    var args = MultipleExpressions(new Token(TokenType.Punctuation, ",", -1, -1),
                        new Token(TokenType.Grouping, ")", -1, -1));
                    return new FuncCallNode(args, token.Line, token.Column);
                }
                else if (previous is KeywordNode)
                {
                    if (((KeywordNode)previous).Keyword == "fn")
                    { 
                        var parameters = MultipleExpressions(new Token(TokenType.Punctuation, "," , -1, -1),
                            new Token(TokenType.Grouping, ")", -1, -1));
                        foreach (var parameter in parameters)
                        {
                            if (parameter is not DeclarationNode)
                            {
                                throw new InterpreterException("Invalid declaration",
                                    parameter.Line, parameter.Column);
                            }
                        }
                        return new FuncDefNode(parameters, token.Line, token.Column);
                    }
                }
                else
                {
                    return null;
                }
            }
            else if (token.Type == TokenType.Grouping &&
                token.Value == "{")
            {
                FuncDefNode definition = null;
                if (previous is FuncDefNode)
                {
                    definition = previous as FuncDefNode;
                }
                List<Node> implementation = MultipleExpressions(
                    Token.EndToken,
                    new Token(TokenType.Grouping, "}", -1, -1));
                return new FuncImplNode(definition, implementation, token.Line, token.Column);
            }
            else if (token.Type == TokenType.Assignment)
            {
                if (previous is TokenNode)
                {
                    Node next = NextExpression();
                    return new AssignmentNode((previous as TokenNode).ChildToken.Value,
                        next, token.Line, token.Column);
                }
                else if (previous is DeclarationNode)
                {
                    DeclarationNode declaration = previous as DeclarationNode;
                    declaration.Value = NextExpression();
                    return declaration;
                }
                else
                {
                    throw new InterpreterException("Can not assign value",
                        token.Line, token.Column);
                }
            }
            throw new InterpreterException($"Unexpected token {token.Type}",
                token.Line, token.Column);
        }

        public List<Node> MultipleExpressions(Token delimiter, Token end)
        {
            List<Node> trees = new List<Node>();
            FailIfAtEOF();
            Token token = Tokens.GetNext();
            if (token.CompareTypeValue(end))
            {
                Tokens.MoveNext();
            }
            else
            {
                Parser argParser = new Parser(Tokens, new List<Token> { delimiter, end });
                while (!token.CompareTypeValue(end))
                {
                    Node expr = argParser.NextExpression();
                    if (expr is not null)
                    {
                        trees.Add(expr);
                    }
                    token = Tokens.MoveNext();
                    FailIfAtEOF();
                }
            }
            return trees;
        }

        public void FailIfAtEOF()
        {
            if (Tokens.GetNext() == default)
            {
                throw new InterpreterException("Unexpected EOF",
                    0, 0);
            }
        }
    }
}
