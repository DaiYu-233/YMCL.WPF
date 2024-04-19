using MinecraftLaunch.Components.Fetcher;
using MinecraftLaunch.Components.Installer;

public class Program
{
    public static async Task Main()
    {
        CurseForgeFetcher curseForgeFetcher = new("$2a$10$ndSPnOpYqH3DRmLTWJTf5Ofm7lz9uYoTGvhSj0OjJWJ8WdO4ZTsr.");
        var a = await curseForgeFetcher.SearchResourcesAsync("jei");

        Console.ReadKey();
    }
}
