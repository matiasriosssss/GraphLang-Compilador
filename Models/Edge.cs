namespace GraphLang.Models;

public class Edge
{
    public Node Source { get; }
    public Node Target { get; }
    public int Weight { get; }
    public bool IsDirected { get; }

    public Edge(Node source, Node target, int weight = 1, bool isDirected = true)
    {
        Source = source ?? throw new ArgumentNullException(nameof(source));
        Target = target ?? throw new ArgumentNullException(nameof(target));
        Weight = weight;
        IsDirected = isDirected;
    }

    public override string ToString()
    {
        string arrow = IsDirected ? "->" : "--";
        return $"{Source.Name} {arrow} {Target.Name} (peso: {Weight})";
    }
}

