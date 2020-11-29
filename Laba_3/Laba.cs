using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Laba_3
{
    public enum NTerminals
    {
        S,
        A,
        B,
        C,
        V,
        T
    }

    public class Rule
    {
        private readonly List<Element> _rules;

        public Rule(params Element[] rules)
        {
            _rules = rules.ToList();
        }

        public List<Element> SplitReverse()
        {
            var t = _rules.ToList();
            t.Reverse();
            return t;
        }
    }

    public class Element
    {
        public TokenNames? Type;
        public string value;
        public NTerminals? NTerminal;

        public List<Element> Children = new List<Element>();

        public Element()
        {
        }

        public Element(NTerminals nTerminals)
        {
            NTerminal = nTerminals;
        }

        public Element(TokenNames tokenName, string value = null)
        {
            Type = tokenName;
            this.value = value;
        }

        public bool IsTerminal()
        {
            return NTerminal == null;
        }

        public void AddChild(Element element)
        {
            Children.Add(element);
        }

        public string Print(StringBuilder prefix = null, StringBuilder childPrefix = null)
        {
            // var buffer = $"\n{prefix}{NTerminal?.ToString() ?? Type.ToString()}";
            var buffer = new StringBuilder();
            buffer.AppendLine();
            buffer.Append(prefix);
            buffer.Append(NTerminal?.ToString() ?? Type.ToString());


            var sb1 = new StringBuilder();
            var sb2 = new StringBuilder();

            for (var i = 0; i < Children.Count; i++)
            {
                var child = Children[i];
                var isLast = Children.Count == 0 || Children.Count - 1 == i;
                if (isLast)
                {
                    sb1.Append(childPrefix);
                    sb1.Append("└──");

                    sb2.Append(childPrefix);
                    sb2.Append("   ");
                }
                else
                {
                    sb1.Append(childPrefix);
                    sb1.Append("├──");

                    sb2.Append(childPrefix);
                    sb2.Append("│  ");
                }

                buffer.Append(child.Print(sb1, sb2));
                sb1.Clear();
                sb2.Clear();
            }

            if (Children.Count == 0 && value != null)
            {
                buffer.Append("──");
                buffer.Append(value);
            }

            return buffer.ToString();
        }
    }

    internal class Laba
    {
        private readonly ILogger _logger;

        private readonly Dictionary<(NTerminals, TokenNames), Rule> _rules =
            new Dictionary<(NTerminals, TokenNames), Rule>()
            {
                {
                    (NTerminals.S, TokenNames.KEY_WORD_FOR),
                    new Rule(
                        new Element(TokenNames.KEY_WORD_FOR), new Element(TokenNames.DELIMITER, "("),
                        new Element(NTerminals.A), new Element(TokenNames.DELIMITER, ";"),
                        new Element(NTerminals.B), new Element(TokenNames.DELIMITER, ";"),
                        new Element(NTerminals.A), new Element(TokenNames.DELIMITER, ")"),
                        new Element(TokenNames.KEY_WORD_DO), new Element(NTerminals.T)
                    )
                },
                {
                    (NTerminals.S, TokenNames.END),
                    new Rule(new Element(TokenNames.END)) // new Element(TokenNames.END)
                },
                {
                    (NTerminals.A, TokenNames.IDENT),
                    new Rule(new Element(TokenNames.IDENT), new Element(TokenNames.ASSIGN), new Element(NTerminals.V))
                },
                {
                    (NTerminals.B, TokenNames.IDENT),
                    new Rule(new Element(TokenNames.IDENT), new Element(TokenNames.OPERATION),
                        new Element(NTerminals.V))
                },
                {
                    (NTerminals.B, TokenNames.NUM),
                    new Rule(new Element(TokenNames.NUM), new Element(TokenNames.OPERATION), new Element(NTerminals.V))
                },
                {
                    (NTerminals.V, TokenNames.IDENT), new Rule(new Element(TokenNames.IDENT))
                },
                {
                    (NTerminals.V, TokenNames.NUM), new Rule(new Element(TokenNames.NUM))
                },
                {
                    (NTerminals.T, TokenNames.KEY_WORD_FOR), new Rule(new Element(NTerminals.S))
                },
                {
                    (NTerminals.T, TokenNames.IDENT),
                    new Rule(new Element(TokenNames.IDENT), new Element(NTerminals.C), new Element(NTerminals.V),
                        new Element(TokenNames.DELIMITER, ";"), new Element(NTerminals.S))
                },
                {
                    (NTerminals.T, TokenNames.NUM),
                    new Rule(new Element(TokenNames.NUM), new Element(TokenNames.OPERATION), new Element(NTerminals.V),
                        new Element(TokenNames.DELIMITER, ";"), new Element(NTerminals.S))
                },
                {
                    (NTerminals.C, TokenNames.OPERATION), new Rule(new Element(TokenNames.OPERATION))
                },
                {
                    (NTerminals.C, TokenNames.ASSIGN), new Rule(new Element(TokenNames.ASSIGN))
                }
            };

        private Element root;

        public Laba()
        {
            _logger = new ConsoleLogger();
        }

        public void Start()
        {
            var lexer = new Lexer(
                "for(a:=1;b>0;b:=1)do f:=f;");
            var tokens = lexer.Start();
            var result = Syntax(tokens);

            if (result)
            {
                var str = root.Print();
                Console.Write(str);
            }
        }

        private bool Syntax(List<Token> tokens)
        {
            var pos = 0;
            var stack = new Stack<Element>();
            stack.Push(new Element {Type = TokenNames.END, value = "$"});
            stack.Push(new Element {NTerminal = NTerminals.S});

            root = stack.Peek();

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
                        _logger.LogSuccess("pop", "type:", token.Name.ToString(), "value:", token.Value);

                        if (token.Value != null)
                        {
                            pop.AddChild(new Element {value = token.Value});
                        }
                    }
                    else
                    {
                        _logger.LogError("Неверный терминал, ожидался типа:", element.Type?.ToString() ?? "NULL");
                        return false;
                    }
                }
                else
                {
                    _logger.Log("finding rule for:");
                    _logger.Log("{ type:", token.Name.ToString(), "value:", token.Value, "}");
                    _logger.Log("{ type:", element.Type.ToString(), "", element.NTerminal?.ToString() ?? "NULL", "}");

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
                        _logger.LogError("Не найдено правил для:");
                        _logger.LogError("{ type:", token.Name.ToString(), "value:", token.Value, "}");
                        _logger.LogError("{ type:", element.ToString(), "}");
                        return false;
                    }
                }
            }

            return pos == tokens.Count && (stack.Count == 0 || stack.Count == 1 && stack.Peek().Type == TokenNames.END);
        }
    }
}