using System.IO;
using System.Text;
using System.Xml;
using KonturTest.Helpers;
using KonturTest.Models;

namespace KonturTest.Services;

public sealed class EmployeeDocumentService : IEmployeeDocumentService
{
    public PayrollResult BuildAndSave(XmlDocument document, string employeesPath)
    {
        var employees = new List<EmployeeSummary>();
        var monthlyTotalsAccumulator = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);

        foreach (XmlElement employee in document.SelectNodes("//Employee")!)
        {
            var employeeTotal = 0m;

            foreach (XmlElement salary in employee.SelectNodes("salary")!)
            {
                var amount = AmountParser.Parse(salary.GetAttribute("amount"));
                employeeTotal += amount;

                var month = salary.GetAttribute("mount");
                monthlyTotalsAccumulator.TryGetValue(month, out var monthTotal);
                monthlyTotalsAccumulator[month] = monthTotal + amount;
            }

            employee.SetAttribute("total", AmountParser.Format(employeeTotal));
            employees.Add(new EmployeeSummary
            {
                Name = employee.GetAttribute("name"),
                Surname = employee.GetAttribute("surname"),
                Total = employeeTotal
            });
        }

        var monthlyTotals = monthlyTotalsAccumulator
            .Select(pair => new MonthlyTotal
            {
                Month = pair.Key,
                Total = pair.Value
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
