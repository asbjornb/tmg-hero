using NUnit.Framework.Internal;
using TMG.Core.Model;

namespace TMG.Core.Tests;

public class ResourceTests
{
    [Test]
    public void ShouldParseResourceWithThousandsSeparator()
    {
        const string Input = "Research\t17,426 / 54,840\t62.06/s";
        Resource.TryParseResourceFromText(Input, out Resource? result).Should().BeTrue();

        result.Should().NotBeNull();
        result!.Name.Should().Be("Research");
        result.Amount.Should().Be(17426);
        result.Cap.Should().Be(54840);
        result.Income.Should().BeApproximately(62.06, 1e-6);
    }

    [Test]
    public void ShouldParseResourceWithKMultiplier()
    {
        const string Input = "Wood\t33,615 / 100.1K\t26.16/s";
        Resource.TryParseResourceFromText(Input, out Resource? result).Should().BeTrue();

        result.Should().NotBeNull();
        result!.Name.Should().Be("Wood");
        result.Amount.Should().Be(33615);
        result.Cap.Should().Be(100100);
        result.Income.Should().BeApproximately(26.16, 1e-6);
    }

    [Test]
    public void ShouldNotParseInvalidResource()
    {
        const string Input = "InvalidResource\t33,615 / 100.1K";
        Resource.TryParseResourceFromText(Input, out Resource? result).Should().BeFalse();
        result.Should().BeNull();
    }

    [Test]
    public void ShouldParseRealExample()
    {
        const string Input = "Research\t17,426 / 54,840\t62.06/s\nGold\t25,380 / 41,355\t122.64/s\nFood\t29,646 / 29,646\t29.47/s\nWood\t33,615 / 100.1K\t26.16/s\nStone\t12,797 / 98,112\t22.63/s\nCopper\t25,355 / 64,667\t11.83/s\nIron\t15,436 / 64,667\t7.6/s\nTools\t2,154 / 66,199\t5.68/s\nCow\t842 / 842\t0.68/s\nHorse\t2,316 / 4,121\t0.34/s\nFaith\t12,711 / 15,320\t28.9/s\nMana\t9,145 / 9,145\t15.73/s\nMaterials\t710 / 8,498\t2.38/s\nSteel\t2,706 / 9,142\t2.37/s";
        foreach (var line in Input.Split('\n'))
        {
            Resource.TryParseResourceFromText(line, out Resource? result).Should().BeTrue();
            result.Should().NotBeNull();
        }
    }

    [Test] 
    public void ShouldParseRealExample2()
    {
        const string Input = "Research\t23,497 / 131.6K\t105.19/s\nGold\t33,623 / 150.1K\t208.58/s\nFood\t6,001 / 34,973\t22.68/s\nWood\t104.0K / 201.7K\t71.82/s\nStone\t19,085 / 201.7K\t64.62/s\nCopper\t126.3K / 126.3K\t48.05/s\nIron\t72,410 / 126.3K\t30.92/s\nTools\t4,426 / 128.4K\t30.62/s\nCow\t777 / 5,088\t1.2/s\nHorse\t405 / 7,082\t0.6/s\nFaith\t26,642 / 27,491\t59.84/s\nMana\t8,078 / 18,667\t19.45/s\nMaterials\t563 / 41,623\t7.45/s\nSteel\t7,846 / 43,330\t6.69/s\nCrystal\t4,004 / 34,826\t3.27/s\nSupplies\t2,255 / 27,868\t3.49/s";
        foreach (var line in Input.Split('\n'))
        {
            Resource.TryParseResourceFromText(line, out Resource? result).Should().BeTrue();
            result.Should().NotBeNull();
        }
    }
}
