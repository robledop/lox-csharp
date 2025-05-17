using static CSharpLox.TokenType;

namespace CSharpLox;

public class Lexer
{
    public List<Token> Tokens { get; set; }
    int CurrentPosition { get; set; }
    string Source { get; set; }
    int NextPosition { get; set; }
    char CurrentChar { get; set; }
    int Line { get; set; } = 1;
    int Column { get; set; } = 1;

    static readonly Dictionary<string, TokenType> Keywords = new()
    {
        { "and", AND },
        { "class", CLASS },
        { "else", ELSE },
        { "false", FALSE },
        { "for", FOR },
        { "fun", FUN },
        { "if", IF },
        { "nil", NIL },
        { "or", OR },
        { "print", PRINT },
        { "return", RETURN },
        { "super", SUPER },
        { "this", THIS },
        { "true", TRUE },
        { "var", VAR },
        { "while", WHILE },
        { "break", BREAK },
        { "continue", CONTINUE },
    };

    public Lexer(string source)
    {
        Source = source;
        ReadChar();
        Tokens = ScanTokens().ToList();

        Reset();
        ReadChar();
    }

    void Reset()
    {
        CurrentPosition = 0;
        NextPosition = 0;
        CurrentChar = '\0';
    }

    void ReadChar()
    {
        CurrentChar = NextPosition >= Source.Length ? '\0' : Source[NextPosition];

        CurrentPosition = NextPosition;
        NextPosition++;
        Column++;
    }

    char Peek(int offset = 0)
    {
        return NextPosition + offset >= Source.Length ? '\0' : Source[NextPosition + offset];
    }

    bool Match(char expected)
    {
        if (IsAtEnd()) return false;
        if (Source[NextPosition] != expected) return false;

        ReadChar();
        return true;
    }

    bool IsAtEnd()
    {
        return CurrentChar == '\0' || NextPosition >= Source.Length;
    }

    Token AddToken(TokenType type, string? lexeme = null, object? literal = null) =>
        new(type, lexeme, literal, Line, Column);

    Token InvalidToken()
    {
        Lox.Error(Line, Column, $"Unexpected character: {CurrentChar}");
        return new Token(Type: INVALID, Line: Line, Column: Column);
    }

    Token NewLine()
    {
        Line++;
        Column = 1;
        return new Token(Type: WHITESPACE, Lexeme: $"{CurrentChar}", Line: Line, Column: Column);
    }

    Token NextToken()
    {
        var token = CurrentChar switch
        {
            '(' => AddToken(LEFT_PAREN, "("),
            ')' => AddToken(RIGHT_PAREN, ")"),
            '{' => AddToken(LEFT_BRACE, "{"),
            '}' => AddToken(RIGHT_BRACE, "}"),
            ',' => AddToken(COMMA, ","),
            '.' => AddToken(DOT, "."),
            '-' => AddToken(MINUS, "-"),
            '+' => AddToken(PLUS, "+"),
            ';' => AddToken(SEMICOLON, ";"),
            '*' => AddToken(STAR, "*"),
            '!' when Match('=') => AddToken(BANG_EQUAL, "!="),
            '!' => AddToken(BANG, "!"),
            '=' when Match('=') => AddToken(EQUAL_EQUAL, "=="),
            '=' => AddToken(EQUAL, "="),
            '<' when Match('=') => AddToken(LESS_EQUAL, "<="),
            '<' => AddToken(LESS, "<"),
            '>' when Match('=') => AddToken(GREATER_EQUAL, ">="),
            '>' => AddToken(GREATER, ">"),
            '/' when Match('/') => ReadLineComment(),
            '/' when Match('*') => ReadBlockComment(),
            '/' => AddToken(SLASH, "/"),
            ' ' or '\r' or '\t' => AddToken(WHITESPACE),
            '\n' => NewLine(),
            '"' => ReadString(),
            '\0' => AddToken(EOF),
            _ when char.IsDigit(CurrentChar) => ReadNumber(),
            _ when IsAlphanumeric(CurrentChar) => ReadIdentifier(),
            _ => InvalidToken()
        };

        ReadChar();

        return token;
    }

    Token ReadLineComment()
    {
        while (Peek() != '\n' && !IsAtEnd())
            ReadChar();

        return new Token(Type: COMMENT, Line: Line, Column: Column);
    }

    Token ReadBlockComment()
    {
        while (Peek() != '*' && Peek(1) != '/' && !IsAtEnd())
        {
            if (Peek() == '\n')
            {
                Line++;
                Column = 1;
            }

            ReadChar();
        }

        if (IsAtEnd())
        {
            Lox.Error(Line, Column, "Unterminated block comment.");
            return new Token(Type: INVALID, Line: Line, Column: Column);
        }

        ReadChar(); // Consume '*'
        ReadChar(); // Consume '/'
        return new Token(Type: COMMENT, Line: Line, Column: Column);
    }

    Token ReadIdentifier()
    {
        var start = CurrentPosition;
        while (IsAlphanumeric(Peek())) ReadChar();

        var end = NextPosition;
        var lexeme = Source[start..end];

        return Keywords.TryGetValue(lexeme, out var type)
            ? new Token(Type: type, Lexeme: lexeme, Line: Line, Column: Column)
            : new Token(Type: IDENTIFIER, Lexeme: lexeme, Line: Line, Column: Column);
    }

    bool IsAlphanumeric(char c) => char.IsLetterOrDigit(c) || c == '_';

    Token ReadString()
    {
        var start = NextPosition;
        while (Peek() != '"' && !IsAtEnd())
        {
            if (Peek() == '\n')
            {
                Line++;
                Column = 1;
            }

            ReadChar();
        }

        if (IsAtEnd())
        {
            Lox.Error(Line, Column, "Unterminated string.");
            return new Token(Type: INVALID, Line: Line, Column: Column);
        }

        // Consume the closing "
        ReadChar();

        var end = CurrentPosition;
        var literal = Source[start..end];

        return new Token(Type: STRING, Lexeme: $"\"{literal}\"", Literal: literal, Line: Line,
            Column: Column);
    }

    Token ReadNumber()
    {
        var start = CurrentPosition;
        while (char.IsDigit(Peek())) ReadChar();

        if (Peek() == '.' && char.IsDigit(Peek(1)))
        {
            ReadChar();
            while (char.IsDigit(Peek()))
                ReadChar();
        }

        var end = NextPosition;

        var lexeme = Source[start..end];
        if (double.TryParse(lexeme, out var number))
        {
            return new Token(Type: NUMBER, Lexeme: lexeme, Literal: number, Line: Line, Column: Column);
        }

        Lox.Error(Line, Column, $"Invalid number: {lexeme}");
        return new Token(Type: INVALID, Lexeme: lexeme, Line: Line, Column: Column);
    }

    IEnumerable<Token> ScanTokens()
    {
        while (true)
        {
            var token = NextToken();
            if (token.Type == EOF)
            {
                yield return token;
                break;
            }

            if (token.Type is INVALID or COMMENT or WHITESPACE)
            {
                continue;
            }

            yield return token;
        }
    }
}