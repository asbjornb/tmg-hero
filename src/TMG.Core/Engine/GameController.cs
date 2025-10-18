using Microsoft.Playwright;
using System.Diagnostics;
using TMG.Core.Strategies;
using TMG.Core.Public.DTOs;

namespace TMG.Core.Engine;

public class GameController : IDisposable
{
    private bool _isPlaying;
    private readonly List<IStrategy> _strategies;
    private readonly SaveGameManager _saveGameManager;

    public IPage? Page { get; private set; }
    public bool IsPlaying => _isPlaying;

    public event EventHandler<string>? StatusChanged;
    public event EventHandler<GameStateDto>? GameStateUpdated;

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

            // Convert to DTO and fire event
            var gameStateDto = ConvertToDto(gameState);
            GameStateUpdated?.Invoke(this, gameStateDto);

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

    public async Task<string> GetSaveAsync()
    {
        if (Page == null)
        {
            throw new InvalidOperationException("Game not initialized.");
        }

        return await SaveGameManager.GetSaveGame(Page);
    }

    private static GameStateDto ConvertToDto(GameState gameState)
    {
        var resourceDtos = gameState.Resources.ToDictionary(
            r => r.Key,
            r => new ResourceDto
            {
                Name = r.Key,
                Amount = r.Value.Amount,
                Cap = r.Value.Cap,
                Income = r.Value.Income
            }
        );

        return new GameStateDto
        {
            BuildingCount = gameState.Buildings.Count,
            Resources = resourceDtos
        };
    }

    public void Dispose()
    {
        _saveGameManager.Dispose();
    }
}
