using System.Globalization;

namespace KonturTest.Helpers;

public static class MonthNames
{
    private static readonly CultureInfo Culture = CultureInfo.InvariantCulture;

    public static IReadOnlyList<string> All { get; } = Enumerable.Range(1, 12)
        .Select(month => Culture.DateTimeFormat.GetMonthName(month).ToLowerInvariant())
        .ToArray();

    public static int GetOrderIndex(string month)
    {
        if (DateTime.TryParseExact(
                month,
                "MMMM",
                Culture,
                DateTimeStyles.AllowWhiteSpaces,
                out var date))
        {
            return date.Month - 1;
        }

        return All.Count;
    }
}
