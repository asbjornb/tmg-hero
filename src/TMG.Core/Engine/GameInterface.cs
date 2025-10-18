using Microsoft.Playwright;

namespace TMG.Core.Engine;

public static class GameInterface
{
    public static async Task DismissPopupAsync(IPage page)
    {
        var portalRootLocator = page.Locator("#headlessui-portal-root");
        var closeBtnLocator = portalRootLocator.GetByRole(AriaRole.Button).GetByText("Close");

        if (await closeBtnLocator.CountAsync() > 0)
        {
            await closeBtnLocator.ClickAsync(); // Click the "Close" button found in the headlessui-portal-root subtree
            // Closing is fluent and takes a while, so wait for it to be gone
            await Task.Delay(500);
        }
    }
}
