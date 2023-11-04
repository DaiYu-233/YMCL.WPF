using System.Diagnostics;

namespace Updater
{
    internal class Program
    {
        static string Old = string.Empty;
        static string New = string.Empty;
        static bool Success = false;
        static void Main(string[] args)
        {
            Thread.Sleep(1000);
            if (args.Length != 2)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[Error]  参数错误");
                Console.WriteLine("[Error]  YMCL更新非正常结束");
                Console.ResetColor();
                Console.WriteLine("\n单击任意键退出  电源键也可以！ヾ(≧▽≦*)o");
                Console.ReadKey();
                Environment.Exit(0);
            }
            else
            {
                Old = args[0];
                New = args[1];
                Up();
                Console.ResetColor();
                Console.WriteLine("\n单击任意键退出  电源键也可以！ヾ(≧▽≦*)o");
                Console.ReadKey();
                if (Success)
                {
                    Process.Start($"{New}");
                }
                Environment.Exit(0);
            }
        }

        static void Up()
        {

            Console.WriteLine("[Info]  YMCL更新已开始");
            if (!File.Exists($"{Old}"))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[Error]  {Old} 不存在");
                Console.WriteLine("[Error]  YMCL更新非正常结束");
                return;
            }
            try
            {
                File.Delete($"{New}");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[Error]  {New} 删除失败");
                Console.WriteLine(ex);
                Console.WriteLine("[Error]  YMCL更新非正常结束");


                Console.WriteLine(ex);
                Console.WriteLine("[Error]  YMCL更新非正常结束");
                return;
            }
            finally
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"[Info]  删除：{New}");
                Console.ResetColor();
            }
            try
            {
                File.Move($"{Old}", $"{New}");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[Error]  {Old} -> {New} 重命名失败");

                Console.WriteLine(ex);
                Console.WriteLine("[Error]  YMCL更新非正常结束");

                return;
            }
            finally
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"[Info]  重命名：{Old} -> {New}");
                Console.ResetColor();
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("[Info]  YMCL更新完成");
            Success = true;
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}