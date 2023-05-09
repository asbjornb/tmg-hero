using Microsoft.Playwright;
using System.Diagnostics;
using tmg_hero.Engine;

namespace tmg_hero.Strategies;

internal class BuildAtCapStrategy : IStrategy
{
    public async Task Execute(GameState gameState, IPage page)
    {
        //If any resources are at cap build any building we can afford that uses that resource
        var cappedResources = gameState.Resources.Values.Where(r => r.IsCapped);
        foreach (var resource in cappedResources)
        {
            var buildingsThatLowerCap = gameState.Buildings.Where(b => b.Cost.ContainsKey(resource.Name) && b.Cost[resource.Name] <= resource.Amount);
            var affordableBuildings = buildingsThatLowerCap.Where(b => b.Cost.All(c => gameState.Resources.ContainsKey(c.Key) && gameState.Resources[c.Key].Amount >= c.Value));
            var doesNotGiveNegativeTotalIncome = affordableBuildings.Where(b => b.NegativeIncomes().All(n => gameState.Resources[n.resource].Income + n.amount >= 0));
            //Take a random that is not null
            var first = doesNotGiveNegativeTotalIncome.OrderBy(_ => Guid.NewGuid()).FirstOrDefault();
            if (first != default)
            {
                await first.Buy();
                foreach (var cost in first.Cost)
                {
                    gameState.Resources[cost.Key].Amount -= cost.Value;
                }
                await GameInterface.DismissPopupAsync(page);
                break;
            }
        }
    }
}
