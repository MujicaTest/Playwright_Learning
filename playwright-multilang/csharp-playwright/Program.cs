using Microsoft.Playwright;

class Program
{
    public static async Task Main()
    {
        using var playwright = await Playwright.CreateAsync();
        var browser = await playwright.Chromium.LaunchAsync(new() { Headless = false });
        var page = await browser.NewPageAsync();
        await page.GotoAsync("https://playwright.dev");
        var title = await page.TitleAsync();
        Console.WriteLine("Page Title: " + title);
        await browser.CloseAsync();
    }
}