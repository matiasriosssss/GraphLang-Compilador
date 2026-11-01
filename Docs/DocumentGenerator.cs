namespace GraphLang.Docs;

public static class DocumentGenerator
{
    public static void GenerateAll(string outputDir)
    {
        Console.WriteLine("\n-> Generando documentación académica y archivos solicitados...");

        string informePdfPath = Path.Combine(outputDir, "Informe_Compilador_GraphLang.pdf");
        string manualPdfPath = Path.Combine(outputDir, "Manual_Usuario_GraphLang.pdf");
        string presentacionPptxPath = Path.Combine(outputDir, "Presentacion_GraphLang.pptx");

        GenerateInformePdf(informePdfPath);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"[PDF Creado]: {informePdfPath}");
        Console.ResetColor();

        GenerateManualPdf(manualPdfPath);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"[PDF Creado]: {manualPdfPath}");
        Console.ResetColor();

        GeneratePresentacionPptx(presentacionPptxPath);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"[PPTX Creado]: {presentacionPptxPath}");
        Console.ResetColor();
    }

    private static void GenerateInformePdf(string filePath)
    {
        var pdf = new PdfWriter("Informe Academico: Compilador GraphLang");

        // PORTADA / ENCABEZADO
        pdf.AddTitle(
            "Desarrollo de un Compilador para Grafos (GraphLang)",
            "Diseno, Teoria Formal, Implementacion con Coco/R y C#, y Tabla de Simbolos Jerarquica",
            "Materia: Compiladores | Catedra Universitaria | Ano Academico 2026"
        );

        // SECCIÓN 1: INTRODUCCIÓN
        pdf.AddHeading1("1. Introduccion y Objetivos del Proyecto");
        pdf.AddParagraph(
            "El objetivo central de este proyecto universitario es la creacion de un compilador funcional " +
            "para un lenguaje de dominio especifico (DSL) denominado GraphLang. Este lenguaje permite definir " +
            "estructuras no lineales de datos (grafos dirigidos y no dirigidos), establecer conexiones entre nodos " +
            "con pesos opcionales, ejecutar recorridos analiticos (BFS y DFS), calcular rutas minimas mediante " +
            "el algoritmo de Dijkstra y realizar deteccion de ciclos, complementando todo con visualizacion grafica automatica."
        );
        pdf.AddParagraph(
            "A diferencia de un interprete secuencial generico, GraphLang demuestra de manera tangible las etapas fundamentales " +
            "de la teoria de construccion de compiladores: analisis lexico (Scanner), analisis sintactico (Parser), " +
            "analisis semantico con gestion de ambitos mediante Tablas de Simbolos, construccion de estructuras internas " +
            "(lista de adyacencia) y generacion de representaciones visuales."
        );

        // SECCIÓN 2: ¿CÓMO FUNCIONA COCO/R?
        pdf.AddHeading1("2. Fundamentos de Coco/R: Scanner y Parser");
        pdf.AddParagraph(
            "Coco/R es un generador de compiladores desarrollado en la Universidad Johannes Kepler de Linz. " +
            "A partir de una unica especificacion gramatical con atributos (.atg - Attributed Translation Grammar), " +
            "Coco/R genera automaticamente dos clases centrales en lenguaje C#:"
        );
        pdf.AddBullet("Scanner.cs (Analizador Lexico): Convierte el flujo continuo de caracteres del archivo fuente en una secuencia ordenada de unidades logicas llamadas Tokens.");
        pdf.AddBullet("Parser.cs (Analizador Sintactico): Reconoce si la secuencia de tokens respeta la estructura sintactica definida mediante el metodo de Descenso Recursivo LL(1).");
        pdf.AddParagraph(
            "El proceso de consumo de tokens funciona de la siguiente manera: el Parser mantiene dos referencias principales: 't' (el ultimo token aceptado) " +
            "y 'la' (lookahead, el proximo token en espera sin consumir). Mediante la primitiva Get(), el Parser le pide al Scanner el siguiente token, " +
            "y con Expect(token) valida que el token recibido coincida con lo esperado por la gramatica. Si coincide, avanza; si no coincide, emite un error sintactico con linea y columna."
        );

        // SECCIÓN 3: TABLA DE SÍMBOLOS
        pdf.AddHeading1("3. La Tabla de Simbolos y Gestion de Ambitos (Scopes)");
        pdf.AddCallout("CONCEPTO CLAVE PARA LA MATERIA",
            "La Tabla de Simbolos es la estructura de datos que almacena los identificadores declarados junto a sus atributos y tipos. " +
            "En GraphLang implementamos una jerarquia de ambitos: un Ambito Global para los grafos y Ambitos Locales para los nodos de cada grafo.", false);
        pdf.AddParagraph(
            "En nuestro diseno existen dos niveles estrictos de alcance (scopes):"
        );
        pdf.AddBullet("Ambito Global (Global Scope): Contiene los simbolos de los grafos (GraphSymbol). Almacena el nombre del grafo, su tipo (DIRIGIDO o NO_DIRIGIDO) y la referencia a su propia tabla local. Detecta errores si dos grafos se declaran con el mismo identificador.");
        pdf.AddBullet("Ambito Local (Local Scope por Grafo): Cada bloque GRAFO abre un nuevo espacio de nombres. Los nodos declarados con 'NODOS' se registran en esta tabla (NodeSymbol). Esto garantiza aislamiento: un nodo 'A' en el grafo Red1 es totalmente independiente de un nodo 'A' en el grafo Red2.");
        pdf.AddBullet("Resolucion de Aristas y Referencias: Al procesar 'ARISTA A -> B', el compilador consulta la tabla de simbolos del grafo activo para resolver si 'A' y 'B' existen. Si alguno no fue declarado, se emite un error semantico claro impidiendo la ejecucion.");

        // SECCIÓN 4: FASES DEL COMPILADOR
        pdf.AddHeading1("4. Fases de Compilacion en GraphLang");
        pdf.AddParagraph(
            "El pipeline de compilacion sigue una secuencia rigurosa y ordenada en cinco pasos:"
        );
        pdf.AddBullet("Paso 1 (Lexico): El Scanner ignora espacios, saltos de linea y comentarios (// y /* */). Reconoce palabras reservadas (GRAFO, DIRIGIDO, NODOS, ARISTA, PESO, MOSTRAR, RECORRER, CAMINO, CICLOS), simbolos (->, --, {, }, ;, ,) y literales (identificadores y numeros enteros).");
        pdf.AddBullet("Paso 2 (Sintactico): El Parser LL(1) evalua las producciones sin ambiguedades. Detecta errores estructurales como la falta de punto y coma, llaves sin cerrar o palabras clave mal posicionadas.");
        pdf.AddBullet("Paso 3 (Semantico): GraphManager verifica que las aristas no usen nodos inexistentes, que el operador '->' solo se use en grafos DIRIGIDOS y '--' en grafos NO_DIRIGIDOS, y que los pesos sean enteros positivos.");
        pdf.AddBullet("Paso 4 (Modelo Interno): Si no hay errores, se construye una Lista de Adyacencia en memoria. En grafos no dirigidos, la conexion se refleja simetricamente en ambas direcciones.");
        pdf.AddBullet("Paso 5 (Ejecucion y Visualizacion): Se ejecutan los comandos del usuario generando archivos DOT/PNG con Graphviz y visores HTML interactivos.");

        // SECCIÓN 5: OPERACIONES Y ALGORITMOS
        pdf.AddHeading1("5. Algoritmos y Operaciones Implementadas");
        pdf.AddParagraph(
            "El compilador soporta cuatro operaciones no lineales sobre los grafos:"
        );
        pdf.AddBullet("BFS (Busqueda en Anchura): Explora el grafo por niveles utilizando una cola (Queue), garantizando encontrar la menor cantidad de aristas desde el origen.");
        pdf.AddBullet("DFS (Busqueda en Profundidad): Explora recursivamente cada rama hasta el fondo antes de retroceder, utilizando una pila o recursion.");
        pdf.AddBullet("Algoritmo de Dijkstra: Calcula la ruta de costo minimo entre dos nodos teniendo en cuenta los pesos de las aristas, utilizando una cola de prioridad.");
        pdf.AddBullet("Deteccion de Ciclos: En grafos dirigidos emplea coloreado DFS (Blanco, Gris, Negro) detectando aristas hacia atras. En no dirigidos detecta aristas hacia nodos visitados distintos del padre.");

        // SECCIÓN 6: PREGUNTAS FRECUENTES PARA LA DEFENSA ORAL
        pdf.AddHeading1("6. Guia para la Exposicion: Preguntas de los Profesores");
        pdf.AddCallout("PREGUNTA 1: ¿Por que la gramatica de GraphLang es LL(1)?",
            "Respuesta: Porque cada decision de produccion se puede tomar analizando unicamente el siguiente token (lookahead de 1), " +
            "sin necesidad de backtracking y sin ambiguedades entre los conjuntos FIRST y FOLLOW.", false);

        pdf.AddCallout("PREGUNTA 2: ¿Como manejan el alcance de variables/nodos en bloques?",
            "Respuesta: Mediante una estructura jerarquica de Tablas de Simbolos. El compilador crea una tabla global para los nombres de grafos " +
            "y una tabla de simbolos local para cada bloque de grafo. Asi, los nodos viven en su propio scope y no colisionan entre si.", false);

        pdf.AddCallout("PREGUNTA 3: ¿Cual es la diferencia entre un error sintactico y un error semantico?",
            "Respuesta: El sintactico ocurre cuando la estructura viola la gramatica formal (ej: omitir un punto y coma). " +
            "El semantico ocurre cuando la estructura esta bien escrita pero no tiene sentido logico (ej: conectar un nodo que no fue declarado).", false);

        pdf.AddCallout("PREGUNTA 4: ¿Por que eligieron lista de adyacencia en lugar de matriz?",
            "Respuesta: Porque los grafos en aplicaciones practicas suelen ser dispersos. La lista de adyacencia ocupa espacio O(V + E) " +
            "y permite iterar directamente sobre los vecinos, a diferencia de la matriz que ocupa O(V^2) y es ineficiente en grafos con pocos enlaces.", false);

        pdf.Save(filePath);
    }

    private static void GenerateManualPdf(string filePath)
    {
        var pdf = new PdfWriter("Manual de Usuario: Lenguaje GraphLang");

        pdf.AddTitle(
            "Manual de Usuario y Referencia del Lenguaje GraphLang",
            "Guia de Sintaxis, Declaraciones, Comandos y Prevencion de Errores",
            "Compilador GraphLang v1.0 | Guia Universitaria"
        );

        pdf.AddHeading1("1. Como Ejecutar el Compilador");
        pdf.AddParagraph(
            "Para compilar y ejecutar cualquier archivo con extension .gl, abre una terminal en la carpeta raiz del proyecto y ejecuta:"
        );
        pdf.AddCodeBlock(new[] {
            "dotnet run -- Examples\\demo.gl",
            "dotnet run -- Examples\\valido_dirigido.gl",
            "dotnet run -- Examples\\valido_no_dirigido.gl"
        });

        pdf.AddHeading1("2. Sintaxis del Lenguaje GraphLang");
        pdf.AddParagraph("Todo programa en GraphLang se compone de declaraciones de grafos seguidas de instrucciones ejecutables.");

        pdf.AddHeading2("A. Declaracion de Grafo Dirigido");
        pdf.AddParagraph("En los grafos dirigidos, el sentido de cada conexion importa y las aristas DEBEN usar el operador '->':");
        pdf.AddCodeBlock(new[] {
            "GRAFO MiRed DIRIGIDO {",
            "    NODOS A, B, C, D;",
            "    ARISTA A -> B PESO 10;",
            "    ARISTA B -> C PESO 5;",
            "    ARISTA C -> D; // Peso 1 por defecto",
            "}"
        });

        pdf.AddHeading2("B. Declaracion de Grafo No Dirigido");
        pdf.AddParagraph("En grafos no dirigidos las conexiones son simetricas y las aristas DEBEN usar el operador '--':");
        pdf.AddCodeBlock(new[] {
            "GRAFO RedAmigos NO_DIRIGIDO {",
            "    NODOS Juan, Maria, Pedro;",
            "    ARISTA Juan -- Maria;",
            "    ARISTA Maria -- Pedro;",
            "}"
        });

        pdf.AddHeading1("3. Instrucciones Ejecutables");
        pdf.AddBullet("MOSTRAR <Grafo>; : Genera el archivo .dot, la imagen .png con Graphviz y el archivo .html interactivo.");
        pdf.AddBullet("RECORRER <Grafo> DESDE <Nodo> BFS; : Recorre el grafo en anchura partiendo del nodo especificado.");
        pdf.AddBullet("RECORRER <Grafo> DESDE <Nodo> DFS; : Recorre el grafo en profundidad partiendo del nodo especificado.");
        pdf.AddBullet("CAMINO <Grafo> DESDE <Origen> HASTA <Destino>; : Encuentra el camino de menor costo ponderado con Dijkstra.");
        pdf.AddBullet("CICLOS <Grafo>; : Analiza el grafo y reporta si contiene ciclos o si es aciclico.");

        pdf.AddHeading1("4. Que Cosas Hacer (Buenas Practicas)");
        pdf.AddBullet("Declarar SIEMPRE todos los nodos en la clausula NODOS antes de conectarlos en las aristas.");
        pdf.AddBullet("Finalizar toda declaracion e instruccion con punto y coma (;).");
        pdf.AddBullet("Utilizar identificadores descriptivos para grafos y nodos (ej: CiudadNorte, NodoCentral).");
        pdf.AddBullet("Aprovechar los comentarios de una linea (//) o multiples lineas (/* */) para documentar el modelo.");

        pdf.AddHeading1("5. Que Cosas NO Hacer (Errores Comunes)");
        pdf.AddCallout("ERROR 1: Conectar nodos no declarados",
            "Incorrecto: ARISTA A -> Z; cuando 'Z' no figura en la lista NODOS. El compilador detendra el analisis semantico.", true);
        pdf.AddCallout("ERROR 2: Mezclar operadores de arista",
            "Incorrecto: Usar '->' en un grafo NO_DIRIGIDO, o usar '--' en un grafo DIRIGIDO. Genera incompatibilidad semantica.", true);
        pdf.AddCallout("ERROR 3: Duplicar nombres en el mismo ambito",
            "Incorrecto: Declarar dos veces el mismo grafo, o declarar dos veces el mismo nodo dentro del mismo grafo.", true);

        pdf.Save(filePath);
    }

    private static void GeneratePresentacionPptx(string filePath)
    {
        var pptx = new PptxWriter();

        // Diapositiva 1: Portada
        pptx.AddSlide(new Slide
        {
            IsTitleSlide = true,
            Title = "Compilador GraphLang",
            Subtitle = "Diseno, Teoria Formal y Construccion con Coco/R y C#\nCatedra de Compiladores — 2026"
        });

        // Diapositiva 2: Motivación
        pptx.AddSlide(new Slide
        {
            Category = "Introduccion",
            Title = "¿Por que un DSL para Grafos?",
            Bullets = new List<string>
            {
                "Los grafos son estructuras de datos fundamentales en informatica: redes, ruteo, dependencias y redes sociales.",
                "Crear un lenguaje propio (DSL) permite a los usuarios definir grafos de manera concisa y expresiva.",
                "Demuestra en la practica todas las fases reales de la teoria de compiladores.",
                "Integra analisis lexico, sintaxis LL(1), validacion semantica y visualizacion automatica."
            },
            HighlightBox = "GraphLang no es un script generico: es un lenguaje orientado exclusivamente al modelado y calculo sobre grafos."
        });

        // Diapositiva 3: Arquitectura
        pptx.AddSlide(new Slide
        {
            Category = "Arquitectura",
            Title = "Pipeline de Compilacion",
            Bullets = new List<string>
            {
                "1. Codigo Fuente (.gl): Archivo de texto con las definiciones.",
                "2. Scanner (Coco/R): Descompone el texto en tokens (palabras clave, simbolos, numeros).",
                "3. Parser LL(1) (Coco/R): Aplica Descenso Recursivo para verificar la gramatica.",
                "4. Tabla de Simbolos y Semantica: Valida coherencia de grafos, nodos y operadores.",
                "5. Lista de Adyacencia: Modelo interno en memoria para calculos.",
                "6. Ejecucion y Visualizacion: Genera PNG con Graphviz e interfaces web interactivas."
            },
            HighlightBox = "Arquitectura modular en capas: Lexer -> Parser -> Semantica -> Modelo -> Algoritmos -> Visualizador."
        });

        // Diapositiva 4: Coco/R
        pptx.AddSlide(new Slide
        {
            Category = "Herramientas",
            Title = "Coco/R: El Generador de Compiladores",
            Bullets = new List<string>
            {
                "Herramienta academica estandar desarrollada en la Univ. Johannes Kepler (Linz).",
                "Utiliza una Gramatica Atribuida de Traduccion (.atg) en formato EBNF extendido.",
                "Genera analizadores sintacticos descendentes recursivos de tipo LL(1).",
                "Produce Scanner.cs y Parser.cs limpios y tipados en C# sin dependencias externas en tiempo de ejecucion.",
                "Permite incrustar acciones semanticas directamente entre parentesis y puntos: (. codigo C# .)."
            },
            HighlightBox = "LL(1) significa: de izquierda a derecha (L), derivacion por izquierda (L) y 1 token de anticipacion (lookahead)."
        });

        // Diapositiva 5: Análisis Léxico
        pptx.AddSlide(new Slide
        {
            Category = "Fase 1",
            Title = "Analisis Lexico (Scanner)",
            Bullets = new List<string>
            {
                "Rol: Leer el flujo de caracteres y agruparlos en unidades llamadas Tokens.",
                "Tokens reconocidos: GRAFO, DIRIGIDO, NODOS, ARISTA, PESO, MOSTRAR, RECORRER, CAMINO, CICLOS.",
                "Operadores y simbolos: '->', '--', '{', '}', ';', ','.",
                "Manejo de comentarios: Descarta automaticamente // linea y /* bloque */.",
                "Estructura del Token: Guarda tipo (kind), valor textual (val), linea y columna para reportar errores exactos."
            },
            HighlightBox = "El Scanner no valida reglas gramaticales; unicamente comprueba que los caracteres formen palabras validas."
        });

        // Diapositiva 6: Análisis Sintáctico
        pptx.AddSlide(new Slide
        {
            Category = "Fase 2",
            Title = "Analisis Sintactico (Parser)",
            Bullets = new List<string>
            {
                "Aplica el metodo de Descenso Recursivo: cada no-terminal de la gramatica es un metodo C#.",
                "Primitivas: Get() para pedir el proximo token, Expect(n) para exigir el token requerido.",
                "Sin ambiguedades: Cada regla tiene conjuntos FIRST disjuntos, permitiendo decidir con 1 solo token (lookahead).",
                "Recuperacion de errores: Detecta y reporta errores sintacticos indicando linea y simbolo faltante (ej: ';' expected)."
            },
            HighlightBox = "Si la sintaxis falla, la compilacion se detiene inmediatamente antes de cualquier ejecucion."
        });

        // Diapositiva 7: Tabla de Símbolos
        pptx.AddSlide(new Slide
        {
            Category = "Fase 3",
            Title = "La Tabla de Simbolos y Gestion de Ambitos",
            Bullets = new List<string>
            {
                "Estructura fundamental que almacena identificadores, tipos y alcances (scopes).",
                "Ambito Global: Guarda los nombres de grafos (GraphSymbol) y sus tipos. Impide grafos duplicados.",
                "Ambitos Locales: Cada bloque GRAFO tiene su propia tabla local de nodos (NodeSymbol).",
                "Aislamiento de nombres: Un nodo 'A' en Grafo1 no colisiona con un nodo 'A' en Grafo2.",
                "Resolucion: Antes de agregar una arista, se verifica que ambos nodos existan en el ambito del grafo."
            },
            HighlightBox = "Manejo riguroso de scopes: GlobalScope -> LocalScope por cada grafo declarado."
        });

        // Diapositiva 8: Análisis Semántico
        pptx.AddSlide(new Slide
        {
            Category = "Fase 3",
            Title = "Comprobaciones Semanticas",
            Bullets = new List<string>
            {
                "Validacion 1: Nodos duplicados en la declaracion NODOS.",
                "Validacion 2: Creacion de aristas con nodos no declarados.",
                "Validacion 3: Coherencia de operador ('->' solo en dirigidos, '--' solo en no dirigidos).",
                "Validacion 4: Pesos invalidos (deben ser enteros mayores a cero).",
                "Validacion 5: Referencias a grafos o nodos inexistentes en MOSTRAR o RECORRER."
            },
            HighlightBox = "Diferencia clave: la sintaxis dice COMO se escribe; la semantica verifica QUE TENGA SENTIDO logico."
        });

        // Diapositiva 9: Representación Interna
        pptx.AddSlide(new Slide
        {
            Category = "Fase 4",
            Title = "Representacion Interna: Lista de Adyacencia",
            Bullets = new List<string>
            {
                "Estructura elegida: Diccionario de nodos y listas de aristas salientes (Dictionary<string, List<Edge>>).",
                "Eficiencia espacial: O(V + E) en lugar de O(V^2) de una matriz de adyacencia.",
                "Eficiencia algoritmica: Consultar vecinos toma tiempo proporcional a su grado, no a todos los nodos.",
                "Grafos no dirigidos: Cada conexion A -- B se registra bidireccionalmente (A->B y B->A)."
            },
            HighlightBox = "La representacion interna esta completamente desacoplada de la sintaxis del lenguaje."
        });

        // Diapositiva 10: Algoritmos
        pptx.AddSlide(new Slide
        {
            Category = "Fase 5",
            Title = "Algoritmos Implementados",
            Bullets = new List<string>
            {
                "BFS (Anchura): Recorrido por niveles usando una cola. Ideal para menor distancia en aristas.",
                "DFS (Profundidad): Recorrido exhaustivo por ramas usando recursion y conjunto de visitados.",
                "Dijkstra (Camino Minimo): Ruta de menor costo considerando pesos de aristas con Cola de Prioridad.",
                "Deteccion de Ciclos: DFS con estados (Blanco, Gris, Negro) en dirigidos y control de padre en no dirigidos."
            },
            HighlightBox = "Todas las operaciones reportan resultados detallados paso a paso por consola."
        });

        // Diapositiva 11: Visualización
        pptx.AddSlide(new Slide
        {
            Category = "Visualizacion",
            Title = "Sistema de Visualizacion Dual",
            Bullets = new List<string>
            {
                "Formato DOT + Graphviz: Genera archivos .dot estandar y renderiza automaticamente imagenes PNG.",
                "Visor Web Interactivo: Genera archivos HTML con la biblioteca vis-network.",
                "Interactividad: Permite arrastrar nodos con el mouse, con fisicas dinamicas y zoom.",
                "Claridad visual: Nodos circulares, flechas de direccion y etiquetas claras de peso."
            },
            HighlightBox = "Solucion moderna sin depender de bibliotecas de escritorio obsoletas como Windows Forms."
        });

        // Diapositiva 12: Pruebas y Validación
        pptx.AddSlide(new Slide
        {
            Category = "Resultados",
            Title = "Casos de Prueba y Diagnosticos",
            Bullets = new List<string>
            {
                "demo.gl: Grafo dirigido con pesos, BFS, DFS, Dijkstra y deteccion de ciclos ejecutados con exito.",
                "valido_no_dirigido.gl: Red simetrica con operador '--' y recorrido en ambas direcciones.",
                "error_sintaxis.gl: El parser detecta omision de ';' indicando linea y columna.",
                "error_duplicados.gl: Semantica intercepta nodos y grafos repetidos.",
                "error_nodo_inexistente.gl: Semantica impide conectar nodos fuera de la tabla de simbolos."
            },
            HighlightBox = "100% de los casos de prueba ejecutados y verificados con codigo de salida acorde."
        });

        // Diapositiva 13: Preguntas de Examen
        pptx.AddSlide(new Slide
        {
            Category = "Defensa Oral",
            Title = "Preguntas Clave en la Exposicion",
            Bullets = new List<string>
            {
                "¿Por que Coco/R? Porque automatiza scanner y parser LL(1) manteniendo control total en C#.",
                "¿Como se aislan los nombres? Cada grafo tiene su propia instancia de SymbolTable para sus nodos.",
                "¿Que pasa si se conecta un nodo inexistente? El analizador semantico emite error y aborta la ejecucion.",
                "¿Que ventaja tiene BFS sobre DFS? BFS garantiza encontrar el camino con menor numero de saltos."
            },
            HighlightBox = "Dominio conceptual: saber justificar por que se tomo cada decision de arquitectura."
        });

        // Diapositiva 14: Conclusiones
        pptx.AddSlide(new Slide
        {
            Category = "Cierre",
            Title = "Conclusiones y Trabajo Futuro",
            Bullets = new List<string>
            {
                "Se construyo un compilador completo, robusto y extensible en .NET 8 y C#.",
                "Se demostro la aplicacion practica de todas las etapas de la teoria de compiladores.",
                "El uso de Coco/R simplifico la definicion formal mediante gramaticas atribuidas LL(1).",
                "Extensiones posibles: Arbol Generador Minimo (Kruskal/Prim), soporte de atributos en nodos y variables."
            },
            HighlightBox = "¡Proyecto finalizado, probado y listo para su presentacion universitaria!"
        });

        pptx.Save(filePath);
    }
}

