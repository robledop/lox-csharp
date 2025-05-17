namespace CSharpLox.Interpreter;

public class LoxEnvironment
{
    public LoxEnvironment? Enclosing { get; set; }
    readonly Dictionary<string, object?> _values = new();

    public LoxEnvironment(LoxEnvironment? enclosing = null) => Enclosing = enclosing;

    public void Define(string name, object? value) => _values[name] = value;

    public object? Get(Token token)
    {
        ArgumentNullException.ThrowIfNull(token.Lexeme);
        if (_values.TryGetValue(token.Lexeme, out var value))
            return value;

        throw new RuntimeError(token, $"Undefined variable '{token.Lexeme}'.");
    }

    public object? GetAt(int distance, Token token) => Ancestor(distance, token).Get(token);

    public void Assign(Token name, object? value)
    {
        ArgumentNullException.ThrowIfNull(name.Lexeme);
        if (!_values.ContainsKey(name.Lexeme))
            throw new RuntimeError(name, $"Undefined variable '{name.Lexeme}'.");

        _values[name.Lexeme!] = value;
    }

    public void AssignAt(int distance, Token token, object? value) => Ancestor(distance, token).Assign(token, value);

    LoxEnvironment Ancestor(int distance, Token token)
    {
        var environment = this;
        for (var i = 0; i < distance; i++)
            environment = environment.Enclosing ?? throw new RuntimeError(token, "Enclosing environment not found.");

        return environment;
    }
}