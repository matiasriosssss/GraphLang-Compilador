namespace GraphLang.Models;

public class Node : IEquatable<Node>
{
    public string Name { get; }

    public Node(string name)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }

    public bool Equals(Node? other)
    {
        if (other is null) return false;
        return string.Equals(Name, other.Name, StringComparison.Ordinal);
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as Node);
    }

    public override int GetHashCode()
    {
        return StringComparer.Ordinal.GetHashCode(Name);
    }

    public override string ToString() => Name;
}

