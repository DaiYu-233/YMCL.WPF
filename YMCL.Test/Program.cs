using System;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using static System.Net.Mime.MediaTypeNames;

public class Program
{
    public static void Main()
    {
        string code = @"
            using System;
            public class YMCLRunner
            {
                public static void Main()
                {
                    Console.WriteLine(""Hello from dynamically compiled code!"");
                }
            }";



        Console.ReadKey();
    }

    public static object RunCodeByString(string code)
    {
        Type type = null;
        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(code);
        PortableExecutableReference portableExecutableReference = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
        CSharpCompilation cSharpCompilation = CSharpCompilation.Create("CustomAssembly")
            .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
            .AddReferences(portableExecutableReference)
            .AddSyntaxTrees(syntaxTree);
        MemoryStream memoryStream = new MemoryStream();
        EmitResult emitResult = cSharpCompilation.Emit(memoryStream);
        if (emitResult.Success)
        {
            memoryStream.Seek(0, SeekOrigin.Begin);
            Assembly assembly = AssemblyLoadContext.Default.LoadFromStream(memoryStream);
            type = assembly.GetType("YMCLRunner");
        }
        else
        {
            Console.WriteLine("Error");
            foreach (var item in emitResult.Diagnostics)
            {
                Console.WriteLine(item);
            }
            type = null;
        }
        if (type != null)
        {
            object? obj = Activator.CreateInstance(type);
            MethodInfo? methodInfo = type.GetMethod("Main");
            object? result = methodInfo.Invoke(obj, new object[] { });
            return result;
        }
        return null;
    }
}
