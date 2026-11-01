using System.Globalization;
using System.Text;

namespace GraphLang.Docs;

public class PdfWriter
{
    private readonly List<PdfPage> _pages = new();
    private PdfPage _currentPage;
    private readonly float _pageWidth = 595.28f;  // A4
    private readonly float _pageHeight = 841.89f; // A4
    private readonly float _marginLeft = 50f;
    private readonly float _marginRight = 50f;
    private readonly float _marginTop = 50f;
    private readonly float _marginBottom = 50f;
    private float _currentY;
    private readonly string _documentHeader;

    public PdfWriter(string documentHeader = "")
    {
        _documentHeader = documentHeader;
        _currentPage = NewPage();
    }

    private PdfPage NewPage()
    {
        var page = new PdfPage(_pages.Count + 1);
        _pages.Add(page);
        _currentY = _pageHeight - _marginTop;
        return page;
    }

    public void AddPageBreak()
    {
        _currentPage = NewPage();
    }

    private void EnsureSpace(float neededHeight)
    {
        if (_currentY - neededHeight < _marginBottom)
        {
            _currentPage = NewPage();
        }
    }

    public void AddTitle(string title, string subtitle = "", string metadata = "")
    {
        EnsureSpace(120);

        // Barra decorativa superior
        _currentPage.AddRectangle(_marginLeft, _currentY - 4, _pageWidth - _marginLeft - _marginRight, 4, 0.14f, 0.38f, 0.92f); // Azul #2563EB
        _currentY -= 20;

        // Título principal
        var titleLines = WrapText(title, 22, _pageWidth - _marginLeft - _marginRight);
        foreach (var line in titleLines)
        {
            _currentPage.AddText(_marginLeft, _currentY, line, "F2", 22, 0.08f, 0.12f, 0.20f); // #141f33
            _currentY -= 26;
        }

        if (!string.IsNullOrEmpty(subtitle))
        {
            _currentY -= 4;
            var subLines = WrapText(subtitle, 13, _pageWidth - _marginLeft - _marginRight);
            foreach (var line in subLines)
            {
                _currentPage.AddText(_marginLeft, _currentY, line, "F1", 13, 0.25f, 0.35f, 0.48f);
                _currentY -= 17;
            }
        }

        if (!string.IsNullOrEmpty(metadata))
        {
            _currentY -= 4;
            _currentPage.AddText(_marginLeft, _currentY, metadata, "F3", 10, 0.40f, 0.45f, 0.55f);
            _currentY -= 15;
        }

        _currentY -= 15;
    }

    public void AddHeading1(string text)
    {
        EnsureSpace(45);
        _currentY -= 10;
        // Fondo suave para título 1
        float width = _pageWidth - _marginLeft - _marginRight;
        _currentPage.AddRectangle(_marginLeft, _currentY - 4, width, 22, 0.92f, 0.95f, 0.99f);
        _currentPage.AddRectangle(_marginLeft, _currentY - 4, 4, 22, 0.14f, 0.38f, 0.92f);
        _currentPage.AddText(_marginLeft + 12, _currentY, text, "F2", 14, 0.08f, 0.15f, 0.35f);
        _currentY -= 26;
    }

    public void AddHeading2(string text)
    {
        EnsureSpace(35);
        _currentY -= 6;
        _currentPage.AddText(_marginLeft, _currentY, text, "F2", 12, 0.12f, 0.22f, 0.45f);
        _currentY -= 16;
    }

    public void AddParagraph(string text)
    {
        float maxWidth = _pageWidth - _marginLeft - _marginRight;
        var lines = WrapText(text, 10, maxWidth);
        EnsureSpace(lines.Count * 14 + 6);

        foreach (var line in lines)
        {
            _currentPage.AddText(_marginLeft, _currentY, line, "F1", 10, 0.15f, 0.18f, 0.22f);
            _currentY -= 14;
        }
        _currentY -= 5;
    }

    public void AddBullet(string text, int level = 0)
    {
        float indent = _marginLeft + (level * 15);
        float maxWidth = _pageWidth - indent - _marginRight - 12;
        var lines = WrapText(text, 10, maxWidth);
        EnsureSpace(lines.Count * 14 + 4);

        // Bullet dot
        _currentPage.AddText(indent, _currentY, "•", "F2", 10, 0.14f, 0.38f, 0.92f);

        for (int i = 0; i < lines.Count; i++)
        {
            _currentPage.AddText(indent + 12, _currentY, lines[i], "F1", 10, 0.15f, 0.18f, 0.22f);
            _currentY -= 14;
        }
        _currentY -= 3;
    }

