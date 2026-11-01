using GraphLang.Models;

namespace GraphLang.Semantics;

public class GraphManager
{
    private readonly SymbolTable _globalScope = new("Global");
    private readonly Dictionary<string, Graph> _graphs = new(StringComparer.Ordinal);
    private readonly List<SemanticError> _errors = new();
    private readonly List<ICommand> _commands = new();

    public SymbolTable GlobalScope => _globalScope;
    public IReadOnlyCollection<Graph> Graphs => _graphs.Values;
    public IReadOnlyList<SemanticError> Errors => _errors;
    public IReadOnlyList<ICommand> Commands => _commands;

    public bool HasErrors => _errors.Count > 0;

    public void ReportError(int line, int col, string message)
    {
        _errors.Add(new SemanticError(line, col, message));
    }

    public Graph? DeclareGraph(string name, GraphType type, int line, int col)
    {
        // 1. Verificación en la Tabla de Símbolos Global
        if (_globalScope.ContainsLocally(name))
        {
            ReportError(line, col, $"El grafo '{name}' ya ha sido declarado previamente en el ámbito global.");
            return null;
        }

        var graph = new Graph(name, type);
        var graphSymbol = new GraphSymbol(name, type, graph, line, col);
        _globalScope.Define(graphSymbol);
        _graphs[name] = graph;
        return graph;
    }

    public void AddNodeToGraph(Graph graph, string nodeName, int line, int col)
    {
        // Buscar el símbolo del grafo para acceder a su ámbito local
        var graphSym = _globalScope.Resolve(graph.Name) as GraphSymbol;
        var localScope = graphSym?.LocalScope;

        // 2. Verificación en la Tabla de Símbolos Local del Grafo
        if (localScope != null && localScope.ContainsLocally(nodeName))
        {
            ReportError(line, col, $"El nodo '{nodeName}' ya está declarado dentro del ámbito local del grafo '{graph.Name}'.");
            return;
        }

        graph.AddNode(nodeName);
        var nodeObj = graph.GetNode(nodeName);
        if (nodeObj != null && localScope != null)
        {
            localScope.Define(new NodeSymbol(nodeName, nodeObj, line, col));
        }
    }

    public void AddEdgeToGraph(Graph graph, string source, string target, bool isDirectedEdge, int weight, int line, int col)
    {
        bool hasError = false;

        // Validación de compatibilidad con el tipo de grafo
        if (graph.Type == GraphType.Directed && !isDirectedEdge)
        {
            ReportError(line, col, $"Operador '--' inválido: el grafo '{graph.Name}' es DIRIGIDO. Debe usar '->'.");
            hasError = true;
        }
        else if (graph.Type == GraphType.Undirected && isDirectedEdge)
        {
            ReportError(line, col, $"Operador '->' inválido: el grafo '{graph.Name}' es NO_DIRIGIDO. Debe usar '--'.");
            hasError = true;
        }

        // Resolución en la Tabla de Símbolos Local
        var graphSym = _globalScope.Resolve(graph.Name) as GraphSymbol;
        var localScope = graphSym?.LocalScope;

        if (localScope != null)
        {
            if (!localScope.ContainsLocally(source))
            {
                ReportError(line, col, $"El nodo origen '{source}' no está declarado en la tabla de símbolos del grafo '{graph.Name}'.");
                hasError = true;
            }

            if (!localScope.ContainsLocally(target))
            {
                ReportError(line, col, $"El nodo destino '{target}' no está declarado en la tabla de símbolos del grafo '{graph.Name}'.");
                hasError = true;
            }
        }
        else
        {
            if (!graph.ContainsNode(source))
            {
                ReportError(line, col, $"El nodo origen '{source}' no existe en el grafo '{graph.Name}'.");
                hasError = true;
            }
            if (!graph.ContainsNode(target))
            {
                ReportError(line, col, $"El nodo destino '{target}' no existe en el grafo '{graph.Name}'.");
                hasError = true;
            }
        }

        // Validación de peso
        if (weight <= 0)
        {
            ReportError(line, col, $"El peso de la arista debe ser un entero positivo mayor que cero (se recibió: {weight}).");
            hasError = true;
        }

        if (!hasError)
        {
            graph.AddEdge(source, target, weight, isDirectedEdge);
        }
    }

    public bool ValidateGraphExists(string name, int line, int col)
    {
        if (!_globalScope.ContainsLocally(name))
        {
            ReportError(line, col, $"El grafo '{name}' no ha sido declarado en la tabla de símbolos global.");
            return false;
        }
        return true;
    }

    public void ValidateNodeExistsInGraph(string graphName, string nodeName, int line, int col)
    {
        var graphSym = _globalScope.Resolve(graphName) as GraphSymbol;
        if (graphSym != null && !graphSym.LocalScope.ContainsLocally(nodeName))
        {
            ReportError(line, col, $"El nodo '{nodeName}' no existe dentro del ámbito local del grafo '{graphName}'.");
        }
    }

    public Graph? GetGraph(string name, int line, int col)
    {
        var sym = _globalScope.Resolve(name) as GraphSymbol;
        if (sym == null)
        {
            ReportError(line, col, $"El grafo '{name}' no existe en la tabla de símbolos.");
            return null;
        }
        return sym.GraphInstance;
    }

    public void AddCommand(ICommand command)
    {
        _commands.Add(command);
    }

    public void ExecuteCommands()
    {
        if (HasErrors)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("No se pueden ejecutar las operaciones debido a errores semánticos:");
            foreach (var err in _errors)
            {
                Console.WriteLine(err);
            }
            Console.ResetColor();
            return;
        }

        foreach (var cmd in _commands)
        {
            cmd.Execute(this);
        }
    }
}
