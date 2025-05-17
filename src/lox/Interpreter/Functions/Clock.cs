namespace CSharpLox.Interpreter.Functions;

public class Clock : ICallable
{
    public object Call(LoxInterpreter loxInterpreter, List<object> arguments) =>
        (double)DateTimeOffset.Now.ToUnixTimeSeconds();

    public int Arity() => 0;
    public override string ToString() => "<native fn>";
}