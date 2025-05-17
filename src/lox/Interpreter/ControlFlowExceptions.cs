namespace CSharpLox.Interpreter;

public class ReturnException(object? value) : Exception
{
    public object? Value { get; } = value;
}

public class ContinueException(Token token) : Exception
{
    public Token Token { get; } = token;
}

public class BreakException(Token token) : Exception
{
    public Token Token { get; } = token;
}