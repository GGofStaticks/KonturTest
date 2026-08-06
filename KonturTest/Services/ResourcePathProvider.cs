using System.IO;

namespace KonturTest.Services;

public sealed class ResourcePathProvider : IResourcePathProvider
{
    public ResourcePathProvider()
    {
        ResourcesDirectory = Path.Combine(AppContext.BaseDirectory, ResourceFileNames.Folder);
    }

    public string ResourcesDirectory { get; }

    public string Data1Path => Path.Combine(ResourcesDirectory, ResourceFileNames.Data1);

    public string XsltPath => ResolveReadablePath(ResourceFileNames.Xslt);

    public string EmployeesPath => Path.Combine(ResourcesDirectory, ResourceFileNames.Employees);

    public string ResolveReadablePath(string fileName)
    {
        var workingPath = Path.Combine(ResourcesDirectory, fileName);
        if (File.Exists(workingPath))
        {
            return workingPath;
        }

        var originalPath = FindOriginalFile(fileName);
        if (originalPath is null)
        {
            throw new FileNotFoundException($"Не найден файл {fileName}.", fileName);
        }

        return originalPath;
    }

    public IReadOnlyList<string> ListDataFiles()
    {
        var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var fileName in EnumerateXmlFileNames(ResourcesDirectory))
        {
            names.Add(fileName);
        }

        foreach (var originalDir in FindOriginalResourceDirectories())
        {
            foreach (var fileName in EnumerateXmlFileNames(originalDir))
            {
                names.Add(fileName);
            }
        }

        return names
            .OrderBy(name => name, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    public void EnsureRequiredFiles(string dataFileName)
    {
        Directory.CreateDirectory(ResourcesDirectory);

        _ = ResolveReadablePath(ResourceFileNames.Xslt);
        _ = ResolveReadablePath(dataFileName);

        if (string.Equals(dataFileName, ResourceFileNames.Data1, StringComparison.OrdinalIgnoreCase))
        {
            CopyData1ToBinIfMissing();
        }
    }

    private void CopyData1ToBinIfMissing()
    {
        if (File.Exists(Data1Path))
        {
            return;
        }

        var originalPath = FindOriginalFile(ResourceFileNames.Data1)
            ?? throw new FileNotFoundException($"Не найден файл {ResourceFileNames.Data1}.", ResourceFileNames.Data1);

        File.Copy(originalPath, Data1Path);
    }

    private static IEnumerable<string> EnumerateXmlFileNames(string directory)
    {
        if (!Directory.Exists(directory))
        {
            yield break;
        }

        foreach (var path in Directory.EnumerateFiles(directory, "*.xml", SearchOption.TopDirectoryOnly))
        {
            var fileName = Path.GetFileName(path);
            if (string.Equals(fileName, ResourceFileNames.Employees, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            yield return fileName;
        }
    }

    private static string? FindOriginalFile(string fileName)
    {
        foreach (var directory in FindOriginalResourceDirectories())
        {
            var candidate = Path.Combine(directory, fileName);
            if (File.Exists(candidate))
            {
                return candidate;
            }
        }

        return null;
    }

    private static IEnumerable<string> FindOriginalResourceDirectories()
    {
        for (var dir = new DirectoryInfo(AppContext.BaseDirectory).Parent; dir != null; dir = dir.Parent)
        {
            var candidate = Path.Combine(dir.FullName, ResourceFileNames.Folder);
            if (Directory.Exists(candidate))
            {
                yield return candidate;
            }
        }
    }
}
