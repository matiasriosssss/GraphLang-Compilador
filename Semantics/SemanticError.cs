namespace GraphLang.Semantics;

public class SemanticError
{
    public int Line { get; }
    public int Column { get; }
    public string Message { get; }

    public SemanticError(int line, int column, string message)
    {
        Line = line;
        Column = column;
        Message = message;
    }

    public override string ToString()
    {
        return $"[Error Semántico] Línea {Line}, Columna {Column}: {Message}";
    }
}

