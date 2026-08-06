using System.Xml;

namespace KonturTest.Services;

public interface IXsltTransformService
{
    XmlDocument TransformToDocument(string sourceXmlPath, string xsltPath);

    XmlDocument TransformToDocument(XmlNode sourceXml, string xsltPath);
}
