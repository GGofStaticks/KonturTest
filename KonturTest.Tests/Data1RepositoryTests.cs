using System.Xml;
using KonturTest.Services;

namespace KonturTest.Tests;

public class Data1RepositoryTests
{
    [Fact]
    public void UpdatePayTotal_writes_total_on_pay_element()
    {
        using var temp = new TempDirectory();
        var data1Path = temp.GetFilePath("Data1.xml");
        File.WriteAllText(data1Path, TestXml.Data1Pay);

        var repository = new Data1Repository();
        repository.UpdatePayTotal(data1Path);

        var document = new XmlDocument();
        document.Load(data1Path);

        Assert.Equal("600", document.DocumentElement!.GetAttribute("total"));
    }

    [Fact]
    public void UpdatePayTotal_with_document_does_not_reload_file()
    {
        using var temp = new TempDirectory();
        var data1Path = temp.GetFilePath("Data1.xml");
        File.WriteAllText(data1Path, TestXml.Data1Pay);

        var repository = new Data1Repository();
        var document = repository.AddItem(data1Path, "Oleg", "Petrov", "50", "march");
        repository.UpdatePayTotal(document, data1Path);

        var savedDocument = new XmlDocument();
        savedDocument.Load(data1Path);

        Assert.Equal("650", savedDocument.DocumentElement!.GetAttribute("total"));
        Assert.Equal(4, savedDocument.SelectNodes("//item")!.Count);
    }

    [Fact]
    public void AddItem_appends_item_with_attributes()
    {
        using var temp = new TempDirectory();
        var data1Path = temp.GetFilePath("Data1.xml");
        File.WriteAllText(data1Path, TestXml.Data1Pay);

        var repository = new Data1Repository();
        repository.AddItem(data1Path, "Oleg", "Petrov", "50", "march");

        var document = new XmlDocument();
        document.Load(data1Path);
        var addedItem = document.SelectSingleNode("//item[@name='Oleg' and @surname='Petrov']") as XmlElement;

        Assert.NotNull(addedItem);
        Assert.Equal("50", addedItem.GetAttribute("amount"));
        Assert.Equal("march", addedItem.GetAttribute("mount"));
    }
}
