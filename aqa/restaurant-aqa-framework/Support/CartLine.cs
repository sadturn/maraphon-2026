using System.Globalization;

namespace Restaurant_Aqa_Framework.Support;

/// <summary>One line of a cart: a dish (by the name the scenario gave it) and how many.</summary>
public sealed record CartLine(string Dish, int Quantity)
{
    /// <summary>Reads the way a feature file writes a cart: "2 Borscht, 1 Varenyky"; an empty cell is an empty cart.</summary>
    public static IReadOnlyList<CartLine> ParseAll(string text) =>
        text.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(Parse)
            .ToList();

    private static CartLine Parse(string line)
    {
        var parts = line.Split(' ', 2, StringSplitOptions.TrimEntries);
        if (parts.Length != 2 || !int.TryParse(parts[0], NumberStyles.None, CultureInfo.InvariantCulture, out var quantity))
            throw new FormatException($"A cart line reads '<quantity> <dish>', e.g. '2 Borscht', not '{line}'.");

        return new CartLine(parts[1], quantity);
    }

    public override string ToString() => $"{Quantity} {Dish}";
}
