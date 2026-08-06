using System.Xml;

namespace KonturTest.Services;

public interface IData1Repository
{
    XmlDocument UpdatePayTotal(string data1Path);

    XmlDocument UpdatePayTotal(XmlDocument document, string data1Path);

    XmlDocument AddItem(string data1Path, string name, string surname, string amount, string mount);
}
