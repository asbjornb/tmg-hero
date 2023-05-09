using System.Globalization;

namespace tmg_hero.Model;

internal sealed class Resource
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

        string amountString = parts[0].Trim();
        double amountMultiplier = 1;
        if (amountString.EndsWith("K", StringComparison.OrdinalIgnoreCase))
        {
            amountMultiplier = 1000;
            amountString = amountString.Substring(0, amountString.Length - 1);
        }
        else if (amountString.EndsWith("M", StringComparison.OrdinalIgnoreCase))
        {
            amountMultiplier = 1000000;
            amountString = amountString.Substring(0, amountString.Length - 1);
        }

        if (!double.TryParse(amountString, NumberStyles.AllowThousands | NumberStyles.Float, CultureInfo.InvariantCulture, out double amountValue))
        {
            return false;
        }

        amount = (int)(amountValue * amountMultiplier);

        string capString = parts[1].Trim();
        double capMultiplier = 1;
        if (capString.EndsWith("K", StringComparison.OrdinalIgnoreCase))
        {
            capMultiplier = 1000;
            capString = capString.Substring(0, capString.Length - 1);
        }
        else if (capString.EndsWith("M", StringComparison.OrdinalIgnoreCase))
        {
            capMultiplier = 1000000;
            capString = capString.Substring(0, capString.Length - 1);
        }

        if (!double.TryParse(capString, NumberStyles.AllowThousands | NumberStyles.Float, CultureInfo.InvariantCulture, out double capValue))
        {
            return false;
        }

        cap = (int)(capValue * capMultiplier);
        return true;
    }
}