    public void AddCallout(string title, string text, bool isWarning = false)
    {
        float maxWidth = _pageWidth - _marginLeft - _marginRight - 24;
        var lines = WrapText(text, 9.5f, maxWidth);
        float boxHeight = 24 + (lines.Count * 13) + 8;
        EnsureSpace(boxHeight + 10);

        float boxWidth = _pageWidth - _marginLeft - _marginRight;
        float top = _currentY;

        // Fondo
        if (isWarning)
        {
            _currentPage.AddRectangle(_marginLeft, top - boxHeight, boxWidth, boxHeight, 0.99f, 0.95f, 0.92f); // Ámbar suave
            _currentPage.AddRectangle(_marginLeft, top - boxHeight, 4, boxHeight, 0.92f, 0.45f, 0.10f); // Borde naranja
            _currentPage.AddText(_marginLeft + 12, top - 14, title, "F2", 10.5f, 0.70f, 0.30f, 0.05f);
        }
        else
        {
            _currentPage.AddRectangle(_marginLeft, top - boxHeight, boxWidth, boxHeight, 0.93f, 0.97f, 1.0f); // Azul suave
            _currentPage.AddRectangle(_marginLeft, top - boxHeight, 4, boxHeight, 0.14f, 0.38f, 0.92f); // Borde azul
            _currentPage.AddText(_marginLeft + 12, top - 14, title, "F2", 10.5f, 0.08f, 0.25f, 0.65f);
        }

        float textY = top - 28;
        foreach (var line in lines)
        {
            _currentPage.AddText(_marginLeft + 12, textY, line, "F1", 9.5f, 0.15f, 0.18f, 0.22f);
            textY -= 13;
        }

        _currentY -= (boxHeight + 10);
    }

    public void AddCodeBlock(string[] codeLines)
    {
        float lineHeight = 12f;
        float boxHeight = (codeLines.Length * lineHeight) + 16;
        EnsureSpace(boxHeight + 10);

        float boxWidth = _pageWidth - _marginLeft - _marginRight;
        float top = _currentY;

        // Fondo oscuro elegante para código
        _currentPage.AddRectangle(_marginLeft, top - boxHeight, boxWidth, boxHeight, 0.96f, 0.97f, 0.98f);
        _currentPage.AddRectangle(_marginLeft, top - boxHeight, 3, boxHeight, 0.25f, 0.30f, 0.40f);

        float textY = top - 14;
        foreach (var line in codeLines)
        {
            _currentPage.AddText(_marginLeft + 10, textY, line, "F4", 9f, 0.10f, 0.15f, 0.25f);
            textY -= lineHeight;
        }

        _currentY -= (boxHeight + 10);
    }

    private List<string> WrapText(string text, float fontSize, float maxWidth)
    {
        var result = new List<string>();
        if (string.IsNullOrWhiteSpace(text)) return result;

        float approxCharWidth = fontSize * 0.52f;
        int maxCharsPerLine = Math.Max(10, (int)(maxWidth / approxCharWidth));

        var words = text.Split(' ');
        var sb = new StringBuilder();

        foreach (var word in words)
        {
            if (sb.Length == 0)
            {
                sb.Append(word);
            }
            else if (sb.Length + 1 + word.Length <= maxCharsPerLine)
            {
                sb.Append(" ").Append(word);
            }
            else
            {
                result.Add(sb.ToString());
                sb.Clear();
                sb.Append(word);
            }
        }

        if (sb.Length > 0)
            result.Add(sb.ToString());

        return result;
    }

    public void Save(string filePath)
    {
        // Agregar pie de página y encabezados en todas las páginas
        int totalPages = _pages.Count;
        for (int i = 0; i < totalPages; i++)
        {
            var p = _pages[i];
            // Encabezado
            if (!string.IsNullOrEmpty(_documentHeader) && i > 0)
            {
                p.AddText(_marginLeft, _pageHeight - 35, _documentHeader, "F3", 8.5f, 0.5f, 0.5f, 0.5f);
                p.AddRectangle(_marginLeft, _pageHeight - 38, _pageWidth - _marginLeft - _marginRight, 0.5f, 0.85f, 0.85f, 0.85f);
            }

            // Pie de página
            string pageText = $"Página {i + 1} de {totalPages}";
            p.AddText(_pageWidth - _marginRight - 70, 30, pageText, "F1", 8.5f, 0.5f, 0.5f, 0.5f);
            p.AddText(_marginLeft, 30, "Compilador GraphLang - Catedra de Compiladores", "F1", 8.5f, 0.5f, 0.5f, 0.5f);
            p.AddRectangle(_marginLeft, 42, _pageWidth - _marginLeft - _marginRight, 0.5f, 0.85f, 0.85f, 0.85f);
        }

        using var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);
        using var writer = new StreamWriter(fs, Encoding.GetEncoding("ISO-8859-1"));

        var offsets = new List<long>();
        long currentOffset = 0;

        void WriteString(string str)
        {
            byte[] bytes = Encoding.GetEncoding("ISO-8859-1").GetBytes(str);
            fs.Write(bytes, 0, bytes.Length);
            currentOffset += bytes.Length;
        }

        WriteString("%PDF-1.4\n%\xE2\xE3\xCF\xD3\n");

        // Objeto 1: Catalog
        offsets.Add(currentOffset);
        WriteString("1 0 obj\n<< /Type /Catalog /Pages 2 0 R >>\nendobj\n");

