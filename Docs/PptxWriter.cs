using System.IO.Compression;
using System.Text;

namespace GraphLang.Docs;

public class Slide
{
    public string Title { get; set; } = "";
    public string Category { get; set; } = "COMPILADORES | GRAPHLANG";
    public List<string> Bullets { get; set; } = new();
    public string HighlightBox { get; set; } = "";
    public bool IsTitleSlide { get; set; } = false;
    public string Subtitle { get; set; } = "";
}

public class PptxWriter
{
    private readonly List<Slide> _slides = new();

    public void AddSlide(Slide slide)
    {
        _slides.Add(slide);
    }

    public void Save(string filePath)
    {
        if (File.Exists(filePath))
            File.Delete(filePath);

        using var zip = ZipFile.Open(filePath, ZipArchiveMode.Create);

        // 1. [Content_Types].xml
        AddEntry(zip, "[Content_Types].xml", GenerateContentTypes());

        // 2. _rels/.rels
        AddEntry(zip, "_rels/.rels", GenerateRootRels());

        // 3. ppt/presentation.xml
        AddEntry(zip, "ppt/presentation.xml", GeneratePresentationXml());

        // 4. ppt/_rels/presentation.xml.rels
        AddEntry(zip, "ppt/_rels/presentation.xml.rels", GeneratePresentationRels());

        // 5. ppt/slideMasters/slideMaster1.xml
        AddEntry(zip, "ppt/slideMasters/slideMaster1.xml", GenerateSlideMaster());

        // 6. ppt/slideMasters/_rels/slideMaster1.xml.rels
        AddEntry(zip, "ppt/slideMasters/_rels/slideMaster1.xml.rels", GenerateSlideMasterRels());

        // 7. ppt/slideLayouts/slideLayout1.xml
        AddEntry(zip, "ppt/slideLayouts/slideLayout1.xml", GenerateSlideLayout());

        // 8. ppt/slideLayouts/_rels/slideLayout1.xml.rels
        AddEntry(zip, "ppt/slideLayouts/_rels/slideLayout1.xml.rels", GenerateSlideLayoutRels());

        // 9. ppt/theme/theme1.xml
        AddEntry(zip, "ppt/theme/theme1.xml", GenerateTheme());

        // 10. Slides y sus rels
        for (int i = 0; i < _slides.Count; i++)
        {
            int slideNum = i + 1;
            AddEntry(zip, $"ppt/slides/slide{slideNum}.xml", GenerateSlideXml(_slides[i], slideNum));
            AddEntry(zip, $"ppt/slides/_rels/slide{slideNum}.xml.rels", GenerateSlideRels());
        }
    }

    private void AddEntry(ZipArchive zip, string path, string content)
    {
        var entry = zip.CreateEntry(path, CompressionLevel.Optimal);
        using var stream = entry.Open();
        byte[] bytes = Encoding.UTF8.GetBytes(content);
        stream.Write(bytes, 0, bytes.Length);
    }

    private string GenerateContentTypes()
    {
        var sb = new StringBuilder();
        sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>");
        sb.AppendLine("<Types xmlns=\"http://schemas.openxmlformats.org/package/2006/content-types\">");
        sb.AppendLine("  <Default Extension=\"rels\" ContentType=\"application/vnd.openxmlformats-package.relationships+xml\"/>");
        sb.AppendLine("  <Default Extension=\"xml\" ContentType=\"application/xml\"/>");
        sb.AppendLine("  <Override PartName=\"/ppt/presentation.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.presentationml.presentation.main+xml\"/>");
        sb.AppendLine("  <Override PartName=\"/ppt/slideMasters/slideMaster1.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.presentationml.slideMaster+xml\"/>");
        sb.AppendLine("  <Override PartName=\"/ppt/slideLayouts/slideLayout1.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.presentationml.slideLayout+xml\"/>");
        sb.AppendLine("  <Override PartName=\"/ppt/theme/theme1.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.theme+xml\"/>");
        for (int i = 0; i < _slides.Count; i++)
        {
            sb.AppendLine($"  <Override PartName=\"/ppt/slides/slide{i + 1}.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.presentationml.slide+xml\"/>");
        }
        sb.AppendLine("</Types>");
        return sb.ToString();
    }

