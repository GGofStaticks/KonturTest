using System.Xml;
using KonturTest.Services;

namespace KonturTest.Tests;

public class XsltTransformServiceTests
{
    [Fact]
    public void TransformToDocument_groups_data1_items_by_employee()
    {
        using var temp = new TempDirectory();
        var data1Path = temp.GetFilePath("Data1.xml");
        File.WriteAllText(data1Path, TestXml.Data1Pay);

        var service = new XsltTransformService();
        var document = service.TransformToDocument(data1Path, TestPaths.XsltPath);

        var lena = document.SelectSingleNode("//Employee[@name='Lena' and @surname='Ivanova']");
        var masha = document.SelectSingleNode("//Employee[@name='Masha' and @surname='Ivanova']");

        Assert.NotNull(lena);
        Assert.NotNull(masha);
        Assert.Equal(2, lena.SelectNodes("salary")!.Count);
        Assert.Equal("january", lena.SelectSingleNode("salary[@amount='100']")!.Attributes!["mount"]!.Value);
        Assert.Equal("february", lena.SelectSingleNode("salary[@amount='200']")!.Attributes!["mount"]!.Value);
    }

    [Fact]
    public void TransformToDocument_uses_parent_name_for_data2_month()
    {
        using var temp = new TempDirectory();
        var data2Path = temp.GetFilePath("Data2.xml");
        File.WriteAllText(data2Path, TestXml.Data2Pay);

        var service = new XsltTransformService();
        var document = service.TransformToDocument(data2Path, TestPaths.XsltPath);

        var februarySalary = document.SelectSingleNode("//Employee[@name='Lena']/salary")!;

        Assert.Equal("february", februarySalary.Attributes!["mount"]!.Value);
    }
}
