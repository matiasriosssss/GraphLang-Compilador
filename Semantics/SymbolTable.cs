namespace GraphLang.Semantics;

using GraphLang.Models;

/// <summary>
/// Clase base para cualquier identificador registrado en la Tabla de Símbolos.
/// </summary>
public abstract class Symbol
{
    public string Name { get; }
    public int Line { get; }
    public int Column { get; }

    protected Symbol(string name, int line, int column)
    {
        Name = name;
        Line = line;
        Column = column;
    }
}

/// <summary>
/// Símbolo que representa un Grafo en el Ámbito Global.
/// </summary>
public class GraphSymbol : Symbol
{
    public GraphType Type { get; }
    public Graph GraphInstance { get; }
    public SymbolTable LocalScope { get; }

    public GraphSymbol(string name, GraphType type, Graph graphInstance, int line, int column)
        : base(name, line, column)
    {
        Type = type;
        GraphInstance = graphInstance;
        LocalScope = new SymbolTable(name, null);
    }
}

/// <summary>
/// Símbolo que representa un Nodo en el Ámbito Local de un Grafo específico.
/// </summary>
public class NodeSymbol : Symbol
{
    public Node NodeInstance { get; }

    public NodeSymbol(string name, Node nodeInstance, int line, int column)
        : base(name, line, column)
    {
        NodeInstance = nodeInstance;
    }
}

/// <summary>
/// Tabla de Símbolos con soporte de Ámbitos (Scopes).
/// Permite aislar identificadores locales (nodos de cada grafo) del ámbito global (grafos).
/// </summary>
public class SymbolTable
{
    public string ScopeName { get; }
    public SymbolTable? Parent { get; }
    private readonly Dictionary<string, Symbol> _symbols = new(StringComparer.Ordinal);

    public IReadOnlyDictionary<string, Symbol> Symbols => _symbols;

    public SymbolTable(string scopeName, SymbolTable? parent = null)
    {
        ScopeName = scopeName;
        Parent = parent;
    }

    /// <summary>
    /// Intenta registrar un símbolo en el ámbito actual.
    /// Retorna false si ya existe un símbolo con ese nombre en este ámbito.
    /// </summary>
    public bool Define(Symbol symbol)
    {
        if (_symbols.ContainsKey(symbol.Name))
            return false;

        _symbols[symbol.Name] = symbol;
        return true;
    }

    /// <summary>
    /// Busca un símbolo en el ámbito actual.
    /// </summary>
    public Symbol? Resolve(string name)
    {
        if (_symbols.TryGetValue(name, out var sym))
            return sym;

        return Parent?.Resolve(name);
    }

    /// <summary>
    /// Comprueba si el símbolo existe en este ámbito específico (sin buscar en padres).
    /// </summary>
    public bool ContainsLocally(string name) => _symbols.ContainsKey(name);
}

