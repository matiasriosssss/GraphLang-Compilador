using GraphLang.Models;

namespace GraphLang.Operations;

public static class GraphAlgorithms
{
    public static void Traverse(Graph graph, string startNodeName, string algorithm)
    {
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"=== Recorrido {algorithm.ToUpperInvariant()} en Grafo '{graph.Name}' desde '{startNodeName}' ===");
        Console.ResetColor();

        if (algorithm.Equals("BFS", StringComparison.OrdinalIgnoreCase))
        {
            Bfs(graph, startNodeName);
        }
        else if (algorithm.Equals("DFS", StringComparison.OrdinalIgnoreCase))
        {
            Dfs(graph, startNodeName);
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Algoritmo '{algorithm}' no reconocido. Utilice BFS o DFS.");
            Console.ResetColor();
        }
    }

    private static void Bfs(Graph graph, string startNodeName)
    {
        var visited = new HashSet<string>(StringComparer.Ordinal);
        var queue = new Queue<string>();
        var order = new List<string>();

        visited.Add(startNodeName);
        queue.Enqueue(startNodeName);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            order.Add(current);

            foreach (var neighbor in graph.GetNeighbors(current))
            {
                if (!visited.Contains(neighbor.Name))
                {
                    visited.Add(neighbor.Name);
                    queue.Enqueue(neighbor.Name);
                }
            }
        }

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"Orden de visita (BFS): {string.Join(" -> ", order)}");
        Console.WriteLine($"Total de nodos alcanzados: {order.Count} de {graph.Nodes.Count}");
        Console.ResetColor();
    }

    private static void Dfs(Graph graph, string startNodeName)
    {
        var visited = new HashSet<string>(StringComparer.Ordinal);
        var order = new List<string>();

        DfsRecursive(graph, startNodeName, visited, order);

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"Orden de visita (DFS): {string.Join(" -> ", order)}");
        Console.WriteLine($"Total de nodos alcanzados: {order.Count} de {graph.Nodes.Count}");
        Console.ResetColor();
    }

    private static void DfsRecursive(Graph graph, string current, HashSet<string> visited, List<string> order)
    {
        visited.Add(current);
        order.Add(current);

        foreach (var neighbor in graph.GetNeighbors(current))
        {
            if (!visited.Contains(neighbor.Name))
            {
                DfsRecursive(graph, neighbor.Name, visited, order);
            }
        }
    }

    /// <summary>
    /// Algoritmo de Dijkstra para calcular el camino de costo mínimo entre dos nodos.
    /// </summary>
    public static void Dijkstra(Graph graph, string startNodeName, string endNodeName)
    {
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"=== Algoritmo de Dijkstra (Camino Mínimo) en '{graph.Name}' ===");
        Console.WriteLine($"Origen: '{startNodeName}'  -->  Destino: '{endNodeName}'");
        Console.ResetColor();

        var distances = new Dictionary<string, int>(StringComparer.Ordinal);
        var previous = new Dictionary<string, (string fromNode, int weight)?>(StringComparer.Ordinal);
        var priorityQueue = new PriorityQueue<string, int>();
        var settled = new HashSet<string>(StringComparer.Ordinal);

        foreach (var node in graph.Nodes)
        {
            distances[node.Name] = int.MaxValue;
            previous[node.Name] = null;
        }

        distances[startNodeName] = 0;
        priorityQueue.Enqueue(startNodeName, 0);

        while (priorityQueue.Count > 0)
        {
            var u = priorityQueue.Dequeue();
            if (!settled.Add(u)) continue;

            if (u == endNodeName) break;

            foreach (var edge in graph.GetOutgoingEdges(u))
            {
                string v = edge.Target.Name;
                if (settled.Contains(v)) continue;

                int alt = distances[u] + edge.Weight;
                if (alt < distances[v])
                {
                    distances[v] = alt;
                    previous[v] = (u, edge.Weight);
                    priorityQueue.Enqueue(v, alt);
                }
            }
        }

        if (distances[endNodeName] == int.MaxValue)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"[Resultado]: No existe ningún camino entre '{startNodeName}' y '{endNodeName}'.");
            Console.ResetColor();
            return;
        }

        // Reconstrucción del camino
        var path = new List<string>();
        string curr = endNodeName;
        while (curr != null)
        {
            path.Insert(0, curr);
            var prevEntry = previous[curr];
            curr = prevEntry?.fromNode;
        }

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"Ruta óptima encontrada: {string.Join(" -> ", path)}");
        Console.WriteLine($"Costo mínimo total: {distances[endNodeName]}");
        Console.ResetColor();
    }

    /// <summary>
    /// Algoritmo de detección de ciclos adaptado según si el grafo es dirigido o no dirigido.
    /// </summary>
    public static void DetectCycles(Graph graph)
    {
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"=== Detección de Ciclos en Grafo '{graph.Name}' ({graph.Type}) ===");
        Console.ResetColor();

        bool hasCycle;
        if (graph.Type == GraphType.Directed)
        {
            hasCycle = DetectCyclesDirected(graph);
        }
        else
        {
            hasCycle = DetectCyclesUndirected(graph);
        }

        if (hasCycle)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"[Resultado]: El grafo '{graph.Name}' CONTIENE al menos un ciclo.");
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[Resultado]: El grafo '{graph.Name}' es ACÍCLICO (no contiene ciclos).");
        }
        Console.ResetColor();
    }

    private static bool DetectCyclesDirected(Graph graph)
    {
        // 0 = White (no visitado), 1 = Gray (en pila de llamada), 2 = Black (completado)
        var color = new Dictionary<string, int>(StringComparer.Ordinal);
        foreach (var node in graph.Nodes)
            color[node.Name] = 0;

        foreach (var node in graph.Nodes)
        {
            if (color[node.Name] == 0)
            {
                if (DfsDirectedCycle(graph, node.Name, color))
                    return true;
            }
        }
        return false;
    }

    private static bool DfsDirectedCycle(Graph graph, string u, Dictionary<string, int> color)
    {
        color[u] = 1; // Gray

        foreach (var edge in graph.GetOutgoingEdges(u))
        {
            string v = edge.Target.Name;
            if (color[v] == 1) return true; // Ciclo encontrado hacia un ancestro
            if (color[v] == 0 && DfsDirectedCycle(graph, v, color)) return true;
        }

        color[u] = 2; // Black
        return false;
    }

    private static bool DetectCyclesUndirected(Graph graph)
    {
        var visited = new HashSet<string>(StringComparer.Ordinal);

        foreach (var node in graph.Nodes)
        {
            if (!visited.Contains(node.Name))
            {
                if (DfsUndirectedCycle(graph, node.Name, null, visited))
                    return true;
            }
        }
        return false;
    }

    private static bool DfsUndirectedCycle(Graph graph, string u, string? parent, HashSet<string> visited)
    {
        visited.Add(u);

        foreach (var edge in graph.GetOutgoingEdges(u))
        {
            string v = edge.Target.Name;
            if (!visited.Contains(v))
            {
                if (DfsUndirectedCycle(graph, v, u, visited))
                    return true;
            }
            else if (v != parent)
            {
                return true; // Encontró un nodo ya visitado que no es el padre inmediato
            }
        }
        return false;
    }
}
