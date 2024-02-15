using System.IO;
using System.Windows.Media.Imaging;

namespace YMCL.Main.Public
{
    internal class Function
    {
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
    }
}
