namespace CSharpLox.Interpreter.Functions;

public class AppendFile : ICallable
{
    public object? Call(LoxInterpreter loxInterpreter, List<object> arguments)
    {
        if (arguments[0] is not string filePath)
            throw new RuntimeError(new Token(TokenType.APPEND_FILE, "append_file"),
                "append_file() first argument must be a string.");

        if (arguments[1] is not string content)
            throw new RuntimeError(new Token(TokenType.APPEND_FILE, "append_file"),
                "append_file() second argument must be a string.");

        File.AppendAllText(filePath, content);
        return null;
    }

    public int Arity()
    {
        return 2;
    }

    public override string ToString() => "<native fn>";
}