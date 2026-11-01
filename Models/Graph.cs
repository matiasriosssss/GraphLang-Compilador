namespace GraphLang.Models;

public class Graph
{
    public string Name { get; }
    public GraphType Type { get; }

    private readonly Dictionary<string, Node> _nodes = new(StringComparer.Ordinal);
    private readonly Dictionary<string, List<Edge>> _adjacency = new(StringComparer.Ordinal);
    private readonly List<Edge> _edges = new();

    public IReadOnlyCollection<Node> Nodes => _nodes.Values;
    public IReadOnlyList<Edge> Edges => _edges;

    public Graph(string name, GraphType type)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Type = type;
    }

    public bool ContainsNode(string name) => _nodes.ContainsKey(name);

    public Node? GetNode(string name) => _nodes.TryGetValue(name, out var node) ? node : null;

    public bool AddNode(string name)
    {
        if (_nodes.ContainsKey(name))
            return false;

        var node = new Node(name);
        _nodes[name] = node;
        _adjacency[name] = new List<Edge>();
        return true;
    }

    public Edge AddEdge(string sourceName, string targetName, int weight, bool isDirected)
    {
        if (!_nodes.TryGetValue(sourceName, out var sourceNode))
            throw new InvalidOperationException($"El nodo origen '{sourceName}' no existe en el grafo '{Name}'.");

        if (!_nodes.TryGetValue(targetName, out var targetNode))
            throw new InvalidOperationException($"El nodo destino '{targetName}' no existe en el grafo '{Name}'.");

        var edge = new Edge(sourceNode, targetNode, weight, isDirected);
        _edges.Add(edge);
        _adjacency[sourceName].Add(edge);

        // Si el grafo no es dirigido, la conexión es bidireccional en la lista de adyacencia
        if (!isDirected)
        {
            var reverseEdge = new Edge(targetNode, sourceNode, weight, false);
            _adjacency[targetName].Add(reverseEdge);
        }

        return edge;
    }

    public IReadOnlyList<Edge> GetOutgoingEdges(string nodeName)
    {
        if (_adjacency.TryGetValue(nodeName, out var edges))
            return edges;
        return Array.Empty<Edge>();
    }

    public IReadOnlyList<Node> GetNeighbors(string nodeName)
    {
        if (_adjacency.TryGetValue(nodeName, out var edges))
        {
            return edges.Select(e => e.Target).Distinct().ToList();
        }
        return Array.Empty<Node>();
    }
}

