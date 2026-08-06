using System.Xml;
using KonturTest.Models;

namespace KonturTest.Services;

public interface IEmployeeDocumentService
{
    PayrollResult BuildAndSave(XmlDocument document, string employeesPath);
}
