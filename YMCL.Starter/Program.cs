using System.Diagnostics;

namespace YMCL.Starter
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DaiYu.YMCL", "YMCL.ExePath.DaiYu");
            var exe = File.ReadAllText(path);
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                UseShellExecute = true,
                WorkingDirectory = Path.GetDirectoryName(exe),
                FileName = exe
            };
            Process.Start(startInfo);
        }
    }
}
