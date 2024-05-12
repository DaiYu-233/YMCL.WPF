using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Panuon.WPF.UI;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Media.Imaging;
using YMCL.Main.Public.Lang;

namespace YMCL.Main.Public
{
    internal class Method
    {
        public static void ParameterProcessing()
        {
            if (App.StartupArgs.Length == 0) return;
            MessageBoxX.Show(App.StartupArgs[0]);
            var urlScheme = System.Web.HttpUtility.UrlDecode(App.StartupArgs[0]);
            urlScheme = urlScheme.Substring(7).Trim('"');
            if (urlScheme.EndsWith("/"))
                urlScheme = urlScheme.TrimEnd('/');
            MessageBoxX.Show(urlScheme);
            foreach (Match match in Regex.Matches(urlScheme, @"--\w+(\s+('[^']*'|[^'\s]+))*?(?=\s*--\w+|$)"))
            {
                var value = match.Value.Trim().Substring(2, match.Value.Trim().Length - 2);
                var method = value.Split(' ')[0];
                List<string> parameters = new List<string>();
                Regex.Matches(match.Value, @"(?<quote>')(.*?)(?<-quote>')|([^\s']+)", RegexOptions.ExplicitCapture).ToList().ForEach(item =>
                {
                    parameters.Add(item.Value.TrimStart('\'').TrimEnd('\''));
                });
                parameters.RemoveAt(0);
                try
                {
                    switch (method)
                    {
                        case "launch":
                            if (parameters.Count >= 1)
                            {
                                string versionId = parameters[0];
                                string minecraftPath = parameters.Count >= 2 && parameters[1] != null ? parameters[1] : "";
                                string serverIP = parameters.Count >= 3 && parameters[2] != null ? parameters[2] : "";
                                Const.Window.main.launch.LaunchClient(versionId, minecraftPath, msg: false, serverIP: serverIP);
                            }
                            else
                            {
                                LauncherErrorShow(LangHelper.Current.GetText("ArgsError"));
                            }
                            break;
                        case "import":
                            if (parameters[0] == "setting")
                            {
                                var hexString = parameters[1];
                                byte[] hexBytes = Enumerable.Range(0, hexString.Length / 2)
                                    .Select(i => Convert.ToByte(hexString.Substring(i * 2, 2), 16))
                                    .ToArray();
                                string data = Encoding.ASCII.GetString(hexBytes);
                                MessageBoxX.Show(data);
                                var source = JObject.FromObject(JsonConvert.DeserializeObject<Class.Setting>(File.ReadAllText(Const.SettingDataPath)));
                                var import = JObject.Parse(data);
                                source.Merge(import, new JsonMergeSettings
                                {
                                    MergeArrayHandling = MergeArrayHandling.Union
                                });
                                File.WriteAllText(Const.SettingDataPath, JsonConvert.SerializeObject(source, Formatting.Indented));
                                MessageBoxX.Show($"\n{MainLang.ImportFinish}\n\n{data}", "Yu Minecraft Launcher");
                                RestartApp();
                            }
                            break;
                    }
                }
                catch (Exception ex)
                {
                    LauncherErrorShow(LangHelper.Current.GetText("ArgsError"), ex);
                }
            }
        }
        public static void ShowWin10Notice(string msg, string title = "Yu Minecraft Launcher", string logoUri = "")
        {
            string logo = string.Empty;
            if (string.IsNullOrEmpty(logoUri))
            {
                logo = Path.Combine(Const.PublicDataRootPath, "Icon.ico");
            }
            else
            {
                logo = logoUri;
            }
            Process.Start(Path.Combine(Const.PublicDataRootPath, "YMCL.Notifier.exe"), new string[]
            {
                $"'{title}'",
                $"'{msg}'",
                $"'{logo}'"
            });
        }
        public static void LauncherErrorShow(string errorTypeMsg, Exception exception = null, bool useToast = false, WindowX window = null)
        {
            string message;
            if (exception != null)
                message = useToast ? $"{errorTypeMsg}：{exception.Message}" : $"{errorTypeMsg}：{exception.Message}\n\n{exception.ToString()}";
            else
                message = errorTypeMsg;
            if (useToast && window != null)
                Toast.Show(message: message, position: ToastPosition.Top, window: window);
            else
                MessageBoxX.Show(message, "Yu Minecraft Launcher");
        }
        public static double GetDirectoryLength(string dirPath)
        {
            //判断给定的路径是否存在,如果不存在则退出
            if (!Directory.Exists(dirPath))
                return 0;
            double len = 0;

            //定义一个DirectoryInfo对象
            DirectoryInfo di = new DirectoryInfo(dirPath);

            //通过GetFiles方法,获取di目录中的所有文件的大小
            foreach (FileInfo fi in di.GetFiles())
            {
                len += fi.Length;
            }

            //获取di中所有的文件夹,并存到一个新的对象数组中,以进行递归
            DirectoryInfo[] dis = di.GetDirectories();
            if (dis.Length > 0)
            {
                for (int i = 0; i < dis.Length; i++)
                {
                    len += GetDirectoryLength(dis[i].FullName);
                }
            }
            return len;
        }
        public static void RestartApp()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                UseShellExecute = true,
                WorkingDirectory = Environment.CurrentDirectory,
                FileName = Application.ExecutablePath
            };
            Process.Start(startInfo);
            System.Windows.Application.Current.Shutdown();
        }
        public static void CreateFolder(string path)
        {
            if (!Directory.Exists(path))
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(path);
                directoryInfo.Create();
            }
        }
        public static string GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();//精确到秒
        }
        public static BitmapImage Base64ToImage(string base64)
        {
            byte[] bytes = Convert.FromBase64String(base64);
            MemoryStream ms = new MemoryStream(bytes);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.StreamSource = ms;
            image.EndInit();
            return image;
        }
        public static string BytesToBase64(byte[] imageBytes)
        {
            string base64String = Convert.ToBase64String(imageBytes);
            return base64String;
        }
        public static object RunCodeByString(string code, object[] args = null, string[] dlls = null)
        {
            //Nuget Microsoft.CodeAnalysis.CSharp
            //Type type = null;
            //SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(code);
            //CSharpCompilation cSharpCompilation = CSharpCompilation.Create("CustomAssembly")
            //    .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
            //    .AddReferences(MetadataReference.CreateFromFile(typeof(object).Assembly.Location))
            //    .AddSyntaxTrees(syntaxTree);
            //if (dlls != null)
            //{
            //    foreach (string dll in dlls)
            //    {
            //        if (!string.IsNullOrEmpty(dll))
            //        {
            //            cSharpCompilation.AddReferences(MetadataReference.CreateFromFile(dll));
            //        }
            //    }
            //}
            //MemoryStream memoryStream = new MemoryStream();
            //EmitResult emitResult = cSharpCompilation.Emit(memoryStream);
            //if (emitResult.Success)
            //{
            //    memoryStream.Seek(0, SeekOrigin.Begin);
            //    Assembly assembly = AssemblyLoadContext.Default.LoadFromStream(memoryStream);
            //    type = assembly.GetType("YMCLRunner");
            //}
            //else
            //{
            //    var str = string.Empty;
            //    foreach (var item in emitResult.Diagnostics)
            //    {
            //        str += $"----> {item}\n";
            //    }
            //    MessageBoxX.Show($"\n{LangHelper.Current.GetText("ComPileCSharpError")}\n\n{str}", "Yu Minecraft Launcher");
            //    type = null;
            //}
            //if (type != null)
            //{
            //    object? obj = Activator.CreateInstance(type);
            //    MethodInfo? methodInfo = type.GetMethod("Main");
            //    object? result = methodInfo.Invoke(obj, args);
            //    //MessageBoxX.Show($"Result: {result}");
            //    return result;
            //}
            return null;
        }
        public static string MsToTime(double ms)//转换为分秒格式
        {
            int minute = 0;
            int second = 0;
            second = (int)(ms / 1000);

            string secondStr = string.Empty;
            string minuteStr = string.Empty;

            if (second > 60)
            {
                minute = second / 60;
                second = second % 60;
            }

            secondStr = second < 10 ? $"0{second}" : $"{second}";
            minuteStr = minute < 10 ? $"0{minute}" : $"{minute}";

            return $"{minuteStr}:{secondStr}";
        }
    }
}
