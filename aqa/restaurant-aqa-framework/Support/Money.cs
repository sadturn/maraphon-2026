using System.Globalization;

namespace Restaurant_Aqa_Framework.Support;

public static class Money
{
    /// <summary>Reads an amount the way the screen's currency pipe shows it ("$1,234.50").</summary>
    public static decimal Parse(string shown)
    {
        var digits = new string(shown.Where(c => char.IsDigit(c) || c is '.' or '-').ToArray());
        return decimal.Parse(digits, NumberStyles.Number, CultureInfo.InvariantCulture);
    }

    public static string Format(decimal amount) => amount.ToString("0.00", CultureInfo.InvariantCulture);
}
