using Microsoft.Playwright;
using tmg_hero.Model;

namespace tmg_hero.Engine;

internal class BuildingManager
{
    private readonly IPage _page;

    public BuildingManager(IPage page)
    {
        _page = page;
    }

    public async Task<List<Building>> GetAllBuildingsAsync()
    {
        var buildings = new List<Building>();

        // Locate the elements containing the resource information
        var root = _page.Locator("#root div");
        var resourceElement = root.Filter(new() { HasText = "Living Quarters" }).Nth(2);
        var buttons = resourceElement.GetByRole(AriaRole.Button);
        //var testText = await resourceElement.AllInnerTextsAsync(); can be used later if we want sections.

        foreach (var button in await buttons.AllAsync())
        {
            // Hover over the button and wait for the tooltip to appear
            await button.HoverAsync();
            // Wait for the Tippy.js tooltip to appear
            try
            {
                await _page.WaitForSelectorAsync("[data-tippy-root]", new() { Timeout = 1000 });
            }
            catch (TimeoutException)
            {
                // Handle the case when the tooltip doesn't appear within the given timeout
            }

            // Get the Tippy.js tooltip text
            var tooltipElement = await _page.QuerySelectorAsync("[data-tippy-root]");
            if (tooltipElement == null)
            {
                continue;
            }
            var tooltipText = await tooltipElement!.InnerTextAsync();

            if (!string.IsNullOrEmpty(tooltipText))
            {
                // Parse the building data from the tooltip text
                var building = Building.TryParseBuildingFromTooltipText(tooltipText, button);
                if (building != null)
                {
                    buildings.Add(building);
                }
            }
        }

        return buildings;
    }

    public async Task<Building?> GetBuildingFromName(string buildingName)
    {
        // Locate the elements containing the resource information
        var root = _page.Locator("#root div");
        var resourceElement = root.Filter(new() { HasText = "Living Quarters" }).Nth(2);
        var button = resourceElement.GetByRole(AriaRole.Button).GetByText(buildingName);

        try
        {
            // Hover over the button and wait for the tooltip to appear
            await button.HoverAsync();
            // Wait for the Tippy.js tooltip to appear
            await _page.WaitForSelectorAsync("[data-tippy-root]", new() { Timeout = 1000 });

            var tooltipElement = await _page.QuerySelectorAsync("[data-tippy-root]");
            if (tooltipElement == null)
            {
                return null;
            }
            var tooltipText = await tooltipElement!.InnerTextAsync();

            if (!string.IsNullOrEmpty(tooltipText))
            {
                // Parse the building data from the tooltip text
                var building = Building.TryParseBuildingFromTooltipText(tooltipText, button);
                if (building != null)
                {
                    return building;
                }
            }

        }
        catch (Exception e)
        {
            Console.WriteLine("Exception: " + e.ToString() + "\n When trying to find Building with Name: "+buildingName);
        }
        return null;
    }
}
