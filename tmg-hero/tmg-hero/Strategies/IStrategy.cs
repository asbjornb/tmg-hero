using Microsoft.Playwright;
using tmg_hero.Engine;

namespace tmg_hero.Strategies;

internal interface IStrategy
{
        Task Execute(GameState gameState, IPage page);
}
