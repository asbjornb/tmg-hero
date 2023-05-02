using Microsoft.Playwright;
using System.Diagnostics;
using tmg_hero.Dialogs;

namespace tmg_hero.Engine;

internal class GameController
{
    private const string Url = "https://www.theresmoregame.com/play/";
    private bool _isPlaying;
    private IPlaywright? _playwright;
    private IBrowser? _browser;
    private IBrowserContext? _browserContext;
    private IPage? _page;

    public GameController()
    {
        _isPlaying = false;
    }

    public async Task PlayGameAsync(CancellationToken cancellationToken)
    {
        if (_page is null)
        {
            //Display a dialog telling to import save before playing, then return
            await LoadFromSaveDialog.ShowLoadFromSaveDialog(x => InjectSaveGameData(x));
        }
        _isPlaying = true;

        while (_isPlaying)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _isPlaying = false;
                break;
            }

            var gameState = new GameState(_page!);
            await gameState.Initialize();

            var stopwatch = Stopwatch.StartNew();
            Console.WriteLine($"Reading gamestate took {stopwatch.ElapsedMilliseconds}ms");
            //If any resources are at cap build any building we can afford that uses that resource
            var cappedResources = gameState.Resources.Values.Where(r => r.IsCapped);
            foreach(var resource in cappedResources)
            {
                var buildingsThatLowerCap = gameState.Buildings.Where(b => b.Cost.ContainsKey(resource.Name) && b.Cost[resource.Name] <= resource.Amount);
                var affordableBuildings = buildingsThatLowerCap.Where(b => b.Cost.All(c => gameState.Resources[c.Key].Amount >= c.Value));
                var doesNotGiveNegativeTotalIncome = affordableBuildings.Where(b => b.NegativeIncomes().All(n => gameState.Resources[n.resource].Income + n.amount >= 0));
                //Take a random that is not null
                var first = doesNotGiveNegativeTotalIncome.OrderBy(_ => Guid.NewGuid()).FirstOrDefault();
                if(first != default)
                {
                    await first.Buy();
                    foreach (var cost in first.Cost)
                    {
                        gameState.Resources[cost.Key].Amount -= cost.Value;
                    }
                    break;
                }
            }

            await Task.Delay(1000, cancellationToken);
        }
    }

    public async Task<bool> IsGameLoaded()
    {
        if (_page is null)
        {
            return false;
        }
        try
        {
            var buttonLocator = _page.GetByRole(AriaRole.Tab).GetByText("Build");
            var buttonCount = await buttonLocator.CountAsync();

            return buttonCount == 1;
        }
        catch (TimeoutException)
        {
            return false;
        }
    }

    public async Task InjectSaveGameData(string saveData, bool headless = false)
    {
        // Initialize a new Playwright instance if not already initialized
        if (_browser == null)
        {
            _playwright = await Playwright.CreateAsync();
            _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = headless });
        }

        _browserContext ??= await _browser.NewContextAsync();
        _page ??= await _browserContext.NewPageAsync();

        await _page.GotoAsync(Url);
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

        await _page.GetByRole(AriaRole.Banner).GetByRole(AriaRole.Button).Nth(3).ClickAsync();

        await _page.ClickAsync("text=Import from clipboard");

        await _page.ClickAsync("text=Click here to paste a save");
        Clipboard.SetText(saveData);

        await _page.Keyboard.DownAsync("Control");
        await _page.Keyboard.PressAsync("KeyV");
        var upKey = _page.Keyboard.UpAsync("Control");
        await _page.WaitForSelectorAsync("text=The game has been loaded from the save, please wait");
        await upKey;
        await _page.WaitForNavigationAsync();
    }

    public void StopPlaying()
    {
        _isPlaying = false;
    }
}