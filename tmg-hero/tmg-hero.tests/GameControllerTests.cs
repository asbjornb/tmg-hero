using tmg_hero.Model;

namespace tmg_hero.tests;

public class GameControllerTests
{
    [Test]
    public void ShouldPassRegex()
    {
        const string Input = "Common House\n\nA small place to live\n\nWood\t206\nStone\t137\nFood\t-1/s\nGold\t+0.2/s\nResearch\t+0.3/s\nPopulation\t+1";
        var result = Building.TryParseBuildingFromTooltipText(Input, null);
        result.Should().NotBeNull();
        result!.Name.Should().Be("Common House");

        result.Cost.Should().Contain(new KeyValuePair<string, int>("Wood", 206));
        result.Cost.Should().Contain(new KeyValuePair<string, int>("Stone", 137));
        result.Production.Should().Contain(new KeyValuePair<string, double>("Food", -1));
        result.Production.Should().Contain(new KeyValuePair<string, double>("Gold", 0.2));
        result.Production.Should().Contain(new KeyValuePair<string, double>("Research", 0.3));
        result.Population.Should().Be(1);
    }
}
