using MinecraftLaunch.Classes.Interfaces;
using MinecraftLaunch.Classes.Models.Auth;
using MinecraftLaunch.Classes.Models.Launch;
using MinecraftLaunch.Components.Authenticator;
using MinecraftLaunch.Components.Installer;
using MinecraftLaunch.Components.Launcher;
using MinecraftLaunch.Components.Resolver;
using System;
using System.Text.RegularExpressions;

namespace YMCL.Test
{
    internal class Program
    {
        //static async Task Main(string[] args)
        //{
        //    var resolver = new GameResolver("D:\\minecraft\\.minecraft");
        //    VanlliaInstaller installer = new(resolver, "1.14.1");

        //    installer.ProgressChanged += Installer_ProgressChanged;

        //    await installer.InstallAsync();

        //    Console.ReadKey();
        //}

        //private static void Installer_ProgressChanged(object? sender, MinecraftLaunch.Classes.Models.Event.ProgressChangedEventArgs e)
        //{
        //    Console.WriteLine(e.ProgressStatus.ToString());
        //}

        static async Task Main(string[] args)
        {
            var account = new OfflineAuthenticator("Yang114").Authenticate();
            var resolver = new GameResolver(".minecraft");

            var config = new LaunchConfig
            {
                Account = account,
                IsEnableIndependencyCore = true,
                JvmConfig = new(@"C:\Program Files\Java\jdk1.8.0_301\bin\javaw.exe")
                {
                    MaxMemory = 1024,
                }
            };

            await Task.Run(async () =>
            {
                Launcher launcher = new(resolver, config);
                var gameProcessWatcher = await launcher.LaunchAsync("1.12.2");

                //获取输出日志
                gameProcessWatcher.OutputLogReceived += (sender, args) =>
                {
                    Console.WriteLine(args.Text);
                };

                //检测游戏退出
                gameProcessWatcher.Exited += (sender, args) =>
                {
                    Console.WriteLine("exit");
                };
            });
        }
    }
}
