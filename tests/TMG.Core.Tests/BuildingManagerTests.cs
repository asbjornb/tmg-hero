using TMG.Core.Engine;

namespace TMG.Core.Tests;

public class BuildingManagerTests
{
    [Test]
    [Apartment(ApartmentState.STA)]
    public async Task ShouldGetSingleBuildingByName()
    {
        var saveGameManager = new SaveGameManager();
        var page = await saveGameManager.OpenGameAsync();
        var saveData = await File.ReadAllTextAsync("TestSaves/CanBuildFirstMarket.txt");
        await SaveGameManager.LoadSaveGame(saveData, page);

        var result = await saveGameManager.IsGameLoaded();
        result.Should().BeTrue();
        
        var manager = new BuildingManager(page);
        var market = await manager.GetBuildingFromName("Marketplace");

        market.Should().NotBeNull();
        market!.Name.Should().Be("Marketplace");
        market.Cost.Should().Contain(new KeyValuePair<string, int>("Gold", 1200));
        market.Cost.Should().Contain(new KeyValuePair<string, int>("Wood", 600));
        market.Cost.Should().Contain(new KeyValuePair<string, int>("Copper", 400));
        market.Cost.Should().Contain(new KeyValuePair<string, int>("Iron", 400));
        market.Cost.Should().Contain(new KeyValuePair<string, int>("Tools", 400));
    }
}
