using System.Diagnostics;
using System.Text;
using GraphLang.Models;

namespace GraphLang.Visualization;

public static class GraphVisualizer
{
    private static readonly string[] PossibleDotPaths = new[]
    {
        @"C:\Program Files\Graphviz\bin\dot.exe",
        @"C:\Program Files (x86)\Graphviz\bin\dot.exe",
        "dot.exe"
    };

    public static void Visualize(Graph graph)
    {
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine($"=== Visualización del Grafo '{graph.Name}' ({graph.Type}) ===");
        Console.ResetColor();

        // 1. Mostrar estructura textual clara en consola
        PrintAsciiAdjacency(graph);

        // 2. Generar archivo .dot para Graphviz
        string dotPath = Path.Combine(Directory.GetCurrentDirectory(), $"{graph.Name}.dot");
        string pngPath = Path.Combine(Directory.GetCurrentDirectory(), $"{graph.Name}.png");
        string htmlPath = Path.Combine(Directory.GetCurrentDirectory(), $"{graph.Name}.html");

        string dotContent = GenerateDot(graph);
        File.WriteAllText(dotPath, dotContent, new UTF8Encoding(false));
        Console.WriteLine($"[Graphviz] Archivo DOT generado: {dotPath}");

        // 3. Renderizar imagen con dot.exe si está instalado
        TryRenderWithGraphviz(dotPath, pngPath);

        // 4. Generar visor HTML interactivo
        string htmlContent = GenerateInteractiveHtml(graph);
        File.WriteAllText(htmlPath, htmlContent, Encoding.UTF8);
        Console.WriteLine($"[Visor Web] Archivo HTML interactivo generado: {htmlPath}");

        // Intentar abrir el visor HTML en el navegador por defecto
        TryOpenInBrowser(htmlPath);
    }

    private static void PrintAsciiAdjacency(Graph graph)
    {
        Console.WriteLine($"Nodos ({graph.Nodes.Count}): {string.Join(", ", graph.Nodes.Select(n => n.Name))}");
        Console.WriteLine($"Aristas ({graph.Edges.Count}):");
        foreach (var edge in graph.Edges)
        {
            Console.WriteLine($"  - {edge}");
        }
    }

    public static string GenerateDot(Graph graph)
    {
        var sb = new StringBuilder();
        bool isDirected = graph.Type == GraphType.Directed;
        string graphKeyword = isDirected ? "digraph" : "graph";
        string edgeOp = isDirected ? "->" : "--";

        sb.AppendLine($"{graphKeyword} {graph.Name} {{");
        sb.AppendLine("    rankdir=LR;");
        sb.AppendLine("    node [shape=circle, style=filled, fillcolor=\"#E3F2FD\", fontname=\"Helvetica\", color=\"#1565C0\"];");
        sb.AppendLine("    edge [fontname=\"Helvetica\", fontsize=10, color=\"#424242\"];");
        sb.AppendLine();

        // Nodos (incluso los aislados)
        foreach (var node in graph.Nodes)
        {
            sb.AppendLine($"    \"{node.Name}\";");
        }
        sb.AppendLine();

        // Aristas
        foreach (var edge in graph.Edges)
        {
            string label = edge.Weight > 1 ? $" [label=\" {edge.Weight}\", weight={edge.Weight}]" : "";
            sb.AppendLine($"    \"{edge.Source.Name}\" {edgeOp} \"{edge.Target.Name}\"{label};");
        }

        sb.AppendLine("}");
        return sb.ToString();
    }

