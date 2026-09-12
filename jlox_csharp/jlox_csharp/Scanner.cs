using System.Globalization;

namespace JLox
{
    public class Scanner
    {
        private static readonly Dictionary<string, TokenType> keywords = new Dictionary<string, TokenType>
        {
            { "and", TokenType.AND },
            { "class", TokenType.CLASS },
            { "else", TokenType.ELSE },
            { "false", TokenType.FALSE },
            { "for", TokenType.FOR },
            { "fun", TokenType.FUN },
            { "if", TokenType.IF },
            { "nil", TokenType.NIL },
            { "or", TokenType.OR },
            { "print", TokenType.PRINT },
            { "return", TokenType.RETURN },
            { "super", TokenType.SUPER },
            { "this", TokenType.THIS },
            { "true", TokenType.TRUE },
            { "var", TokenType.VAR },
            { "while", TokenType.WHILE },
        };

        private readonly string _source;
        private readonly List<Token> _tokens = new List<Token>();
        private int _start = 0;
        private int _current = 0;
        private int _line = 1;

        public Scanner(string source)
        {
            _source = source;
        }

        public List<Token> ScanTokens()
        {
            while (!IsAtEnd())
            {
                // We are at the beginning of the next lexeme.
                _start = _current;
                ScanToken();
            }

            _tokens.Add(new Token(TokenType.EOF, "", null, _line));
            return _tokens;
        }

        private bool IsAtEnd()
        {
            return _current >= _source.Length;
        }

        private void ScanToken()
        {
            char c = Advance();
            switch (c)
            {
                // LEXICAL ERRORS | NUMBER LITERAL | IDENTIFIER
                default:
                    if (IsDigit(c))
                        NumberLiteral();
                    else if (IsAlpha(c))
                        Identifier();
                    else
                        Lox.Error(_line, $"Unexpected character: '{c}'");
                    break;
                // LEXEMES
                case '(': AddToken(TokenType.LEFT_PAREN); break;
                case ')': AddToken(TokenType.RIGHT_PAREN); break;
                case '{': AddToken(TokenType.LEFT_BRACE); break;
                case '}': AddToken(TokenType.RIGHT_BRACE); break;
                case ',': AddToken(TokenType.COMMA); break;
                case '.': AddToken(TokenType.DOT); break;
                case '-': AddToken(TokenType.MINUS); break;
                case '+': AddToken(TokenType.PLUS); break;
                case ';': AddToken(TokenType.SEMICOLON); break;
                case '*': AddToken(TokenType.STAR); break;
                // OPERATORS
                case '!': AddToken(Match('=') ? TokenType.BANG_EQUAL : TokenType.BANG); break;
                case '=': AddToken(Match('=') ? TokenType.EQUAL_EQUAL : TokenType.EQUAL); break;
                case '<': AddToken(Match('=') ? TokenType.LESS_EQUAL : TokenType.LESS); break;
                case '>': AddToken(Match('=') ? TokenType.GREATER_EQUAL : TokenType.GREATER); break;
                // LONGER LEXEMES
                case '/':
                    if (Match('/'))
                    {
                        // A comment goes until the end of the line.
                        while (Peek() != '\n' && !IsAtEnd()) Advance();
                    }
                    else if (Match('*'))
                    {
                        BlockComment();
                    }
                    else
                    {
                        AddToken(TokenType.SLASH);
                    }
                    break;
                // NEWLINES && WHITESPACES
                case '\n': _line++; break;
                case ' ':
                case '\r':
                case '\t':
                    // Ignore whitespace
                    break;
                // STRING LITERAL
                case '"': StringLiteral(); break;
            }
        }

        private char Advance()
        {
            return _source[_current++];
        }

        private void AddToken(TokenType type)
        {
            AddToken(type, null);
        }

        private void AddToken(TokenType type, object? literal)
        {
            string text = _source.Substring(_start, _current - _start);
            _tokens.Add(new Token(type, text, literal, _line));
        }

        private bool Match(char expected)
        {
            if (IsAtEnd()) return false;
            if (_source[_current] != expected) return false;

            _current++;
            return true;
        }

        private char Peek()
        {
            if (IsAtEnd()) return '\0';
            return _source[_current];
        }

        private void StringLiteral()
        {
            while (Peek() != '"' && !IsAtEnd())
            {
                if (Peek() == '\n') _line++;
                Advance();
            }

            if (IsAtEnd())
            {
                Lox.Error(_line, "Unterminated string.");
                return;
            }

            // The closing "
            Advance();

            // Trim the surrounding quotes
            string value = _source.Substring(_start + 1, _current - _start - 2);
            AddToken(TokenType.STRING, value);
        }

        private bool IsDigit(char c) => c >= '0' && c <= '9';

        private void NumberLiteral()
        {
            while (IsDigit(Peek())) Advance();

            // Look for fractional part
            if (Peek() == '.' && IsDigit(PeekNext()))
            {
                // Consume the .
                Advance();
                while (IsDigit(Peek())) Advance();
            }

            string value = _source.Substring(_start, _current - _start);
            AddToken(TokenType.NUMBER, Double.Parse(value, CultureInfo.InvariantCulture));
        }

        private char PeekNext()
        {
            if (_current + 1 >= _source.Length) return '\0';
            return _source[_current + 1];
        }

        private void Identifier()
        {
            while (IsAlphaNumeric(Peek())) Advance();

            string text = _source.Substring(_start, _current - _start);

            TokenType type = keywords.TryGetValue(text, out var keywordType) ? keywordType : TokenType.IDENTIFIER;

            AddToken(type);
        }

        private bool IsAlpha(char c) => (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || c == '_';
        private bool IsAlphaNumeric(char c) => IsAlpha(c) || IsDigit(c);

        private void BlockComment()
        {
            int nesting = 1;

            while (nesting > 0 && !IsAtEnd())
            {
                if (Peek() == '/' && PeekNext() == '*')
                {
                    // Detect nesting aperture
                    Advance(); // Consum /
                    Advance(); // Consum *
                    nesting++;
                }
                else if (Peek() == '*' && PeekNext() == '/')
                {
                    // Detect closure */
                    Advance(); // Consum *
                    Advance(); // Consum /
                    nesting--;
                }
                else
                {
                    if (Peek() == '\n') _line++;
                    Advance();
                }
            }

            if (nesting > 0) Lox.Error(_line, "Unterminated block comment.");
        }
    }
}
