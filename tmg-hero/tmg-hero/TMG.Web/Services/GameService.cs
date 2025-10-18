using Microsoft.AspNetCore.SignalR;
using tmg_hero.Engine;
using TMG.Web.Hubs;

namespace TMG.Web.Services;

public class GameService
{
    private readonly GameController _gameController;
    private readonly IHubContext<GameHub> _hubContext;
    private CancellationTokenSource? _cancellationTokenSource;

    public GameService(GameController gameController, IHubContext<GameHub> hubContext)
    {
        _gameController = gameController;
        _hubContext = hubContext;

        // Subscribe to game controller events
        _gameController.StatusChanged += OnStatusChanged;
        _gameController.GameStateUpdated += OnGameStateUpdated;
    }

    public async Task<object> GetStatusAsync()
    {
        return new
        {
            isPlaying = _gameController.IsPlaying,
            hasPage = _gameController.Page != null,
            timestamp = DateTime.UtcNow
        };
    }

    public async Task InitializeAsync(string? saveData = null)
    {
        await _gameController.InitializeAsync(saveData);
        await _hubContext.Clients.All.SendAsync("StatusChanged", "Game initialized");
    }

    public async Task StartAsync()
    {
        if (_gameController.Page == null)
        {
            throw new InvalidOperationException("Game not initialized. Please initialize first.");
        }

        if (_gameController.IsPlaying)
        {
            throw new InvalidOperationException("Bot is already running.");
        }

        _cancellationTokenSource = new CancellationTokenSource();

        // Start the game loop in background
        _ = Task.Run(async () =>
        {
            try
            {
                await _gameController.PlayGameAsync(_cancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
                // Expected when stopping
            }
            catch (Exception ex)
            {
                await _hubContext.Clients.All.SendAsync("Error", $"Bot error: {ex.Message}");
            }
        });

        await _hubContext.Clients.All.SendAsync("StatusChanged", "Bot started");
    }

    public async Task StopAsync()
    {
        _cancellationTokenSource?.Cancel();
        _gameController.StopPlaying();
        await _hubContext.Clients.All.SendAsync("StatusChanged", "Bot stopped");
    }

    public async Task<string> GetSaveAsync()
    {
        if (_gameController.Page == null)
        {
            throw new InvalidOperationException("Game not initialized.");
        }

        return await SaveGameManager.GetSaveGame(_gameController.Page);
    }

    private async void OnStatusChanged(object? sender, string status)
    {
        await _hubContext.Clients.All.SendAsync("StatusChanged", status);
    }

    private async void OnGameStateUpdated(object? sender, GameState gameState)
    {
        var update = new
        {
            buildings = gameState.Buildings.Count,
            resources = gameState.Resources.ToDictionary(r => r.Key, r => new
            {
                amount = r.Value.Amount,
                cap = r.Value.Cap,
                income = r.Value.Income
            }),
            timestamp = DateTime.UtcNow
        };

        await _hubContext.Clients.All.SendAsync("GameStateUpdated", update);
    }
}