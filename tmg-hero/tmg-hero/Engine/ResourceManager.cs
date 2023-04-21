using Microsoft.Playwright;
using System.Globalization;
using System.Text.RegularExpressions;

namespace tmg_hero.Engine;

internal class ResourceManager
{
    private readonly IPage _page;

    public ResourceManager(IPage page)
    {
        _page = page;
    }

    public async Task<Dictionary<string, (int current, int max, double income)>> GetResourceDataAsync()
    {
        var resourceData = new Dictionary<string, (int current, int max, double income)>();

        // Locate the elements containing the resource information
        var root = _page.Locator("#root div");
        var resourceElement = root.Filter(new() { HasText = "Research" }).Nth(3);

        // Get the inner text of the resource element
        string text = await resourceElement.InnerTextAsync();

        // Use regular expressions to extract the resource data
        var matches = Regex.Matches(text, @"(\w+)(?=\s*\d+)\s*(\d+(?:,\d+)?)(?:(?:\s*/\s*)?(\d+(?:,\d+)?))?\s*(\d+(?:\.\d+)?)/s");

        foreach (Match match in matches)
        {
            if (match.Success && match.Groups.Count == 5)
            {
                string name = match.Groups[1].Value;
                int current = int.Parse(match.Groups[2].Value.Replace(",", ""));
                int max = int.Parse(match.Groups[3].Value.Replace(",", ""));
                double income = double.Parse(match.Groups[4].Value, CultureInfo.InvariantCulture);

                resourceData[name] = (current, max, income);
            }
        }

        return resourceData;
    }

}