    private static void TryRenderWithGraphviz(string dotPath, string pngPath)
    {
        string? dotExe = PossibleDotPaths.FirstOrDefault(File.Exists);
        if (dotExe == null)
        {
            Console.WriteLine("[Graphviz] dot.exe no encontrado en rutas estándar. Omitiendo generación de PNG.");
            return;
        }

        try
        {
            var psi = new ProcessStartInfo
            {
                FileName = dotExe,
                Arguments = $"-Tpng \"{dotPath}\" -o \"{pngPath}\"",
                UseShellExecute = false,
                CreateNoWindow = true
            };
            using var proc = Process.Start(psi);
            proc?.WaitForExit(5000);
            if (File.Exists(pngPath))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"[Graphviz] Imagen generada con éxito: {pngPath}");
                Console.ResetColor();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Graphviz] Aviso: no se pudo ejecutar dot: {ex.Message}");
        }
    }

    public static string GenerateInteractiveHtml(Graph graph)
    {
        bool isDirected = graph.Type == GraphType.Directed;

        var nodesJson = new StringBuilder("[");
        var edgesJson = new StringBuilder("[");

        int i = 0;
        foreach (var node in graph.Nodes)
        {
            if (i++ > 0) nodesJson.Append(",");
            nodesJson.Append($"{{\"id\":\"{node.Name}\",\"label\":\"{node.Name}\"}}");
        }
        nodesJson.Append("]");

        int j = 0;
        foreach (var edge in graph.Edges)
        {
            if (j++ > 0) edgesJson.Append(",");
            string arrows = isDirected ? ",\"arrows\":\"to\"" : "";
            string label = edge.Weight > 1 ? $",\"label\":\"{edge.Weight}\"" : "";
            edgesJson.Append($"{{\"from\":\"{edge.Source.Name}\",\"to\":\"{edge.Target.Name}\"{arrows}{label}}}");
        }
        edgesJson.Append("]");

        return $$"""
        <!DOCTYPE html>
        <html lang="es">
        <head>
            <meta charset="UTF-8">
            <title>Grafo: {{graph.Name}} (GraphLang)</title>
            <script type="text/javascript" src="https://unpkg.com/vis-network/standalone/umd/vis-network.min.js"></script>
            <style>
                body { font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; margin: 0; background: #f8fafc; }
                header { background: #1e293b; color: white; padding: 12px 24px; display: flex; justify-content: space-between; align-items: center; }
                h1 { margin: 0; font-size: 20px; }
                .badge { background: #3b82f6; color: white; padding: 4px 10px; border-radius: 9999px; font-size: 13px; }
                #network { width: 100vw; height: calc(100vh - 60px); border: none; }
            </style>
        </head>
        <body>
            <header>
                <h1>Grafo: {{graph.Name}}</h1>
                <span class="badge">{{(isDirected ? "DIRIGIDO" : "NO DIRIGIDO")}} | Nodos: {{graph.Nodes.Count}} | Aristas: {{graph.Edges.Count}}</span>
            </header>
            <div id="network"></div>
            <script>
                const nodes = new vis.DataSet({{nodesJson}});
                const edges = new vis.DataSet({{edgesJson}});
                const container = document.getElementById('network');
                const data = { nodes: nodes, edges: edges };
                const options = {
                    nodes: {
                        shape: 'circle',
                        color: { background: '#93c5fd', border: '#2563eb', highlight: { background: '#60a5fa', border: '#1d4ed8' } },
                        font: { size: 16, color: '#0f172a', bold: true }
                    },
                    edges: {
                        font: { align: 'top', size: 13, color: '#334155' },
                        color: { color: '#64748b', highlight: '#0284c7' },
                        smooth: { type: 'continuous' }
                    },
                    physics: {
                        stabilization: true,
                        barnesHut: { springLength: 120 }
                    }
                };
                new vis.Network(container, data, options);
            </script>
        </body>
        </html>
        """;
    }

    private static void TryOpenInBrowser(string filePath)
    {
        try
        {
            var psi = new ProcessStartInfo
            {
                FileName = filePath,
                UseShellExecute = true
            };
            Process.Start(psi);
        }
        catch
        {
            // Si no se puede abrir el navegador automáticamente (entorno sin GUI o headless), no pasa nada
        }
    }
}
