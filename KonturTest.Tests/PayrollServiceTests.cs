using System.Xml;
using KonturTest.Services;

namespace KonturTest.Tests;

public class PayrollServiceTests
{
    [Fact]
    public void AddItemAndProcess_restores_data1_when_pipeline_fails()
    {
        using var temp = new TempDirectory();
        var resourcesDirectory = temp.GetFilePath("Resources");
        Directory.CreateDirectory(resourcesDirectory);

        var data1Path = Path.Combine(resourcesDirectory, ResourceFileNames.Data1);
        File.WriteAllText(data1Path, TestXml.Data1Pay);
        File.Copy(TestPaths.XsltPath, Path.Combine(resourcesDirectory, ResourceFileNames.Xslt));

        var paths = new TestResourcePathProvider(resourcesDirectory);
        var payrollService = new PayrollService(
            paths,
            new Data1Repository(),
            new XsltTransformService(),
            new FailingEmployeeDocumentService());

        Assert.Throws<InvalidOperationException>(() =>
            payrollService.AddItemAndProcess("Oleg", "Petrov", "50", "march"));

        var document = new XmlDocument();
        document.Load(data1Path);

        Assert.Equal(3, document.SelectNodes("//item")!.Count);
        Assert.False(document.SelectNodes("//item[@name='Oleg']")!.Count > 0);
        Assert.Equal(string.Empty, document.DocumentElement!.GetAttribute("total"));
    }

    private sealed class TestResourcePathProvider(string resourcesDirectory) : IResourcePathProvider
    {
        public string ResourcesDirectory { get; } = resourcesDirectory;

        public string Data1Path => Path.Combine(ResourcesDirectory, ResourceFileNames.Data1);

        public string XsltPath => Path.Combine(ResourcesDirectory, ResourceFileNames.Xslt);

        public string EmployeesPath => Path.Combine(ResourcesDirectory, ResourceFileNames.Employees);

        public string ResolveReadablePath(string fileName) => Path.Combine(ResourcesDirectory, fileName);

        public IReadOnlyList<string> ListDataFiles() => [ResourceFileNames.Data1];

        public void EnsureRequiredFiles(string dataFileName)
        {
            Directory.CreateDirectory(ResourcesDirectory);
        }
    }

    private sealed class FailingEmployeeDocumentService : IEmployeeDocumentService
    {
        public Models.PayrollResult BuildAndSave(XmlDocument document, string employeesPath) =>
            throw new InvalidOperationException("pipeline failed");
    }
}
