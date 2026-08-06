namespace KonturTest.Models;

public sealed class MonthlyTotal
{
    public required string Month { get; init; }

    public required decimal Total { get; init; }
}
