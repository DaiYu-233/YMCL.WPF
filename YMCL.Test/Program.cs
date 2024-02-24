using MinecraftLaunch.Components.Installer;

public class Program
{
    public static void Main()
    {
        var forge = ForgeInstaller.EnumerableFromVersionAsync("1.12.2");

        Console.ReadKey();
    }
}
