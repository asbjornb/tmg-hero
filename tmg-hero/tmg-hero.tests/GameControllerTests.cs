using tmg_hero.Engine;

namespace tmg_hero.tests;

public class GameControllerTests
{
    [Test]
    [Apartment(ApartmentState.STA)]
    public async Task ShouldLoadSave()
    {
        var saveGameManager = new SaveGameManager();
        var page = await saveGameManager.OpenGameAsync();
        var saveData = await File.ReadAllTextAsync("TestSaves/5Mines.txt");
        await SaveGameManager.LoadSaveGame(saveData, page);

        var result = await saveGameManager.IsGameLoaded();
        result.Should().BeTrue();
    }

    [Test]
    [Apartment(ApartmentState.STA)]
    public async Task ShouldDismissPopup()
    {
        var saveGameManager = new SaveGameManager();
        var page = await saveGameManager.OpenGameAsync();
        var saveData = await File.ReadAllTextAsync("TestSaves/5Mines.txt");
        await SaveGameManager.LoadSaveGame(saveData, page);

        var result = await saveGameManager.IsGameLoaded();
        result.Should().BeTrue();
        
        var manager = new BuildingManager(page);
        var mine = await manager.GetBuildingFromName("Mine");
        mine.Should().NotBeNull();
        await mine!.BuyAsync();
        var locator = page.GetByRole(Microsoft.Playwright.AriaRole.Heading).GetByText("The depths of Theresmore");
        var numMinePopups = await locator.CountAsync();
        numMinePopups.Should().Be(1);

        await GameInterface.DismissPopupAsync(page);
        locator = page.GetByRole(Microsoft.Playwright.AriaRole.Heading).GetByText("The depths of Theresmore");
        numMinePopups = await locator.CountAsync();
        numMinePopups.Should().Be(0);
    }

    [Test]
    [Apartment(ApartmentState.STA)]
    public async Task ShouldDismissPopupForMarket()
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
        await market!.BuyAsync();
        var locator = page.GetByRole(Microsoft.Playwright.AriaRole.Heading).GetByText("The Market");
        var numMinePopups = await locator.CountAsync();
        numMinePopups.Should().Be(1);

        await GameInterface.DismissPopupAsync(page);
        locator = page.GetByRole(Microsoft.Playwright.AriaRole.Heading).GetByText("The Market");
        numMinePopups = await locator.CountAsync();
        numMinePopups.Should().Be(0);
    }
}
