namespace CSharpLox.Interpreter.Functions;

public class ReadFile : ICallable
{
    public object Call(LoxInterpreter loxInterpreter, List<object> arguments)
    {
        if (arguments[0] is not string filePath)
            throw new RuntimeError(new Token(TokenType.READ_FILE, "read_file"),
                "read_file() argument must be a string.");

        return File.ReadAllText(filePath);
    }

    public int Arity() => 1;
    public override string ToString() => "<native fn>";
}