        // Objeto 2: Pages Root
        offsets.Add(currentOffset);
        var kids = new StringBuilder();
        for (int i = 0; i < _pages.Count; i++)
            kids.Append($"{3 + i * 2} 0 R ");
        WriteString($"2 0 obj\n<< /Type /Pages /Kids [{kids.ToString().Trim()}] /Count {_pages.Count} >>\nendobj\n");

        // Objetos de Página y Streams
        for (int i = 0; i < _pages.Count; i++)
        {
            int pageObjNum = 3 + i * 2;
            int streamObjNum = pageObjNum + 1;

            // Page Object
            offsets.Add(currentOffset);
            WriteString($"{pageObjNum} 0 obj\n<< /Type /Page /Parent 2 0 R /MediaBox [0 0 {_pageWidth.ToString(CultureInfo.InvariantCulture)} {_pageHeight.ToString(CultureInfo.InvariantCulture)}] /Contents {streamObjNum} 0 R /Resources << /Font << /F1 {3 + _pages.Count * 2} 0 R /F2 {4 + _pages.Count * 2} 0 R /F3 {5 + _pages.Count * 2} 0 R /F4 {6 + _pages.Count * 2} 0 R >> >> >>\nendobj\n");

            // Stream Object
            string content = _pages[i].GetContent();
            byte[] contentBytes = Encoding.GetEncoding("ISO-8859-1").GetBytes(content);
            offsets.Add(currentOffset);
            WriteString($"{streamObjNum} 0 obj\n<< /Length {contentBytes.Length} >>\nstream\n");
            fs.Write(contentBytes, 0, contentBytes.Length);
            currentOffset += contentBytes.Length;
            WriteString("\nendstream\nendobj\n");
        }

        // Fuentes estándar (F1=Helvetica, F2=Helvetica-Bold, F3=Helvetica-Oblique, F4=Courier)
        int fontBase = 3 + _pages.Count * 2;

        offsets.Add(currentOffset);
        WriteString($"{fontBase} 0 obj\n<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica /Encoding /WinAnsiEncoding >>\nendobj\n");

        offsets.Add(currentOffset);
        WriteString($"{fontBase + 1} 0 obj\n<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica-Bold /Encoding /WinAnsiEncoding >>\nendobj\n");

        offsets.Add(currentOffset);
        WriteString($"{fontBase + 2} 0 obj\n<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica-Oblique /Encoding /WinAnsiEncoding >>\nendobj\n");

        offsets.Add(currentOffset);
        WriteString($"{fontBase + 3} 0 obj\n<< /Type /Font /Subtype /Type1 /BaseFont /Courier /Encoding /WinAnsiEncoding >>\nendobj\n");

        // Tabla Xref
        long xrefOffset = currentOffset;
        int totalObjects = fontBase + 4;
        WriteString($"xref\n0 {totalObjects}\n0000000000 65535 f \n");
        foreach (var off in offsets)
        {
            WriteString($"{off:D10} 00000 n \n");
        }

        // Trailer
        WriteString($"trailer\n<< /Size {totalObjects} /Root 1 0 R >>\nstartxref\n{xrefOffset}\n%%EOF\n");
    }

    private class PdfPage
    {
        public int PageNumber { get; }
        private readonly StringBuilder _stream = new();

        public PdfPage(int pageNumber)
        {
            PageNumber = pageNumber;
        }

        public void AddText(float x, float y, string text, string font, float size, float r, float g, float b)
        {
            string cleanText = EscapePdfString(text);
            _stream.Append("BT\n");
            _stream.Append(string.Format(CultureInfo.InvariantCulture, "{0:0.##} {1:0.##} {2:0.##} rg\n", r, g, b));
            _stream.Append(string.Format(CultureInfo.InvariantCulture, "/{0} {1:0.##} Tf\n", font, size));
            _stream.Append(string.Format(CultureInfo.InvariantCulture, "{0:0.##} {1:0.##} Td\n", x, y));
            _stream.Append($"({cleanText}) Tj\n");
            _stream.Append("ET\n");
        }

        public void AddRectangle(float x, float y, float w, float h, float r, float g, float b)
        {
            _stream.Append(string.Format(CultureInfo.InvariantCulture, "{0:0.##} {1:0.##} {2:0.##} rg\n", r, g, b));
            _stream.Append(string.Format(CultureInfo.InvariantCulture, "{0:0.##} {1:0.##} {2:0.##} {3:0.##} re\n", x, y, w, h));
            _stream.Append("f\n");
        }

        public string GetContent() => _stream.ToString();

        private string EscapePdfString(string text)
        {
            var sb = new StringBuilder();
            foreach (char c in text)
            {
                if (c == '(' || c == ')' || c == '\\')
                {
                    sb.Append('\\').Append(c);
                }
                else if (c >= 32 && c <= 126)
                {
                    sb.Append(c);
                }
                else
                {
                    // Convertir acentos y caracteres especiales a ISO-8859-1
                    byte[] b = Encoding.GetEncoding("ISO-8859-1").GetBytes(new[] { c });
                    if (b.Length > 0 && b[0] >= 32)
                    {
                        sb.Append((char)b[0]);
                    }
                    else
                    {
                        sb.Append('?');
                    }
                }
            }
            return sb.ToString();
        }
    }
}

