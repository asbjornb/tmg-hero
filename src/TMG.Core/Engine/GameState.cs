using Microsoft.Playwright;
using TMG.Core.Model;

namespace TMG.Core.Engine;

public class GameState
{
    private readonly IPage _page;
    public List<Building> Buildings { get; } = new();
    public Dictionary<string, Resource> Resources { get; } = new();

    public GameState(IPage page)
    {
        _page = page;
    }

    public async Task Initialize()
    {
        var buildingManager = new BuildingManager(_page);
        Buildings.AddRange(await buildingManager.GetAllBuildingsAsync());

        var resourceManager = new ResourceManager(_page);
        var dic = await resourceManager.GetResourceDataAsync();
        foreach (var item in dic)
        {
            Resources[item.Key] = item.Value;
        }
    }
}
