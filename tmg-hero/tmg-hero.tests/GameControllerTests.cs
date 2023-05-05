using tmg_hero.Engine;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace tmg_hero.tests;

public class GameControllerTests
{
    [Test]
    [Apartment(ApartmentState.STA)]
    public async Task ShouldLoadSave()
    {
        var gameController = new GameController();
        var saveData = await File.ReadAllTextAsync("TestSaves/5Mines.txt");
        await gameController.InjectSaveGameData(saveData, false);

        var result = await gameController.IsGameLoaded();
        result.Should().BeTrue();
    }

    [Test]
    [Apartment(ApartmentState.STA)]
    public async Task ShouldDismissPopup()
    {
        var gameController = new GameController();
        var saveData = await File.ReadAllTextAsync("TestSaves/5Mines.txt");
        await gameController.InjectSaveGameData(saveData, false);

        var isLoaded = await gameController.IsGameLoaded();
        isLoaded.Should().BeTrue();
        var gameState = new GameState(gameController.Page!);
        await gameState.Initialize();
        await gameState.Buildings.First(x => x.Name == "Mine").Buy();
        var locator = gameController.Page!.GetByRole(Microsoft.Playwright.AriaRole.Heading).GetByText("The depths of Theresmore");
        var numMinePopups = await locator.CountAsync();
        numMinePopups.Should().Be(1);
        await gameController.DismissPopupAsync();
        locator = gameController.Page!.GetByRole(Microsoft.Playwright.AriaRole.Heading).GetByText("The depths of Theresmore");
        numMinePopups = await locator.CountAsync();
        numMinePopups.Should().Be(0);
    }
}
