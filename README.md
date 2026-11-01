# 🌐 Compilador GraphLang

Bienvenido a **GraphLang**, un compilador desarrollado para la materia de **Compiladores**. 
GraphLang es un lenguaje propio diseñado específicamente para **crear, manipular, analizar y visualizar grafos** (tanto dirigidos como no dirigidos) de forma simple y expresiva.

---

## 🛠️ Tecnologías y Herramientas Utilizadas

* **C# (.NET 8)**: Lenguaje principal utilizado para la lógica del compilador, estructuras de datos y algoritmos.
* **Coco/R para C#**: Generador de analizadores que crea el **Scanner** (léxico) y **Parser** (sintáctico descendente recursivo LL(1)) a partir del archivo de gramática `GraphLang.atg`.
* **Graphviz**: Herramienta utilizada para generar las imágenes gráficas en formato `.png` a partir de archivos `.dot`.
* **Vis-network (HTML/JS)**: Visor web interactivo donde puedes mover y arrastrar los nodos con el mouse.

---

## 🚀 Cómo Ejecutar el Compilador

Solo necesitas tener instalado [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0).

Abre una terminal en la carpeta del proyecto y corre:

```bash
# Ejecutar el demo principal completo:
dotnet run -- Examples\demo.gl

# Ejecutar un grafo no dirigido:
dotnet run -- Examples\valido_no_dirigido.gl

# Probar la detección de errores (sintácticos y semánticos):
dotnet run -- Examples\error_sintaxis.gl
dotnet run -- Examples\error_nodo_inexistente.gl
dotnet run -- Examples\error_duplicados.gl
dotnet run -- Examples\error_operador_incompatible.gl
```

> **Nota:** Si modificas la gramática en `Grammar/GraphLang.atg`, puedes regenerar el Scanner y Parser con:
> ```bash
> .\coco\Coco.exe Grammar\GraphLang.atg -namespace GraphLang.Generated -frames coco -o Generated
> dotnet build
> ```

---

## 📝 Guía Rápida del Lenguaje GraphLang

### 1. Grafo Dirigido (usa `->` y pesos opcionales)
```text
GRAFO RedTransporte DIRIGIDO {
    NODOS Central, Norte, Sur, Este;

    ARISTA Central -> Norte PESO 10;
    ARISTA Central -> Sur PESO 5;
    ARISTA Norte -> Este PESO 3;
    ARISTA Sur -> Este; // Si no pones PESO, por defecto es 1
}
```

### 2. Grafo No Dirigido (usa `--`)
```text
GRAFO RedAmigos NO_DIRIGIDO {
    NODOS Juan, Maria, Pedro;

    ARISTA Juan -- Maria;
    ARISTA Maria -- Pedro;
}
```

### 3. Instrucciones y Operaciones
* `MOSTRAR RedTransporte;` $\rightarrow$ Genera la imagen `.png` y abre el visor web `.html`.
* `RECORRER RedTransporte DESDE Central BFS;` $\rightarrow$ Recorrido en anchura por niveles.
* `RECORRER RedTransporte DESDE Central DFS;` $\rightarrow$ Recorrido en profundidad.
* `CAMINO RedTransporte DESDE Central HASTA Este;` $\rightarrow$ Camino de menor costo ponderado (**Dijkstra**).
* `CICLOS RedTransporte;` $\rightarrow$ Detecta si el grafo contiene ciclos o es acíclico.

---

## 🧠 Conceptos Clave de Compiladores Aplicados

1. **Análisis Léxico (Scanner)**: Lee carácter por carácter y agrupa el texto en *Tokens* (palabras reservadas, identificadores, números, operadores). Ignora espacios y comentarios (`//` y `/* */`).
2. **Análisis Sintáctico (Parser LL(1))**: Aplica *Descenso Recursivo*. Verifica que la estructura cumpla las reglas de la gramática formal sin ambigüedades.
3. **Tabla de Símbolos y Ámbitos (Scopes)**: 
   * **Ámbito Global**: Administra los nombres de los grafos para evitar grafos duplicados.
   * **Ámbito Local por Grafo**: Cada bloque de grafo tiene su propia tabla de símbolos para sus nodos. Esto evita que dos nodos tengan el mismo nombre dentro de un grafo, pero permite que dos grafos distintos tengan un nodo llamado igual sin interferir.
4. **Análisis Semántico**: Valida que no se usen nodos inexistentes en las aristas, que no se mezcle `->` en grafos no dirigidos ni `--` en dirigidos, y que los pesos sean positivos.
5. **Representación Interna**: Modela los grafos en memoria mediante una **Lista de Adyacencia** eficiente $O(V + E)$.

---

## 📂 Estructura del Proyecto

```text
GraphLang/
├── coco/                       # Ejecutable Coco.exe y plantillas frame
├── Grammar/
│   └── GraphLang.atg           # Gramática formal del lenguaje (EBNF con atributos)
├── Generated/                  # Archivos generados automáticamente por Coco/R
│   ├── Scanner.cs              # Analizador léxico
│   └── Parser.cs               # Analizador sintáctico
├── Models/                     # Clases internas: Node, Edge, Graph (Lista de Adyacencia)
├── Semantics/                  # Tabla de Símbolos (SymbolTable) y GraphManager
├── Operations/                 # Algoritmos: BFS, DFS, Dijkstra y Ciclos
├── Visualization/              # Exportador Graphviz (PNG) y visor web (HTML)
├── Examples/                   # Archivos de prueba (.gl)
├── Informe_Compilador_GraphLang.pdf  # Informe académico para la entrega
├── Manual_Usuario_GraphLang.pdf      # Manual de usuario del lenguaje
├── Presentacion_GraphLang.pptx       # Presentación de 14 diapositivas para exponer
├── Program.cs                  # Punto de entrada de la aplicación
└── README.md                   # Este archivo
```

---

## 👥 Integrantes y Materia
* **Materia**: Compiladores
* **Año**: 2026

