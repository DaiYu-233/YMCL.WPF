using MinecraftLaunch.Classes.Enums;
using MinecraftLaunch.Components.Analyzer;
using MinecraftLaunch.Components.Fetcher;
using MinecraftLaunch.Components.Installer;
using MinecraftLaunch.Components.Resolver;

public class Program
{
    public static async Task Main()
    {
        CurseForgeFetcher curseForgeFetcher = new("$2a$10$ndSPnOpYqH3DRmLTWJTf5Ofm7lz9uYoTGvhSj0OjJWJ8WdO4ZTsr.");
        var entries = (await curseForgeFetcher.SearchResourcesAsync("jei")).ToList();
    }
}
