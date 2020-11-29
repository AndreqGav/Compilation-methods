using System;

namespace Laba_3
{
    public interface ILogger
    {
        public void Log(params string[] value);
        public void LogError(params string[] value);
        public void LogSuccess(params string[] value);
    }

    public class ConsoleLogger : ILogger
    {
        public void Log(params string[] value)
        {
            Console.WriteLine(string.Join(' ', value));
        }

        public void LogError(params string[] value)
        {
            SetColor(ConsoleColor.DarkRed);
            Log(value);
            Reset();
        }

        public void LogSuccess(params string[] value)
        {
            SetColor(ConsoleColor.DarkGreen);
            Log(value);
            Reset();
        }

        private void SetColor(ConsoleColor color)
        {
            Console.ForegroundColor = color;
        }

        private void Reset()
        {
            Console.ResetColor();
        }
    }
}