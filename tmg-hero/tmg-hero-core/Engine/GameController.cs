using Microsoft.Playwright;
using System.Diagnostics;
using tmg_hero.Strategies;

namespace tmg_hero.Engine;

public class GameController : IDisposable
{
    private bool _isPlaying;
    private readonly List<IStrategy> _strategies;
    private readonly SaveGameManager _saveGameManager;

    public IPage? Page { get; private set; }

    public GameController()
    {
        _isPlaying = false;
        _strategies = new List<IStrategy>() { new BuildAtCapStrategy() };
        _saveGameManager = new SaveGameManager();
    }

    public async Task PlayGameAsync(CancellationToken cancellationToken)
    {
        _isPlaying = true;

        while (_isPlaying)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _isPlaying = false;
                break;
            }

            var stopwatch = Stopwatch.StartNew();
            var gameState = new GameState(Page!);
            await gameState.Initialize();

            Console.WriteLine($"Reading gamestate took {stopwatch.ElapsedMilliseconds}ms");

            foreach (var strategy in _strategies)
            {
                await strategy.Execute(gameState, Page!);
            }

            await Task.Delay(1000, cancellationToken);
        }
    }

    public void StopPlaying()
    {
        _isPlaying = false;
    }

    public void Dispose()
    {
        _saveGameManager.Dispose();
    }
}