    private string GenerateRootRels()
    {
        return """
        <?xml version="1.0" encoding="UTF-8" standalone="yes"?>
        <Relationships xmlns="http://schemas.openxmlformats.org/package/2006/relationships">
          <Relationship Id="rId1" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument" Target="ppt/presentation.xml"/>
        </Relationships>
        """;
    }

    private string GeneratePresentationXml()
    {
        var sb = new StringBuilder();
        sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>");
        sb.AppendLine("<p:presentation xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\" xmlns:r=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships\" xmlns:p=\"http://schemas.openxmlformats.org/presentationml/2006/main\">");
        sb.AppendLine("  <p:sldMasterIdLst>");
        sb.AppendLine("    <p:sldMasterId id=\"2147483648\" r:id=\"rId1\"/>");
        sb.AppendLine("  </p:sldMasterIdLst>");
        sb.AppendLine("  <p:sldIdLst>");
        for (int i = 0; i < _slides.Count; i++)
        {
            sb.AppendLine($"    <p:sldId id=\"{256 + i}\" r:id=\"rId{i + 2}\"/>");
        }
        sb.AppendLine("  </p:sldIdLst>");
        sb.AppendLine("  <p:sldSz cx=\"9144000\" cy=\"5143500\" type=\"screen16x9\"/>");
        sb.AppendLine("  <p:notesSz cx=\"6858000\" cy=\"9144000\"/>");
        sb.AppendLine("</p:presentation>");
        return sb.ToString();
    }

    private string GeneratePresentationRels()
    {
        var sb = new StringBuilder();
        sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>");
        sb.AppendLine("<Relationships xmlns=\"http://schemas.openxmlformats.org/package/2006/relationships\">");
        sb.AppendLine("  <Relationship Id=\"rId1\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/slideMaster\" Target=\"slideMasters/slideMaster1.xml\"/>");
        for (int i = 0; i < _slides.Count; i++)
        {
            sb.AppendLine($"  <Relationship Id=\"rId{i + 2}\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/slide\" Target=\"slides/slide{i + 1}.xml\"/>");
        }
        sb.AppendLine("</Relationships>");
        return sb.ToString();
    }

    private string GenerateSlideMaster()
    {
        return """
        <?xml version="1.0" encoding="UTF-8" standalone="yes"?>
        <p:sldMaster xmlns:a="http://schemas.openxmlformats.org/drawingml/2006/main" xmlns:r="http://schemas.openxmlformats.org/officeDocument/2006/relationships" xmlns:p="http://schemas.openxmlformats.org/presentationml/2006/main">
          <p:cSld>
            <p:spTree>
              <p:nvGrpSpPr><p:cNvPr id="1" name=""/><p:cNvGrpSpPr/><p:nvPr/></p:nvGrpSpPr>
              <p:grpSpPr><a:xfrm><a:off x="0" y="0"/><a:ext cx="0" cy="0"/><a:chOff x="0" y="0"/><a:chExt cx="0" cy="0"/></a:xfrm></p:grpSpPr>
            </p:spTree>
          </p:cSld>
          <p:clrMap bg1="lt1" tx1="dk1" bg2="lt2" tx2="dk2" accent1="accent1" accent2="accent2" accent3="accent3" accent4="accent4" accent5="accent5" accent6="accent6" hlink="hlink" folHlink="folHlink"/>
          <p:sldLayoutIdLst>
            <p:sldLayoutId id="2147483649" r:id="rId1"/>
          </p:sldLayoutIdLst>
        </p:sldMaster>
        """;
    }

    private string GenerateSlideMasterRels()
    {
        return """
        <?xml version="1.0" encoding="UTF-8" standalone="yes"?>
        <Relationships xmlns="http://schemas.openxmlformats.org/package/2006/relationships">
          <Relationship Id="rId1" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/slideLayout" Target="../slideLayouts/slideLayout1.xml"/>
          <Relationship Id="rId2" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/theme" Target="../theme/theme1.xml"/>
        </Relationships>
        """;
    }

