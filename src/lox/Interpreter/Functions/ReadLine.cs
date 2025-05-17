namespace CSharpLox.Interpreter.Functions;

public class ReadLine : ICallable
{
    public object? Call(LoxInterpreter loxInterpreter, List<object> arguments) => Console.ReadLine();
    public int Arity() => 0;
    public override string ToString() => "<native fn>";
}