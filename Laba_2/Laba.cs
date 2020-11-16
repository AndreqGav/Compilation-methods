using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Laba_2
{
    public class Laba
    {
        private char[] S = new char[100];
        private int _i;
        private int ST;
        private int OLD;
        private int CL;
        private int FIX;

        private readonly int[][] D =
        {
            new[] {-1, 1, 1, 2, 3, -1}, //S0
            new[] {-1, -1, 1, 2, 3, -2}, //S1
            new[] {-1, -1, 2, -1, 3, -2}, //S2
            new[] {-1, 4, 4, -1, -1, -1}, //S3
            new[] {-1, -1, 4, -1, -1, -2}, //S4
        };

        private readonly int[] W = {-1, 2, 3, -1, 4, -1};
        private readonly int[] FIN = {1, 2, 4};

        private readonly string[] _outStrings =
        {
            "COMPLETE (+,-)125.666E(+,-)44 :", // fin-1
            "Error", // fin-2
            "Целая часть:", // fin-3
            "Дробная часть: ", // fin-4
            "Порядок: " // fin-5
        };

        private static int Sclass(char c)
        {
            switch (c)
            {
                case '+':
                case '-':
                    return 1;
                case '.':
                    return 3;
                case 'E':
                case 'e':
                    return 4;

                case '\0': return 5;

                default:
                {
                    return char.IsDigit(c) ? 2 : 0;
                }
            }
        }

        private void Lexfile()
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;

            string name;
            do
            {
                Console.WriteLine("file name:");
                name = Console.ReadLine() ?? "";
                name = name!.IndexOf('.') > -1 ? name : $"{name}.txt";
            } while (!File.Exists(name));

            Console.ResetColor();

            S[0] = '\0';
            _i = 0;
            ST = 0;
            var lines = File.ReadAllLines(name, Encoding.UTF8);
            var numberLine = 0;

            while (true)
            {
                if ((ST == 0) && (S[_i] == '\0'))
                {
                    if (numberLine == lines.Length)
                    {
                        Console.WriteLine();
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.WriteLine("The end");
                        Console.ResetColor();

                        return;
                    }

                    S = new char[100];
                    var arr = lines[numberLine].ToCharArray(0, Math.Min(lines[numberLine].Length, 100));
                    arr.CopyTo(S, 0);
                    numberLine++;
                    _i = 0;

                    Console.WriteLine();
                    Console.Write($"Start: ");
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.Write(new string(S));
                    Console.ResetColor();
                    Console.WriteLine();
                }


                CheckFin();

                OLD = ST;

                CL = Sclass(S[_i]);
                Console.Write("{0}[{1}->", S[_i], ST);
                ST = D[ST][CL];
                Console.Write("{0}]\n", ST);
                _i++;

                if (ST < 0)
                {
                    var n = -ST - 1;

                    if (n == 0) // ST == -1
                    {
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine(_outStrings[1]);
                        S[_i] = '\0';
                    }

                    if (n == 1) // ST == -2
                    {
                        CheckFin();
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"{_outStrings[0]} {new string(S)}");
                        S[_i] = '\0';
                    }


                    Console.ResetColor();

                    ST = 0;
                    OLD = -1;
                }
            }
        }

        private void CheckFin()
        {
            if (ST != OLD)
            {
                if (FIN.Contains(OLD))
                {
                    var n = W[OLD];
                    if (n >= 0)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.WriteLine($"{_outStrings[n]} {new string(S[FIX..(_i - 1)])}");
                        Console.ResetColor();
                    }
                }

                FIX = Math.Max(0, _i - 1);
            }
        }

        public void Start()
        {
            Lexfile();
        }
    }
}