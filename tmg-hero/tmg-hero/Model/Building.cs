using System.Globalization;
using System.Text.RegularExpressions;

namespace tmg_hero.Model;

//Currently simplistic building model. No supported roles, cap increases or bonuses modelled
public sealed class Building
{
    public string Name { get; }
    public Dictionary<string, int> Cost { get; }
    public Dictionary<string, double> Production { get; }
    public int? Population { get; }

    private Building(string name, Dictionary<string, int> cost, Dictionary<string, double> production, int? population)
    {
        Name = name;
        Cost = cost;
        Production = production;
        Population = population;
    }

    public static Building? TryParseBuildingFromTooltipText(string tooltipText)
    {
        try
        {
            // Split the tooltip text into lines
            var lines = tooltipText.Split('\n');

            // Get the building name from the first line
            var name = lines[0].Trim();

            // Parse the cost, production, and population values from the remaining lines
            var resourcePattern = @"(\w+)\s*([\+\-]?\d+(?:\.\d+)?)(?:\/s)?";
            int? population = default;
            var cost = new Dictionary<string, int>();
            var production = new Dictionary<string, double>();

            foreach (var line in lines.Skip(1))
            {
                var match = Regex.Match(line, resourcePattern);
                if (match.Success)
                {
                    string resourceName = match.Groups[1].Value;
                    double value = double.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture);

                    if (line.Contains("/s"))
                    {
                        production[resourceName] = value;
                    }
                    else if (resourceName == "Population")
                    {
                        population = (int)value;
                    }
                    else
                    {
                        cost[resourceName] = (int)value;
                    }
                }
            }

            return new Building(name, cost, production, population);
        }
        catch(Exception e)
        {
            Console.WriteLine($"Failed to parse building with error {e}");
            return null;
        }
    }

    public bool IsHousing()
    {
        return Population.HasValue;
    }

    public List<(string resource, int amount)> CostResources()
    {
        var costResources = new List<(string resource, int amount)>();

        foreach (var resource in Production)
        {
            if (resource.Value < 0)
            {
                costResources.Add((resource.Key, (int)Math.Abs(resource.Value)));
            }
        }

        return costResources;
    }
}
