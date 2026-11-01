using GraphLang.Generated;

namespace GraphLang;

class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("=================================================");
        Console.WriteLine("       Compilador GraphLang - Versión 1.0        ");
        Console.WriteLine("  Lenguaje para Definición y Análisis de Grafos  ");
        Console.WriteLine("=================================================");
        Console.ResetColor();

        if (args.Length > 0 && (args[0] == "--generate-docs" || args[0] == "docs" || args[0] == "-d"))
        {
            Docs.DocumentGenerator.GenerateAll(Directory.GetCurrentDirectory());
            return;
        }

        string sourceFile;

        if (args.Length > 0)
        {
            sourceFile = args[0];
        }
        else
        {
            // Archivo de prueba por defecto
            sourceFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Examples", "demo.gl");
            if (!File.Exists(sourceFile))
            {
                sourceFile = Path.Combine(Directory.GetCurrentDirectory(), "Examples", "demo.gl");
            }
        }

        if (!File.Exists(sourceFile))
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Aviso: No se proporcionó ningún archivo existente.");
            Console.WriteLine($"Uso: dotnet run -- <archivo.gl>");
            Console.ResetColor();
            return;
        }

        Console.WriteLine($"Procesando archivo: {sourceFile}\n");

        try
        {
            // 1. Fase de Análisis Léxico (Scanner)
            var scanner = new Scanner(sourceFile);

            // 2. Fase de Análisis Sintáctico (Parser) y Semántico
            var parser = new Parser(scanner);

            Console.WriteLine("-> Iniciando análisis léxico y sintáctico...");
            parser.Parse();

            // 3. Verificación de errores sintácticos
            if (parser.errors.count > 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n[Fallo de Compilación] Se detectaron {parser.errors.count} error(es) léxico(s)/sintáctico(s).");
                Console.ResetColor();
                Environment.ExitCode = 1;
                return;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("-> Análisis sintáctico completado sin errores.");
            Console.ResetColor();

            // 4. Verificación de errores semánticos
            Console.WriteLine("-> Verificando coherencia semántica...");
            if (parser.Manager.HasErrors)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n[Fallo Semántico] Se detectaron {parser.Manager.Errors.Count} error(es) semántico(s):");
                foreach (var err in parser.Manager.Errors)
                {
                    Console.WriteLine($"   {err}");
                }
                Console.ResetColor();
                Environment.ExitCode = 2;
                return;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("-> Análisis semántico completado sin errores.");
            Console.ResetColor();

            // 5. Ejecución de instrucciones (Mostrar grafos, recorridos, etc.)
            Console.WriteLine("-> Ejecutando instrucciones del programa...");
            parser.Manager.ExecuteCommands();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n=== Compilación y ejecución finalizada con éxito ===");
            Console.ResetColor();
        }
        catch (FatalError fe)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[Error Fatal]: {fe.Message}");
            Console.ResetColor();
            Environment.ExitCode = 3;
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[Error Inesperado]: {ex.Message}");
            Console.ResetColor();
            Environment.ExitCode = 4;
        }
    }
}
