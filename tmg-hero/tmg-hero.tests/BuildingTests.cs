using tmg_hero.Model;

namespace tmg_hero.tests;

public class BuildingTests
{
    [Test]
    public void ShouldParseCommonHouse()
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

    [Test]
    public void ShouldParseFarm()
    {
        const string Input = "Farm\n\nThe peasants will feed our great nation\n\nGold\t75\nWood\t180\nFarmer\t+1\nFarmer food\t+1%\nFood cap\t+240";
        var result = Building.TryParseBuildingFromTooltipText(Input, null);
        result.Should().NotBeNull();
        result!.Name.Should().Be("Farm");

        result.Cost.Should().Contain(new KeyValuePair<string, int>("Gold", 75));
        result.Cost.Should().Contain(new KeyValuePair<string, int>("Wood", 180));
        result.Cost.Should().HaveCount(2);
        result.Production.Should().BeEmpty();
        result.Population.Should().BeNull();
    }

    [Test]
    public void ShouldParseMarket()
    {
        const string Input = "Marketplace\n\nFrom a small provincial market to the monopoly of entire nations\n\nGold\t1,200\nWood\t600\nCopper\t400\nIron\t400\nTools\t400\nMerchant\t+1\nMerchant gold\t+2%\nArtisan tools\t+2%\nArtisan Gold\t+2%\nGold cap\t+750\nTools cap\t+200";
        var result = Building.TryParseBuildingFromTooltipText(Input, null);
        result.Should().NotBeNull();
        result!.Name.Should().Be("Marketplace");

        result.Cost.Should().Contain(new KeyValuePair<string, int>("Gold", 1200));
        result.Cost.Should().Contain(new KeyValuePair<string, int>("Wood", 600));
        result.Cost.Should().Contain(new KeyValuePair<string, int>("Copper", 400));
        result.Cost.Should().Contain(new KeyValuePair<string, int>("Iron", 400));
        result.Cost.Should().Contain(new KeyValuePair<string, int>("Tools", 400));
        result.Cost.Should().HaveCount(5);
        result.Production.Should().BeEmpty();
        result.Population.Should().BeNull();
    }
}
