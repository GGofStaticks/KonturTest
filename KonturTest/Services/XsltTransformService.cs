using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Xsl;

namespace KonturTest.Services;

public sealed class XsltTransformService : IXsltTransformService
{
    private readonly object _sync = new();
    private string? _cachedXsltPath;
    private XslCompiledTransform? _cachedTransform;

    public XmlDocument TransformToDocument(string sourceXmlPath, string xsltPath)
    {
        using var stream = TransformToStream(sourceXmlPath, xsltPath);
        return LoadDocument(stream);
    }

    public XmlDocument TransformToDocument(XmlNode sourceXml, string xsltPath)
    {
        using var stream = TransformToStream(sourceXml, xsltPath);
        return LoadDocument(stream);
    }

    private MemoryStream TransformToStream(string sourceXmlPath, string xsltPath)
    {
        var transform = GetOrLoadTransform(xsltPath);
        var stream = new MemoryStream();
        using (var xmlWriter = CreateWriter(stream))
        {
            transform.Transform(sourceXmlPath, xmlWriter);
        }

        stream.Position = 0;
        return stream;
    }

    private MemoryStream TransformToStream(XmlNode sourceXml, string xsltPath)
    {
        var transform = GetOrLoadTransform(xsltPath);
        var stream = new MemoryStream();
        using (var xmlWriter = CreateWriter(stream))
        {
            transform.Transform(sourceXml, null, xmlWriter);
        }

        stream.Position = 0;
        return stream;
    }

    private static XmlDocument LoadDocument(MemoryStream stream)
    {
        var document = new XmlDocument { PreserveWhitespace = false };
        document.Load(stream);
        return document;
    }

    private XslCompiledTransform GetOrLoadTransform(string xsltPath)
    {
        lock (_sync)
        {
            if (_cachedTransform is not null
                && string.Equals(_cachedXsltPath, xsltPath, StringComparison.OrdinalIgnoreCase))
            {
                return _cachedTransform;
            }

            var transform = new XslCompiledTransform();
            transform.Load(xsltPath);
            _cachedTransform = transform;
            _cachedXsltPath = xsltPath;
            return transform;
        }
    }

    private static XmlWriter CreateWriter(Stream stream)
    {
        var settings = new XmlWriterSettings
        {
            Encoding = new UTF8Encoding(false),
            Indent = true,
            OmitXmlDeclaration = false,
            CloseOutput = false
        };

        return XmlWriter.Create(stream, settings);
    }
}
