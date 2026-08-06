using System.Xml;
using KonturTest.Helpers;

namespace KonturTest.Services;

public sealed class Data1Repository : IData1Repository
{
    public XmlDocument UpdatePayTotal(string data1Path)
    {
        var document = LoadDocument(data1Path);
        var total = document.SelectNodes("//item")!
            .Cast<XmlElement>()
            .Sum(item => AmountParser.Parse(item.GetAttribute("amount")));

        var payElement = document.DocumentElement
            ?? throw new InvalidOperationException("В файле нет корневого элемента Pay.");

        payElement.SetAttribute("total", AmountParser.Format(total));
        document.Save(data1Path);
        return document;
    }

    public void AddItem(string data1Path, string name, string surname, string amount, string mount)
    {
        var document = LoadDocument(data1Path);
        var payElement = document.DocumentElement
            ?? throw new InvalidOperationException("В файле нет корневого элемента Pay.");

        var item = document.CreateElement("item");
        item.SetAttribute("name", name);
        item.SetAttribute("surname", surname);
        item.SetAttribute("amount", amount);
        item.SetAttribute("mount", mount);
        payElement.AppendChild(item);

        document.Save(data1Path);
    }

    private static XmlDocument LoadDocument(string data1Path)
    {
        var document = new XmlDocument { PreserveWhitespace = false };
        document.Load(data1Path);
        return document;
    }
}
