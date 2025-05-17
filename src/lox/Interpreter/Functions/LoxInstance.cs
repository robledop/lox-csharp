namespace CSharpLox.Interpreter.Functions;

public class LoxInstance(LoxClass klass)
{
    LoxClass Klass { get; init; } = klass;
    readonly Dictionary<string, object?> _fields = new();

    public object? Get(Token name)
    {
        ArgumentException.ThrowIfNullOrEmpty(name.Lexeme);
        if (_fields.TryGetValue(name.Lexeme, out var value))
            return value;

        var method = Klass.FindMethod(name.Lexeme);
        if (method != null)
            return method.Bind(this);

        throw new RuntimeError(name, $"Undefined property '{name.Lexeme}'.");
    }

    public void Set(Token name, object? value) => _fields[name.Lexeme ?? throw new InvalidOperationException()] = value;

    public override string ToString() => $"{Klass.Name} instance";
}