    private string GenerateSlideLayout()
    {
        return """
        <?xml version="1.0" encoding="UTF-8" standalone="yes"?>
        <p:sldLayout xmlns:a="http://schemas.openxmlformats.org/drawingml/2006/main" xmlns:r="http://schemas.openxmlformats.org/officeDocument/2006/relationships" xmlns:p="http://schemas.openxmlformats.org/presentationml/2006/main" type="blank">
          <p:cSld>
            <p:spTree>
              <p:nvGrpSpPr><p:cNvPr id="1" name=""/><p:cNvGrpSpPr/><p:nvPr/></p:nvGrpSpPr>
              <p:grpSpPr><a:xfrm><a:off x="0" y="0"/><a:ext cx="0" cy="0"/><a:chOff x="0" y="0"/><a:chExt cx="0" cy="0"/></a:xfrm></p:grpSpPr>
            </p:spTree>
          </p:cSld>
        </p:sldLayout>
        """;
    }

    private string GenerateSlideLayoutRels()
    {
        return """
        <?xml version="1.0" encoding="UTF-8" standalone="yes"?>
        <Relationships xmlns="http://schemas.openxmlformats.org/package/2006/relationships">
          <Relationship Id="rId1" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/slideMaster" Target="../slideMasters/slideMaster1.xml"/>
        </Relationships>
        """;
    }

    private string GenerateTheme()
    {
        return """
        <?xml version="1.0" encoding="UTF-8" standalone="yes"?>
        <a:theme xmlns:a="http://schemas.openxmlformats.org/drawingml/2006/main" name="Office Theme">
          <a:themeElements>
            <a:clrScheme name="ModernBlue">
              <a:dk1><a:srgbClr val="0F172A"/></a:dk1>
              <a:lt1><a:srgbClr val="FFFFFF"/></a:lt1>
              <a:dk2><a:srgbClr val="1E293B"/></a:dk2>
              <a:lt2><a:srgbClr val="F8FAFC"/></a:lt2>
              <a:accent1><a:srgbClr val="2563EB"/></a:accent1>
              <a:accent2><a:srgbClr val="06B6D4"/></a:accent2>
              <a:accent3><a:srgbClr val="10B981"/></a:accent3>
              <a:accent4><a:srgbClr val="F59E0B"/></a:accent4>
              <a:accent5><a:srgbClr val="6366F1"/></a:accent5>
              <a:accent6><a:srgbClr val="EC4899"/></a:accent6>
              <a:hlink><a:srgbClr val="2563EB"/></a:hlink>
              <a:folHlink><a:srgbClr val="9333EA"/></a:folHlink>
            </a:clrScheme>
            <a:fontScheme name="ModernFont">
              <a:majorFont><a:latin typeface="Segoe UI Semibold"/></a:majorFont>
              <a:minorFont><a:latin typeface="Segoe UI"/></a:minorFont>
            </a:fontScheme>
            <a:fmtScheme name="Office">
              <a:fillStyleLst><a:solidFill><a:schemeClr val="phClr"/></a:solidFill><a:solidFill><a:schemeClr val="phClr"/></a:solidFill><a:solidFill><a:schemeClr val="phClr"/></a:solidFill></a:fillStyleLst>
              <a:lnStyleLst><a:ln w="9525"><a:solidFill><a:schemeClr val="phClr"/></a:solidFill></a:ln><a:ln w="25400"><a:solidFill><a:schemeClr val="phClr"/></a:solidFill></a:ln><a:ln w="38100"><a:solidFill><a:schemeClr val="phClr"/></a:solidFill></a:ln></a:lnStyleLst>
              <a:effectStyleLst><a:effectLst/><a:effectLst/><a:effectLst/></a:effectStyleLst>
              <a:bgFillStyleLst><a:solidFill><a:schemeClr val="phClr"/></a:solidFill><a:solidFill><a:schemeClr val="phClr"/></a:solidFill><a:solidFill><a:schemeClr val="phClr"/></a:solidFill></a:bgFillStyleLst>
            </a:fmtScheme>
          </a:themeElements>
        </a:theme>
        """;
    }

