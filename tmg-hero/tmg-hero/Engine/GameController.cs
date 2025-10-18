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
    public bool IsPlaying => _isPlaying;

    public event EventHandler<string>? StatusChanged;
    public event EventHandler<GameState>? GameStateUpdated;

    public GameController()
    {
        _isPlaying = false;
        _strategies = new List<IStrategy>() { new BuildAtCapStrategy() };
        _saveGameManager = new SaveGameManager();
    }

    public async Task InitializeAsync(string? saveGameData = null)
    {
        if (Page is null)
        {
            StatusChanged?.Invoke(this, "Opening browser...");
            Page = await _saveGameManager.OpenGameAsync();
            if (!string.IsNullOrEmpty(saveGameData))
            {
                StatusChanged?.Invoke(this, "Loading save data...");
                await SaveGameManager.LoadSaveGame(saveGameData, Page);
            }
            StatusChanged?.Invoke(this, "Game initialized successfully");
        }
    }

    public async Task PlayGameAsync(CancellationToken cancellationToken)
    {
        if (Page is null)
        {
            throw new InvalidOperationException("Game not initialized. Call InitializeAsync first.");
        }
        _isPlaying = true;
        StatusChanged?.Invoke(this, "Bot started");

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

            // Fire the GameStateUpdated event
            GameStateUpdated?.Invoke(this, gameState);

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
        StatusChanged?.Invoke(this, "Bot stopped");
    }

    public void Dispose()
    {
        _saveGameManager.Dispose();
    }
}
