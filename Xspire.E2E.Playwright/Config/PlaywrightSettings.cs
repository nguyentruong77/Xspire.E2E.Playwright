using Microsoft.Extensions.Configuration;

namespace Xspire.E2E.Playwright.Config;

public class PlaywrightSettings
{
    public string BaseUrl { get; set; } = "";
    public bool Headless { get; set; } = true;

    public int InstantTimeoutMs { get; set; } = 1000;
    public int SmallTimeoutMs { get; set; } = 5000;
    public int StandardTimeoutMs { get; set; } = 15000;
    public int BigTimeoutMs { get; set; } = 30000;
    public int MaxTimeoutMs { get; set; } = 60000;

    // Timeout tương ứng Playwright config
    public int ExpectTimeoutMs { get; set; } = 5000;
    public int ActionTimeoutMs { get; set; } = 5000;
    public int NavigationTimeoutMs { get; set; } = 30000;
    public int TestTimeoutMs { get; set; } = 120000;

    public string ValidUser { get; set; } = "";
    public string ValidPassword { get; set; } = "";

    public static PlaywrightSettings Load()
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("Config/appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        var section = config.GetSection("Playwright");
        var settings = section.Get<PlaywrightSettings>() ?? new PlaywrightSettings();
        return settings;
    }
}