using System;
using System.Collections.Generic;
using System.Text;
using Laba_3.Enums;

namespace Laba_3
{
    public struct Token
    {
        public TokenNames Name;
        public string Value;
    };

    public class Lexer
    {
        private readonly List<Token> _tokens = new List<Token>();

        private int _pos;

        private readonly string _text;

        public Lexer(string text)
        {
            _text = text;
        }

        private char GetNext()
        {
            if (_pos < _text.Length)
            {
                return _text[_pos++];
            }

            _pos++;
            return '$';
        }

        private void AddToken(Token token)
        {
            _tokens.Add(token);
        }

        private static TokenNames? GetKeyWord(string word)
        {
            return word switch
            {
                "for" => TokenNames.KEY_WORD_FOR,
                "do" => TokenNames.KEY_WORD_DO,
                _ => null
            };
        }

        public List<Token> Start()
        {
            var cs = States.H;

            var c = GetNext();

            var errSymbol = '$';
            while (_pos <= _text.Length)
            {
                switch (cs)
                {
                    case States.H:
                    {
                        while (c == ' ' || (c == '\t') || (c == '\n'))
                        {
                            c = GetNext();
                        }

                        if (c >= 'A' && c <= 'Z' || c >= 'a' && c <= 'z' || c == '_')
                        {
                            cs = States.ID;
                        }
                        else if (c >= '0' && c <= '9')
                        {
                            cs = States.NM;
                        }
                        else if (c == ':')
                        {
                            cs = States.ASGN;
                        }
                        else
                        {
                            cs = States.DLM;
                        }

                        break;
                    } // case H

                    case States.ASGN:
                    {
                        var colon = c;
                        c = GetNext();
                        if (c == '=')
                        {
                            AddToken(new Token {Name = TokenNames.ASSIGN, Value = ":="});
                            c = GetNext();
                            cs = States.H;
                        }
                        else
                        {
                            errSymbol = colon;
                            cs = States.ERR;
                        }

                        break;
                    } // case ASGN
                    case States.DLM:
                    {
                        switch (c)
                        {
                            case '(':
                            case ')':
                            case ';':
                                AddToken(new Token {Name = TokenNames.DELIMITER, Value = c.ToString()});

                                c = GetNext();
                                cs = States.H;
                                break;
                            case '<':
                            case '>':
                            case '=':
                                AddToken(new Token {Name = TokenNames.OPERATION, Value = c.ToString()});
                                c = GetNext();
                                cs = States.H;
                                break;
                            default:
                                errSymbol = c;
                                // c = getNext();
                                cs = States.ERR;
                                break;
                        }

                        break;
                    } // case DLM
                    case States.ERR:
                    {
                        Console.WriteLine("\nUnknown character: %c\n" + errSymbol);

                        cs = States.H;
                        c = GetNext();

                        break;
                    }
                    case States.ID:
                    {
                        var buf = new StringBuilder();
                        buf.Append(c);
                        c = GetNext();
                        while (c >= 'A' && c <= 'Z' || c >= 'a' && c <= 'z' || c >= '0' && c <= '9' || c == '_')
                        {
                            buf.Append(c);
                            c = GetNext();
                        }

                        AddToken(
                            new Token
                            {
                                Name = GetKeyWord(buf.ToString()) ?? TokenNames.IDENT,
                                Value = buf.ToString()
                            });
                        cs = States.H;
                        break;
                    } // case ID
                    case States.NM:
                    {
                        var buf = new StringBuilder();
                        buf.Append(c);
                        c = GetNext();
                        while (c >= 'a' && c <= 'f' || c >= '0' && c <= '9')
                        {
                            buf.Append(c);
                            c = GetNext();
                        }

                        AddToken(
                            new Token
                            {
                                Name = TokenNames.NUM,
                                Value = buf.ToString()
                            });
                        cs = States.H;
                        break;
                    } // case NM
                    default:
                        throw new ArgumentOutOfRangeException();
                } // switch
            } // while

            _tokens.Add(new Token {Name = TokenNames.END, Value = "$"});
            return _tokens;
        }
    }
}