using System;
using System.Collections.Generic;
using System.Linq;
using Laba_3.Enums;

namespace Laba_3
{
    internal class Laba
    {
        private readonly ILogger _logger;

        private readonly Dictionary<(NTerminals, TokenNames), Rule> _rules =
            new Dictionary<(NTerminals, TokenNames), Rule>()
            {
                {
                    (NTerminals.S, TokenNames.KEY_WORD_FOR), Rule.All[0]
                },
                {
                    (NTerminals.S, TokenNames.END), Rule.All[1]
                },
                {
                    (NTerminals.A, TokenNames.IDENT), Rule.All[2]
                },
                {
                    (NTerminals.B, TokenNames.IDENT), Rule.All[3]
                },
                {
                    (NTerminals.B, TokenNames.NUM), Rule.All[3]
                },
                {
                    (NTerminals.V, TokenNames.IDENT), Rule.All[4]
                },
                {
                    (NTerminals.V, TokenNames.NUM), Rule.All[5]
                },
                {
                    (NTerminals.T, TokenNames.KEY_WORD_FOR), Rule.All[6]
                },
                {
                    (NTerminals.T, TokenNames.IDENT), Rule.All[7]
                },
                {
                    (NTerminals.T, TokenNames.NUM), Rule.All[8]
                },
                {
                    (NTerminals.C, TokenNames.OPERATION), Rule.All[9]
                },
                {
                    (NTerminals.C, TokenNames.ASSIGN), Rule.All[10]
                }
            };

        private Element _root;

        public Laba()
        {
            _logger = new ConsoleLogger();
        }

        public void Start()
        {
            var test = "for(a=5abcf34; b>0; b:=1) do f<f;";
            // var test = "for(_testVariable:=0; 135abcdef123>0; BETA:=_test_test_test) do for(a:=5; b>0; b:=1) do 5 < 10; for(a:=5; b>0; b:=1) do f:= 141f4;";
            var lexer = new Lexer(test);
            var tokens = lexer.Start();
            var result = Syntax(tokens);

            if (result)
            {
                var str = _root.Print();
                Console.Write(str);
            }
        }

        private bool Syntax(IReadOnlyList<Token> tokens)
        {
            var pos = 0;
            var stack = new Stack<Element>();
            stack.Push(new Element {Type = TokenNames.END, value = "$"});
            stack.Push(new Element {NTerminal = NTerminals.S});

            _root = stack.Peek();

            while (stack.Any() && pos < tokens.Count)
            {
                var element = stack.Peek();
                var token = tokens[pos];

                if (element.IsTerminal())
                {
                    if (element.Type == token.Name && (element.value == null || element.value == token.Value))
                    {
                        pos++;
                        var pop = stack.Pop();
                        _logger.LogSuccess(
                            $"Успешный парсинг токена: {{ Name = {token.Name} Value = {token.Value} }}. Ожидалось Type = {element.Type} {new string(element.value != null ? new string($"Value = {element.value}") : "")}");

                        if (token.Value != null)
                        {
                            pop.AddChild(new Element {value = token.Value});
                        }
                    }
                    else
                    {
                        _logger.LogError(
                            $"Неверный токен: {{ Name = {token.Name} Value = {token.Value} }}. Ожидалось Type = {element.Type} {new string(element.value != null ? new string($"Value = {element.value}") : "")}");
                        return false;
                    }
                }
                else
                {
                    if (_rules.TryGetValue((element.NTerminal!.Value, token.Name!), out var rule))
                    {
                        var pop = stack.Pop();
                        rule.SplitReverse().ForEach(el =>
                        {
                            var copy = new Element() {NTerminal = el.NTerminal, Type = el.Type, value = el.value};
                            stack.Push(copy);
                            pop.AddChild(copy);
                        });

                        pop.Children.Reverse();
                    }
                    else
                    {
                        _logger.LogError(
                            $"Не найдено правил для: токен = {{ Name = {token.Name} Value = {token.Value} }}. терминал = {element.NTerminal}");

                        return false;
                    }
                }
            }

            return pos == tokens.Count && (stack.Count == 0 || stack.Count == 1 && stack.Peek().Type == TokenNames.END);
        }
    }
}