using Microsoft.Playwright;
using System.Diagnostics;
using tmg_hero.Dialogs;
using tmg_hero.Strategies;

namespace tmg_hero.Engine;

internal class GameController : IDisposable
{
    private const string Url = "https://www.theresmoregame.com/play/";
    private bool _isPlaying;
    private IPlaywright? _playwright;
    private IBrowser? _browser;
    private IBrowserContext? _browserContext;
    private readonly List<IStrategy> _strategies;
    public IPage? Page { get; private set; }

    public GameController()
    {
        _isPlaying = false;
        _strategies = new List<IStrategy>() { new BuildAtCapStrategy() };
    }

    public async Task PlayGameAsync(CancellationToken cancellationToken)
    {
        if (Page is null)
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

            var stopwatch = Stopwatch.StartNew();
            var gameState = new GameState(Page!);
            await gameState.Initialize();

            Console.WriteLine($"Reading gamestate took {stopwatch.ElapsedMilliseconds}ms");

            foreach (var strategy in _strategies)
            {
                await strategy.Execute(gameState, this);
            }

            await Task.Delay(1000, cancellationToken);
        }
    }

    public async Task<bool> IsGameLoaded()
    {
        if (Page is null)
        {
            return false;
        }
        try
        {
            var buttonLocator = Page.GetByRole(AriaRole.Tab).GetByText("Build");
            var buttonCount = await buttonLocator.CountAsync();

            return buttonCount == 1;
        }
        catch (TimeoutException)
        {
            return false;
        }
    }

    public async Task DismissPopupAsync()
    {
        var portalRootLocator = Page!.Locator("#headlessui-portal-root");
        var closeBtnLocator = portalRootLocator.GetByRole(AriaRole.Button).GetByText("Close");

        if (await closeBtnLocator.CountAsync() > 0)
        {
            await closeBtnLocator.ClickAsync(); // Click the "Close" button found in the headlessui-portal-root subtree
            // Closing is fluent and takes a while, so wait for it to be gone
            await Task.Delay(500);
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
        Page ??= await _browserContext.NewPageAsync();

        await Page.GotoAsync(Url);
        await Page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

        await Page.GetByRole(AriaRole.Banner).GetByRole(AriaRole.Button).Nth(3).ClickAsync();

        await Page.ClickAsync("text=Import from clipboard");

        await Page.ClickAsync("text=Click here to paste a save");
        Clipboard.SetText(saveData);

        await Page.Keyboard.DownAsync("Control");
        await Page.Keyboard.PressAsync("KeyV");
        var upKey = Page.Keyboard.UpAsync("Control");
        await Page.WaitForSelectorAsync("text=The game has been loaded from the save, please wait");
        await upKey;
#pragma warning disable CS0612 // Type or member is obsolete
        await Page.WaitForNavigationAsync();
#pragma warning restore CS0612 // Type or member is obsolete
    }

    public void StopPlaying()
    {
        _isPlaying = false;
    }

    public void Dispose()
    {
        _browser?.DisposeAsync().AsTask().GetAwaiter().GetResult();
    }
}