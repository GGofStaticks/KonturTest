using System.IO;
using System.Xml;
using KonturTest.Helpers;
using KonturTest.Models;

namespace KonturTest.Services;

public sealed class PayrollService : IPayrollService
{
    private readonly IResourcePathProvider _paths;
    private readonly IData1Repository _data1Repository;
    private readonly IXsltTransformService _xsltTransformService;
    private readonly IEmployeeDocumentService _employeeDocumentService;

    public PayrollService(
        IResourcePathProvider paths,
        IData1Repository data1Repository,
        IXsltTransformService xsltTransformService,
        IEmployeeDocumentService employeeDocumentService)
    {
        _paths = paths;
        _data1Repository = data1Repository;
        _xsltTransformService = xsltTransformService;
        _employeeDocumentService = employeeDocumentService;
    }

    public PayrollResult Process(string dataFileName)
    {
        _paths.EnsureRequiredFiles(dataFileName);
        return ExecutePipeline(dataFileName);
    }

    public PayrollResult AddItemAndProcess(string name, string surname, string amount, string mount)
    {
        if (!AmountParser.TryParse(amount, out _))
        {
            throw new FormatException("Сумма должна быть числом (например 1000 или 3001,10).");
        }

        _paths.EnsureRequiredFiles(ResourceFileNames.Data1);

        var data1Path = _paths.Data1Path;
        var backupPath = data1Path + ".bak";
        File.Copy(data1Path, backupPath, overwrite: true);

        try
        {
            var data1Document = _data1Repository.AddItem(data1Path, name, surname, amount, mount);
            return ExecutePipeline(ResourceFileNames.Data1, data1Document);
        }
        catch
        {
            File.Copy(backupPath, data1Path, overwrite: true);
            throw;
        }
        finally
        {
            if (File.Exists(backupPath))
            {
                File.Delete(backupPath);
            }
        }
    }

    private PayrollResult ExecutePipeline(string dataFileName, XmlDocument? data1Document = null)
    {
        var xsltPath = _paths.XsltPath;
        XmlDocument transformedDocument;

        if (string.Equals(dataFileName, ResourceFileNames.Data1, StringComparison.OrdinalIgnoreCase))
        {
            var preparedData1 = data1Document is not null
                ? _data1Repository.UpdatePayTotal(data1Document, _paths.Data1Path)
                : _data1Repository.UpdatePayTotal(_paths.Data1Path);

            transformedDocument = _xsltTransformService.TransformToDocument(preparedData1, xsltPath);
        }
        else
        {
            var dataPath = _paths.ResolveReadablePath(dataFileName);
            transformedDocument = _xsltTransformService.TransformToDocument(dataPath, xsltPath);
        }

        return _employeeDocumentService.BuildAndSave(transformedDocument, _paths.EmployeesPath);
    }
}