    private string GenerateSlideRels()
    {
        return """
        <?xml version="1.0" encoding="UTF-8" standalone="yes"?>
        <Relationships xmlns="http://schemas.openxmlformats.org/package/2006/relationships">
          <Relationship Id="rId1" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/slideLayout" Target="../slideLayouts/slideLayout1.xml"/>
        </Relationships>
        """;
    }

    private string GenerateSlideXml(Slide slide, int slideNum)
    {
        var sb = new StringBuilder();
        sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>");
        sb.AppendLine("<p:sld xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\" xmlns:r=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships\" xmlns:p=\"http://schemas.openxmlformats.org/presentationml/2006/main\">");
        sb.AppendLine("  <p:cSld>");
        sb.AppendLine("    <p:spTree>");
        sb.AppendLine("      <p:nvGrpSpPr><p:cNvPr id=\"1\" name=\"\"/><p:cNvGrpSpPr/><p:nvPr/></p:nvGrpSpPr>");
        sb.AppendLine("      <p:grpSpPr><a:xfrm><a:off x=\"0\" y=\"0\"/><a:ext cx=\"0\" cy=\"0\"/><a:chOff x=\"0\" y=\"0\"/><a:chExt cx=\"0\" cy=\"0\"/></a:xfrm></p:grpSpPr>");

        int spId = 2;

        if (slide.IsTitleSlide)
        {
            // Fondo oscuro para portada (9144000 x 5143500 EMUs)
            sb.AppendLine($"""
                  <p:sp>
                    <p:nvSpPr><p:cNvPr id="{spId++}" name="Bg"/><p:cNvSpPr/><p:nvPr/></p:nvSpPr>
                    <p:spPr>
                      <a:xfrm><a:off x="0" y="0"/><a:ext cx="9144000" cy="5143500"/></a:xfrm>
                      <a:prstGeom prst="rect"><a:avLst/></a:prstGeom>
                      <a:solidFill><a:srgbClr val="0F172A"/></a:solidFill>
                    </p:spPr>
                  </p:sp>
            """);

            // Barra decorativa azul
            sb.AppendLine($"""
                  <p:sp>
                    <p:nvSpPr><p:cNvPr id="{spId++}" name="Bar"/><p:cNvSpPr/><p:nvPr/></p:nvSpPr>
                    <p:spPr>
                      <a:xfrm><a:off x="800000" y="1200000"/><a:ext cx="120000" cy="1800000"/></a:xfrm>
                      <a:prstGeom prst="rect"><a:avLst/></a:prstGeom>
                      <a:solidFill><a:srgbClr val="2563EB"/></a:solidFill>
                    </p:spPr>
                  </p:sp>
            """);

            // Título y subtítulo
            sb.AppendLine($"""
                  <p:sp>
                    <p:nvSpPr><p:cNvPr id="{spId++}" name="TitleBox"/><p:cNvSpPr><a:spLocks noGrp="1"/></p:cNvSpPr><p:nvPr/></p:nvSpPr>
                    <p:spPr>
                      <a:xfrm><a:off x="1100000" y="1100000"/><a:ext cx="7200000" cy="2000000"/></a:xfrm>
                    </p:spPr>
                    <p:txBody>
                      <a:bodyPr rtlCol="0" anchor="ctr"/>
                      <a:p>
                        <a:pPr algn="l"/>
                        <a:r>
                          <a:rPr lang="es-ES" sz="4000" b="1">
                            <a:solidFill><a:srgbClr val="FFFFFF"/></a:solidFill>
                            <a:latin typeface="Segoe UI Semibold"/>
                          </a:rPr>
                          <a:t>{EscapeXml(slide.Title)}</a:t>
                        </a:r>
                      </a:p>
                      <a:p>
                        <a:pPr algn="l"/>
                        <a:r>
                          <a:rPr lang="es-ES" sz="2200">
                            <a:solidFill><a:srgbClr val="94A3B8"/></a:solidFill>
                            <a:latin typeface="Segoe UI"/>
                          </a:rPr>
                          <a:t>{EscapeXml(slide.Subtitle)}</a:t>
                        </a:r>
                      </a:p>
                    </p:txBody>
                  </p:sp>
            """);
        }
        else
        {
            // Diapositiva estándar con fondo blanco y franja superior elegante
            // Fondo general blanco
            sb.AppendLine($"""
                  <p:sp>
                    <p:nvSpPr><p:cNvPr id="{spId++}" name="Bg"/><p:cNvSpPr/><p:nvPr/></p:nvSpPr>
                    <p:spPr>
                      <a:xfrm><a:off x="0" y="0"/><a:ext cx="9144000" cy="5143500"/></a:xfrm>
                      <a:prstGeom prst="rect"><a:avLst/></a:prstGeom>
                      <a:solidFill><a:srgbClr val="F8FAFC"/></a:solidFill>
                    </p:spPr>
                  </p:sp>
            """);

            // Barra decorativa superior
            sb.AppendLine($"""
                  <p:sp>
                    <p:nvSpPr><p:cNvPr id="{spId++}" name="TopBar"/><p:cNvSpPr/><p:nvPr/></p:nvSpPr>
                    <p:spPr>
                      <a:xfrm><a:off x="0" y="0"/><a:ext cx="9144000" cy="60000"/></a:xfrm>
                      <a:prstGeom prst="rect"><a:avLst/></a:prstGeom>
                      <a:solidFill><a:srgbClr val="2563EB"/></a:solidFill>
                    </p:spPr>
                  </p:sp>
            """);

            // Categoría / Módulo (Texto pequeño)
            sb.AppendLine($"""
                  <p:sp>
                    <p:nvSpPr><p:cNvPr id="{spId++}" name="Category"/><p:cNvSpPr/><p:nvPr/></p:nvSpPr>
                    <p:spPr>
                      <a:xfrm><a:off x="685800" y="320000"/><a:ext cx="7772400" cy="300000"/></a:xfrm>
                    </p:spPr>
                    <p:txBody>
                      <a:bodyPr rtlCol="0" anchor="t"/>
                      <a:p>
                        <a:r>
                          <a:rPr lang="es-ES" sz="1100" b="1">
                            <a:solidFill><a:srgbClr val="2563EB"/></a:solidFill>
                            <a:latin typeface="Segoe UI Semibold"/>
                          </a:rPr>
                          <a:t>{EscapeXml(slide.Category.ToUpperInvariant())}</a:t>
                        </a:r>
                      </a:p>
                    </p:txBody>
                  </p:sp>
            """);

            // Título de la diapositiva
            sb.AppendLine($"""
                  <p:sp>
                    <p:nvSpPr><p:cNvPr id="{spId++}" name="Title"/><p:cNvSpPr/><p:nvPr/></p:nvSpPr>
                    <p:spPr>
                      <a:xfrm><a:off x="685800" y="600000"/><a:ext cx="7772400" cy="600000"/></a:xfrm>
                    </p:spPr>
                    <p:txBody>
                      <a:bodyPr rtlCol="0" anchor="t"/>
                      <a:p>
                        <a:r>
                          <a:rPr lang="es-ES" sz="2600" b="1">
                            <a:solidFill><a:srgbClr val="0F172A"/></a:solidFill>
                            <a:latin typeface="Segoe UI Semibold"/>
                          </a:rPr>
                          <a:t>{EscapeXml(slide.Title)}</a:t>
                        </a:r>
                      </a:p>
                    </p:txBody>
                  </p:sp>
            """);

            // Contenido / Bullets
            var bulletsXml = new StringBuilder();
            foreach (var b in slide.Bullets)
            {
                bulletsXml.AppendLine($"""
                      <a:p>
                        <a:pPr marL="285750" indent="-285750">
                          <a:buClr><a:srgbClr val="2563EB"/></a:buClr>
                          <a:buChar char="•"/>
                        </a:pPr>
                        <a:r>
                          <a:rPr lang="es-ES" sz="1500">
                            <a:solidFill><a:srgbClr val="334155"/></a:solidFill>
                            <a:latin typeface="Segoe UI"/>
                          </a:rPr>
                          <a:t>{EscapeXml(b)}</a:t>
                        </a:r>
                      </a:p>
                """);
            }

            int contentWidth = string.IsNullOrEmpty(slide.HighlightBox) ? 7772400 : 4900000;

            sb.AppendLine($"""
                  <p:sp>
                    <p:nvSpPr><p:cNvPr id="{spId++}" name="Content"/><p:cNvSpPr/><p:nvPr/></p:nvSpPr>
                    <p:spPr>
                      <a:xfrm><a:off x="685800" y="1300000"/><a:ext cx="{contentWidth}" cy="3100000"/></a:xfrm>
                    </p:spPr>
                    <p:txBody>
                      <a:bodyPr rtlCol="0" anchor="t"/>
                      {bulletsXml}
                    </p:txBody>
                  </p:sp>
            """);

            // Recuadro destacado lateral (si existe)
            if (!string.IsNullOrEmpty(slide.HighlightBox))
            {
                sb.AppendLine($"""
                  <p:sp>
                    <p:nvSpPr><p:cNvPr id="{spId++}" name="HighlightBg"/><p:cNvSpPr/><p:nvPr/></p:nvSpPr>
                    <p:spPr>
                      <a:xfrm><a:off x="5800000" y="1300000"/><a:ext cx="2650000" cy="3000000"/></a:xfrm>
                      <a:prstGeom prst="roundRect"><a:avLst/></a:prstGeom>
                      <a:solidFill><a:srgbClr val="EFF6FF"/></a:solidFill>
                      <a:ln w="12700"><a:solidFill><a:srgbClr val="93C5FD"/></a:solidFill></a:ln>
                    </p:spPr>
                  </p:sp>
                  <p:sp>
                    <p:nvSpPr><p:cNvPr id="{spId++}" name="HighlightText"/><p:cNvSpPr/><p:nvPr/></p:nvSpPr>
                    <p:spPr>
                      <a:xfrm><a:off x="5950000" y="1450000"/><a:ext cx="2350000" cy="2700000"/></a:xfrm>
                    </p:spPr>
                    <p:txBody>
                      <a:bodyPr rtlCol="0" anchor="t"/>
                      <a:p>
                        <a:r>
                          <a:rPr lang="es-ES" sz="1300" b="1">
                            <a:solidFill><a:srgbClr val="1E40AF"/></a:solidFill>
                            <a:latin typeface="Segoe UI Semibold"/>
                          </a:rPr>
                          <a:t>CLAVE EXPOSICIÓN</a:t>
                        </a:r>
                      </a:p>
                      <a:p>
                        <a:r>
                          <a:rPr lang="es-ES" sz="1300">
                            <a:solidFill><a:srgbClr val="1E3A8A"/></a:solidFill>
                            <a:latin typeface="Segoe UI"/>
                          </a:rPr>
                          <a:t>{EscapeXml(slide.HighlightBox)}</a:t>
                        </a:r>
                      </a:p>
                    </p:txBody>
                  </p:sp>
                """);
            }

            // Pie de página con número de diapositiva
            sb.AppendLine($"""
                  <p:sp>
                    <p:nvSpPr><p:cNvPr id="{spId++}" name="Footer"/><p:cNvSpPr/><p:nvPr/></p:nvSpPr>
                    <p:spPr>
                      <a:xfrm><a:off x="685800" y="4650000"/><a:ext cx="7772400" cy="300000"/></a:xfrm>
                    </p:spPr>
                    <p:txBody>
                      <a:bodyPr rtlCol="0" anchor="b"/>
                      <a:p>
                        <a:r>
                          <a:rPr lang="es-ES" sz="1000">
                            <a:solidFill><a:srgbClr val="94A3B8"/></a:solidFill>
                            <a:latin typeface="Segoe UI"/>
                          </a:rPr>
                          <a:t>Compiladores | GraphLang — Diapositiva {slideNum} de {_slides.Count}</a:t>
                        </a:r>
                      </a:p>
                    </p:txBody>
                  </p:sp>
            """);
        }

        sb.AppendLine("    </p:spTree>");
        sb.AppendLine("  </p:cSld>");
        sb.AppendLine("</p:sld>");
        return sb.ToString();
    }

    private string EscapeXml(string text)
    {
        if (string.IsNullOrEmpty(text)) return "";
        return text.Replace("&", "&amp;")
                   .Replace("<", "&lt;")
                   .Replace(">", "&gt;")
                   .Replace("\"", "&quot;")
                   .Replace("'", "&apos;");
    }
}

