namespace GraphLang.Semantics;

public interface ICommand
{
    int Line { get; }
    int Column { get; }
    void Execute(GraphManager manager);
}

public class ShowCommand : ICommand
{
    public string GraphName { get; }
    public int Line { get; }
    public int Column { get; }

    public ShowCommand(string graphName, int line, int column)
    {
        GraphName = graphName;
        Line = line;
        Column = column;
    }

    public void Execute(GraphManager manager)
    {
        var graph = manager.GetGraph(GraphName, Line, Column);
        if (graph != null)
        {
            Visualization.GraphVisualizer.Visualize(graph);
        }
    }
}

public class TraverseCommand : ICommand
{
    public string GraphName { get; }
    public string StartNode { get; }
    public string Algorithm { get; } // "BFS" o "DFS"
    public int Line { get; }
    public int Column { get; }

    public TraverseCommand(string graphName, string startNode, string algorithm, int line, int column)
    {
        GraphName = graphName;
        StartNode = startNode;
        Algorithm = algorithm;
        Line = line;
        Column = column;
    }

    public void Execute(GraphManager manager)
    {
        var graph = manager.GetGraph(GraphName, Line, Column);
        if (graph == null) return;

        if (!graph.ContainsNode(StartNode))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[Error de Ejecución] El nodo '{StartNode}' no existe en el grafo '{GraphName}'.");
            Console.ResetColor();
            return;
        }

        Operations.GraphAlgorithms.Traverse(graph, StartNode, Algorithm);
    }
}

public class DijkstraCommand : ICommand
{
    public string GraphName { get; }
    public string StartNode { get; }
    public string EndNode { get; }
    public int Line { get; }
    public int Column { get; }

    public DijkstraCommand(string graphName, string startNode, string endNode, int line, int column)
    {
        GraphName = graphName;
        StartNode = startNode;
        EndNode = endNode;
        Line = line;
        Column = column;
    }

    public void Execute(GraphManager manager)
    {
        var graph = manager.GetGraph(GraphName, Line, Column);
        if (graph == null) return;

        if (!graph.ContainsNode(StartNode))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[Error de Ejecución] El nodo origen '{StartNode}' no existe en el grafo '{GraphName}'.");
            Console.ResetColor();
            return;
        }

        if (!graph.ContainsNode(EndNode))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[Error de Ejecución] El nodo destino '{EndNode}' no existe en el grafo '{GraphName}'.");
            Console.ResetColor();
            return;
        }

        Operations.GraphAlgorithms.Dijkstra(graph, StartNode, EndNode);
    }
}

public class DetectCyclesCommand : ICommand
{
    public string GraphName { get; }
    public int Line { get; }
    public int Column { get; }

    public DetectCyclesCommand(string graphName, int line, int column)
    {
        GraphName = graphName;
        Line = line;
        Column = column;
    }

    public void Execute(GraphManager manager)
    {
        var graph = manager.GetGraph(GraphName, Line, Column);
        if (graph == null) return;

        Operations.GraphAlgorithms.DetectCycles(graph);
    }
}
