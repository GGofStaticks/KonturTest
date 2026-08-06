using System.IO;
using System.Text;
using System.Xml;
using KonturTest.Helpers;
using KonturTest.Models;

namespace KonturTest.Services;

public sealed class EmployeeDocumentService : IEmployeeDocumentService
{
    public PayrollResult BuildAndSave(string transformedXml, string employeesPath)
    {
        var document = new XmlDocument { PreserveWhitespace = false };
        document.LoadXml(transformedXml);

        foreach (XmlElement employee in document.SelectNodes("//Employee")!)
        {
            var total = employee.SelectNodes("salary")!
                .Cast<XmlElement>()
                .Sum(salary => AmountParser.Parse(salary.GetAttribute("amount")));

            employee.SetAttribute("total", AmountParser.Format(total));
        }

        var employees = document.SelectNodes("//Employee")!
            .Cast<XmlElement>()
            .Select(employee => new EmployeeSummary
            {
                Name = employee.GetAttribute("name"),
                Surname = employee.GetAttribute("surname"),
                Total = AmountParser.Parse(employee.GetAttribute("total"))
            })
            .ToList();

        var monthlyTotals = document.SelectNodes("//salary")!
            .Cast<XmlElement>()
            .GroupBy(salary => salary.GetAttribute("mount"))
            .Select(group => new MonthlyTotal
            {
                Month = group.Key,
                Total = group.Sum(salary => AmountParser.Parse(salary.GetAttribute("amount")))
            })
            .OrderBy(item => MonthNames.GetOrderIndex(item.Month))
            .ThenBy(item => item.Month, StringComparer.OrdinalIgnoreCase)
            .ToList();

        var xmlBytes = Serialize(document);
        File.WriteAllBytes(employeesPath, xmlBytes);

        return new PayrollResult
        {
            Employees = employees,
            MonthlyTotals = monthlyTotals,
            EmployeesXml = Encoding.UTF8.GetString(xmlBytes)
        };
    }

    private static byte[] Serialize(XmlDocument document)
    {
        using var stream = new MemoryStream();
        var settings = new XmlWriterSettings
        {
            Encoding = new UTF8Encoding(false),
            Indent = true,
            OmitXmlDeclaration = false,
            CloseOutput = false
        };

        using (var xmlWriter = XmlWriter.Create(stream, settings))
        {
            document.Save(xmlWriter);
        }

        return stream.ToArray();
    }
}
