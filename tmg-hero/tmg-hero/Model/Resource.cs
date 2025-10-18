using System.Globalization;

namespace tmg_hero.Model;

public sealed class Resource
{
    private Resource(string name, int amount, int cap, double income)
    {
        Name = name;
        Amount = amount;
        Cap = cap;
        Income = income;
    }

    public string Name { get; }
    public int Amount { get; set; }
    public int Cap { get; }
    public double Income { get; }

    public bool IsCapped => Amount >= Cap;

    public static bool TryParseResourceFromText(string resourceLine, out Resource? resource)
    {
        resource = null;
        var parts = resourceLine.Split('\t');

        if (parts.Length != 3)
        {
            return false;
        }

        string name = parts[0];

        if (!TryParseAmountAndCap(parts[1], out int amount, out int cap))
        {
            return false;
        }

        if (!double.TryParse(parts[2].Split('/')[0], NumberStyles.AllowThousands | NumberStyles.Float, CultureInfo.InvariantCulture, out double income))
        {
            return false;
        }

        resource = new Resource(name, amount, cap, income);
        return true;
    }

    private static bool TryParseAmountAndCap(string amountAndCap, out int amount, out int cap)
    {
        amount = 0;
        cap = 0;
        var parts = amountAndCap.Split('/');
        if (parts.Length != 2)
        {
            return false;
        }

        return TryParseValueWithMultiplier(parts[0].Trim(), out amount) && TryParseValueWithMultiplier(parts[1].Trim(), out cap);
    }

    private static bool TryParseValueWithMultiplier(string valueString, out int value)
    {
        value = 0;
        double multiplier = 1;
        if (valueString.EndsWith("K", StringComparison.OrdinalIgnoreCase))
        {
            multiplier = 1000;
            valueString = valueString.Substring(0, valueString.Length - 1);
        }
        else if (valueString.EndsWith("M", StringComparison.OrdinalIgnoreCase))
        {
            multiplier = 1000000;
            valueString = valueString.Substring(0, valueString.Length - 1);
        }

        if (!double.TryParse(valueString, NumberStyles.AllowThousands | NumberStyles.Float, CultureInfo.InvariantCulture, out double parsedValue))
        {
            return false;
        }

        value = (int)(parsedValue * multiplier);
        return true;
    }
}
