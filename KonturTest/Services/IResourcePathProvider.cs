namespace KonturTest.Services;

public interface IResourcePathProvider
{
    string ResourcesDirectory { get; }

    string Data1Path { get; }

    string XsltPath { get; }

    string EmployeesPath { get; }

    string ResolveReadablePath(string fileName);

    IReadOnlyList<string> ListDataFiles();

    void EnsureRequiredFiles(string dataFileName);
}
