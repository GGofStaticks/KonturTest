using System.Xml;
using KonturTest.Services;

namespace KonturTest.Tests;

public class EmployeeDocumentServiceTests
{
    [Fact]
    public void BuildAndSave_calculates_employee_and_monthly_totals()
    {
        using var temp = new TempDirectory();
        var employeesPath = temp.GetFilePath("Employees.xml");
        var document = new XmlDocument();
        document.LoadXml(TestXml.Employees);

        var service = new EmployeeDocumentService();
        var result = service.BuildAndSave(document, employeesPath);

        Assert.Equal(2, result.Employees.Count);
        Assert.Equal(300m, result.Employees.Single(employee => employee.Name == "Lena").Total);
        Assert.Equal(600m, result.Employees.Single(employee => employee.Name == "Masha").Total);

        Assert.Equal(400m, result.MonthlyTotals.Single(item => item.Month == "january").Total);
        Assert.Equal(300m, result.MonthlyTotals.Single(item => item.Month == "february").Total);
        Assert.Equal(200m, result.MonthlyTotals.Single(item => item.Month == "march").Total);
        Assert.Equal(["january", "february", "march"], result.MonthlyTotals.Select(item => item.Month).ToArray());

        var savedDocument = new XmlDocument();
        savedDocument.Load(employeesPath);
        Assert.Equal("300", savedDocument.SelectSingleNode("//Employee[@name='Lena']")!.Attributes!["total"]!.Value);
        Assert.False(string.IsNullOrWhiteSpace(result.EmployeesXml));
    }
}
