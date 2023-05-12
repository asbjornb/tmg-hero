using Microsoft.Playwright;

namespace tmg_hero.Engine;

public class SaveGameManager : IDisposable
{
    private const string Url = "https://www.theresmoregame.com/play/";
    private IBrowser? _browser;
    private IPage? _page;

    public async Task<IPage> OpenGameAsync()
    {
        // Initialize a new Playwright instance if not already initialized
        if (_browser == null)
        {
            var playwright = await Playwright.CreateAsync();
            _browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
        }

        if (_page is null)
        {
            var browserContext = await _browser.NewContextAsync();
            _page = await browserContext.NewPageAsync();

            await _page.GotoAsync(Url);
            await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
        }
        return _page;
    }

    public static async Task LoadSaveGame(string saveData, IPage page, bool showBuildingsAtCap = false)
    {
        await page.GetByRole(AriaRole.Banner).GetByRole(AriaRole.Button).Nth(3).ClickAsync();

        if (!showBuildingsAtCap)
        {
            await page.Locator("#headlessui-switch-\\:r8\\:").ClickAsync();
        }

        await page.ClickAsync("text=Import from clipboard");

        await page.ClickAsync("text=Click here to paste a save");
        TextCopy.ClipboardService.SetText(saveData);

        await page.Keyboard.DownAsync("Control");
        await page.Keyboard.PressAsync("KeyV");
        var upKey = page.Keyboard.UpAsync("Control");
        await page.WaitForSelectorAsync("text=The game has been loaded from the save, please wait");
        await upKey;
#pragma warning disable CS0612 // Type or member is obsolete
        await page.WaitForNavigationAsync();
#pragma warning restore CS0612 // Type or member is obsolete
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

    public void Dispose()
    {
        _browser?.DisposeAsync().AsTask().GetAwaiter().GetResult();
    }
}
