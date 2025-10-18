using Microsoft.Playwright;
using TMG.Core.Engine;

namespace TMG.Core.Strategies;

internal interface IStrategy
{
        Task Execute(GameState gameState, IPage page);
}
