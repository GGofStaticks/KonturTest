using System.Globalization;

namespace KonturTest.Helpers;

public static class AmountParser
{
    public static bool TryParse(string? value, out decimal result)
    {
        result = 0;
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        var normalized = value.Trim().Replace(',', '.');
        return decimal.TryParse(
            normalized,
            NumberStyles.Number,
            CultureInfo.InvariantCulture,
            out result);
    }

    public static decimal Parse(string value)
    {
        if (!TryParse(value, out var result))
        {
            throw new FormatException($"Некорректное значение суммы: '{value}'.");
        }

        return result;
    }

    public static string Format(decimal value) =>
        value.ToString(CultureInfo.InvariantCulture);
}
