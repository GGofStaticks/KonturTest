using KonturTest.Models;

namespace KonturTest.Services;

public interface IEmployeeDocumentService
{
    PayrollResult BuildAndSave(string transformedXml, string employeesPath);
}
