using System.Diagnostics;

namespace YMCL.Updater
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Args Error");
                Console.ReadKey();
                return;
            }
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("更新将在5秒后开始");
            Console.CursorVisible = false;
            int totalSeconds = 5;
            for (int i = 0; i <= totalSeconds; i++)
            {
                Console.Write("[");
                Console.Write(new string('#', i * 5));
                Console.Write(new string(' ', (totalSeconds - i) * 5));
                Console.Write("] ");
                Console.Write(i);
                Console.Write("/");
                Console.Write(totalSeconds);
                Console.Write(" s");

                Thread.Sleep(1000);

                if (i < totalSeconds)
                {
                    Console.SetCursorPosition(0, Console.CursorTop);
                }
            }
            Console.WriteLine("\n");
            Console.WriteLine($"{args[1]} --> {args[0]}");
            Console.CursorVisible = true;
            try
            {
                File.Move(args[0], args[1], true);
                File.Delete(args[0]);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Successfully");
                Process.Start(args[1]);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error：" + ex.Message + "\n\n" + ex.ToString());
                Console.ReadKey();
            }
        }
    }
}
