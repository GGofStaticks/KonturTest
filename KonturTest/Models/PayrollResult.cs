namespace KonturTest.Models;

public sealed class PayrollResult
{
    public required IReadOnlyList<EmployeeSummary> Employees { get; init; }

    public required IReadOnlyList<MonthlyTotal> MonthlyTotals { get; init; }

    public required string EmployeesXml { get; init; }
}
