using System;
using System.Text;

namespace JLox
{
    public class Lox
    {
        public static bool _hadError = false;
        public static void Main(string[] args)
        {
            if (args.Length > 1)
            {
                Console.WriteLine("Usage: jlox [script]");
                Environment.Exit(64);
            }
            else if (args.Length == 1)
            {
                RunFile(args[0]);
            }
            else
            {
                RunPrompt();
            }
        }

        private static void RunFile(string path)
        {
            byte[] bytes = File.ReadAllBytes(path);
            Run(Encoding.Default.GetString(bytes));

            if (_hadError) Environment.Exit(65);
        }

        private static void RunPrompt()
        {
            for (; ; )
            {
                Console.Write("> ");
                string? line = Console.ReadLine();

                if (line == null) break;

                Run(line);

                _hadError = false;
            }
        }

        private static void Run(String source)
        {
            Scanner scanner = new Scanner(source);
            List<Token> tokens = scanner.ScanTokens();

            // Por ahora, solo imprimimos los tokens.
            foreach (Token token in tokens)
            {
                Console.WriteLine(token);
            }
        }

        public static void Error(int line, string message)
        {
            Report(line, "", message);
        }

        private static void Report(int line, string where, string message)
        {
            Console.Error.WriteLine($"[line {line}] Error{where}: {message}");
            _hadError = true;
        }
    }
}