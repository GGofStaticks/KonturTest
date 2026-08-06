using System.Xml;

namespace KonturTest.Services;

public interface IData1Repository
{
    XmlDocument UpdatePayTotal(string data1Path);

    void AddItem(string data1Path, string name, string surname, string amount, string mount);
}
