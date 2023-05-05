using tmg_hero.Engine;

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
}
