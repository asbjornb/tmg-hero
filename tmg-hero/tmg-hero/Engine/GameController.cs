using Microsoft.Playwright;
using System.Diagnostics;
using tmg_hero.Dialogs;

namespace tmg_hero.Engine;

public class GameController
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
            LoadFromSaveDialog.ShowLoadFromSaveDialog(InjectSaveGameData);
        }
        _isPlaying = true;

        var resourceManager = new ResourceManager(_page!);
        var buildingManager = new BuildingManager(_page!);

        while (_isPlaying)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _isPlaying = false;
                break;
            }
            var stopwatch = Stopwatch.StartNew();
            var resources = await resourceManager.GetResourceDataAsync();
            var buildings = await buildingManager.GetBuildingDataAsync();
            Console.WriteLine($"Reading gamestate took {stopwatch.ElapsedMilliseconds}ms");
            //If any resources are at cap build any building we can afford that uses that resource
            var cappedResources = resources.Values.Where(r => r.IsCapped);
            foreach(var resource in cappedResources)
            {
                var buildingsThatLowerCap = buildings.Where(b => b.Cost.ContainsKey(resource.Name) && b.Cost[resource.Name] <= resource.Amount);
                var affordableBuildings = buildingsThatLowerCap.Where(b => b.Cost.All(c => resources[c.Key].Amount >= c.Value));
                var doesNotGiveNegativeTotalIncome = affordableBuildings.Where(b => b.NegativeIncomes().All(n => resources[n.resource].Income + n.amount >= 0));
                //Take a random that is not null
                var first = doesNotGiveNegativeTotalIncome.OrderBy(_ => Guid.NewGuid()).FirstOrDefault();
                if(first != default)
                {
                    await first.Buy();
                    foreach (var cost in first.Cost)
                    {
                        resources[cost.Key].Amount -= cost.Value;
                    }
                    break;
                }
            }

            await Task.Delay(1000, cancellationToken); // Adjust this value to set the interval between interactions
        }
    }

    public async Task InjectSaveGameData(string saveData)
    {
        // Initialize a new Playwright instance if not already initialized
        if (_browser == null)
        {
            _playwright = await Playwright.CreateAsync();
            _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
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
        await _page.Keyboard.UpAsync("Control");
    }

    public void StopPlaying()
    {
        _isPlaying = false;
    }
}