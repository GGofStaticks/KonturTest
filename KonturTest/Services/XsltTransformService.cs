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

    public string Transform(string sourceXmlPath, string xsltPath)
    {
        var transform = GetOrLoadTransform(xsltPath);
        using var stream = new MemoryStream();
        using (var xmlWriter = CreateWriter(stream))
        {
            transform.Transform(sourceXmlPath, xmlWriter);
        }

        return Encoding.UTF8.GetString(stream.ToArray());
    }

    public string Transform(XmlNode sourceXml, string xsltPath)
    {
        var transform = GetOrLoadTransform(xsltPath);
        using var stream = new MemoryStream();
        using (var xmlWriter = CreateWriter(stream))
        {
            transform.Transform(sourceXml, null, xmlWriter);
        }

        return Encoding.UTF8.GetString(stream.ToArray());
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
