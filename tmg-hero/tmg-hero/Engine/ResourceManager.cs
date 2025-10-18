using Microsoft.Playwright;
using tmg_hero.Model;

namespace tmg_hero.Engine;

public class ResourceManager
{
    private readonly IPage _page;

    public ResourceManager(IPage page)
    {
        _page = page;
    }

    public async Task<Dictionary<string, Resource>> GetResourceDataAsync()
    {
        var resourceData = new Dictionary<string, Resource>();

        // Locate the elements containing the resource information
        var root = _page.Locator("#root div");
        var resourceElement = root.Filter(new() { HasText = "Research" }).Nth(3);

        // Get the inner text of the resource element
        string text = await resourceElement.InnerTextAsync();

        foreach (var line in text.Split("\n"))
        {
            if (Resource.TryParseResourceFromText(line, out var resource))
            {
                resourceData[resource!.Name] = resource;
            }
            else
            {
                Console.WriteLine($"Failed to parse resource from text: {line}");
            }
        }

        return resourceData;
    }
}
