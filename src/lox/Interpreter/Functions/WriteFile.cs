namespace CSharpLox.Interpreter.Functions;

public class WriteFile : ICallable
{
    public object? Call(LoxInterpreter loxInterpreter, List<object> arguments)
    {
        if (arguments[0] is not string filePath)
            throw new RuntimeError(new Token(TokenType.WRITE_FILE, "write_file"),
                "write_file() first argument must be a string.");

        if (arguments[1] is not string content)
            throw new RuntimeError(new Token(TokenType.WRITE_FILE, "write_file"),
                "write_file() second argument must be a string.");

        File.WriteAllText(filePath, content);
        return null;
    }

    public int Arity() => 2;

    public override string ToString() => "<native fn>";
}