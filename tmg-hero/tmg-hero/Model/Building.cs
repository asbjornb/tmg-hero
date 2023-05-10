using Microsoft.Playwright;
using System.Globalization;
using System.Text.RegularExpressions;

namespace tmg_hero.Model;

//Currently simplistic building model. No supported roles, cap increases or bonuses modelled
internal sealed class Building
{
    private readonly ILocator? _button;

    public string Name { get; }
    public Dictionary<string, int> Cost { get; }
    public Dictionary<string, double> Production { get; }
    public int? Population { get; }

    private Building(string name, Dictionary<string, int> cost, Dictionary<string, double> production, int? population, ILocator? button)
    {
        Name = name;
        Cost = cost;
        Production = production;
        Population = population;
        _button = button;
    }

    public static Building? TryParseBuildingFromTooltipText(string tooltipText, ILocator? button)
    {
        try
        {
            // Split the tooltip text into lines
            var lines = tooltipText.Split('\n');

            // Get the building name from the first line
            var name = lines[0].Trim();

            // Parse the cost, production, and population values from the remaining lines
            const string resourcePattern = @"(\w+)\s*((?:\+|\-)?[\d,]+(?:\.\d+)?)(?:\/s)?";
            int? population = default;
            var cost = new Dictionary<string, int>();
            var production = new Dictionary<string, double>();

            bool isCost = true;

            foreach (var line in lines.Skip(1))
            {
                if (line.Contains("Population"))
                {
                    population = int.Parse(line.Split("\t").Last());
                    continue;
                }
                if (line.Contains("/s") || line.Contains("%") || line.Contains("+"))
                {
                    isCost = false;
                }

                var match = Regex.Match(line, resourcePattern);
                if (match.Success)
                {
                    string resourceName = match.Groups[1].Value;
                    double value = double.Parse(match.Groups[2].Value, NumberStyles.AllowThousands | NumberStyles.Float, CultureInfo.InvariantCulture);

                    if (isCost)
                    {
                        cost[resourceName] = (int)value;
                    }
                    else if (line.Contains("/s"))
                    {
                        production[resourceName] = value;
                    }
                }
            }

            return new Building(name, cost, production, population, button);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Failed to parse building with error {e}");
            return null;
        }
    }


    public bool IsHousing()
    {
        return Population.HasValue;
    }

    public List<(string resource, int amount)> NegativeIncomes()
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

    public async Task BuyAsync()
    {
        if(_button is not null)
        {
            try
            {
                await _button.ClickAsync();
                //Buildings and research takes 2 seconds to complete
                await Task.Delay(2500);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to click Button for building {Name} with error {e}");
                throw;
            }
        }
    }
}
