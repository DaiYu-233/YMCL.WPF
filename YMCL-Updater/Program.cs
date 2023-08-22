using System.Diagnostics;

namespace YMCL_Updater
{
    internal class Program
    {
        static void Up()
        {
            Console.WriteLine("[Info]  YMCL更新已开始");
            if (!File.Exists("./YMCL-Temp.exe"))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[Error]  YMCL-Temp.exe 不存在");
                Console.WriteLine("[Error]  YMCL更新非正常结束");
                return;
            }
            try
            {
                File.Delete("./YMCL.exe");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[Error]  YMCL.exe 删除失败");
                Console.WriteLine(ex);
                Console.WriteLine("[Error]  YMCL更新非正常结束");


                Console.WriteLine(ex);
                Console.WriteLine("[Error]  YMCL更新非正常结束");
                return;
            }
            finally
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[Info]  删除：YMCL.exe");
                Console.ResetColor();
            }
            try
            {
                File.Move("./YMCL-Temp.exe", "./YMCL.exe");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[Error]  YMCL-Temp.exe -> YMCL.exe 重命名失败");

                Console.WriteLine(ex);
                Console.WriteLine("[Error]  YMCL更新非正常结束");

                return;
            }
            finally
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[Info]  重命名：YMCL-Temp.exe -> YMCL.exe");
                Console.ResetColor();
            }
            Console.WriteLine("[Info]  YMCL更新完成");
        }
        static void Main(string[] args)
        {
            //Console.WriteLine("Hello, World!");
            Up();
            Console.ResetColor();
            Process.Start("./YMCL.exe");
            Console.WriteLine("\n单击任意键退出  电源键也可以！ヾ(≧▽≦*)o");
            Console.ReadKey();
        }
    }
}