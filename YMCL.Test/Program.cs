using MinecraftLaunch.Classes.Interfaces;
using MinecraftLaunch.Classes.Models.Auth;
using MinecraftLaunch.Classes.Models.Launch;
using MinecraftLaunch.Components.Authenticator;
using MinecraftLaunch.Components.Launcher;
using MinecraftLaunch.Components.Resolver;
using System;

namespace YMCL.Test
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            OfflineAuthenticator authenticator = new("DaiYu");
            var account = authenticator.Authenticate();

            LaunchConfig config = new LaunchConfig
            {
                Account = account,
                GameWindowConfig = new GameWindowConfig
                {
                    IsFullscreen = false
                },
                JvmConfig = new JvmConfig("C:\\Program Files\\Common Files\\Oracle\\Java\\javapath\\javaw.exe")
                {
                    MaxMemory = 2048
                },
                IsEnableIndependencyCore = true
            };

            var resolver = new GameResolver("D:\\minecraft\\.minecraft");
            Launcher launcher = new(resolver, config);

            var watcher = await launcher.LaunchAsync("1.19.2");

            Console.WriteLine("Launched");

            watcher.Process.Exited += (sender, args) =>
            {
                Console.WriteLine("Exited");
                Console.WriteLine(args.ToString());
            };

            watcher.Process.OutputDataReceived += (sender, args) =>
            {
                Console.WriteLine(args.ToString());
            };

            Console.ReadKey();
        }
    }
}
