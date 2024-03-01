using MinecraftLaunch.Components.Installer;

public class Program
{
    public static async Task Main()
    {
        var forge = await ForgeInstaller.EnumerableFromVersionAsync("1.12.2");

        Console.ReadKey();
    }
}
