using System.Xml;

namespace KonturTest.Services;

public interface IXsltTransformService
{
    string Transform(string sourceXmlPath, string xsltPath);

    string Transform(XmlNode sourceXml, string xsltPath);
}
