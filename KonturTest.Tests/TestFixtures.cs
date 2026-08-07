namespace KonturTest.Tests;

internal static class TestPaths
{
    public static string XsltPath =>
        Path.Combine(AppContext.BaseDirectory, "Resources", "PayToEmployees.xslt");
}

internal static class TestXml
{
    public const string Data1Pay = """
        <?xml version="1.0" encoding="utf-8"?>
        <Pay>
            <item name="Lena" surname="Ivanova" amount="100" mount="january"/>
            <item name="Lena" surname="Ivanova" amount="200" mount="february"/>
            <item name="Masha" surname="Ivanova" amount="300" mount="january"/>
        </Pay>
        """;

    public const string Data2Pay = """
        <?xml version="1.0" encoding="utf-8"?>
        <Pay>
            <january>
                <item name="Masha" surname="Ivanova" amount="2000" mount="january"/>
            </january>
            <february>
                <item name="Lena" surname="Ivanova" amount="3001,10" mount="january"/>
            </february>
        </Pay>
        """;

    public const string Employees = """
        <?xml version="1.0" encoding="utf-8"?>
        <Employees>
            <Employee name="Lena" surname="Ivanova">
                <salary amount="100" mount="january"/>
                <salary amount="200" mount="march"/>
            </Employee>
            <Employee name="Masha" surname="Ivanova">
                <salary amount="300" mount="january"/>
                <salary amount="300" mount="february"/>
            </Employee>
        </Employees>
        """;
}

internal sealed class TempDirectory : IDisposable
{
    public TempDirectory()
    {
        DirectoryPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(DirectoryPath);
    }

    public string DirectoryPath { get; }

    public string GetFilePath(string fileName) => Path.Combine(DirectoryPath, fileName);

    public void Dispose()
    {
        if (Directory.Exists(DirectoryPath))
        {
            Directory.Delete(DirectoryPath, recursive: true);
        }
    }
}
