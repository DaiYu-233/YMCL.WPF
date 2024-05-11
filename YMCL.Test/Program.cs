using MinecraftLaunch.Components.Analyzer;
using MinecraftLaunch.Components.Fetcher;
using MinecraftLaunch.Components.Installer;
using MinecraftLaunch.Components.Resolver;

public class Program
{
    public static List<Tuple<string, List<string>>> ParseCommandLine(string commandLine)
    {
        var parameters = new List<Tuple<string, List<string>>>();
        var currentMethod = string.Empty;
        var currentArgs = new List<string>();
        var isInQuotes = false;
        var argBuilder = new System.Text.StringBuilder();

        foreach (var c in commandLine)
        {
            if (c == '"')
            {
                isInQuotes = !isInQuotes; // Toggle the quotes state  
                continue; // Skip the quote character  
            }

            if (char.IsWhiteSpace(c) && !isInQuotes)
            {
                // If we're not in quotes and encounter a whitespace, it might be the end of a method or argument  
                if (!string.IsNullOrEmpty(currentMethod))
                {
                    // If we have a method name, add the current args list to the parameters  
                    parameters.Add(Tuple.Create(currentMethod, currentArgs));
                    // Reset for the next method/argument  
                    currentMethod = string.Empty;
                    currentArgs = new List<string>();
                }
                else if (argBuilder.Length > 0)
                {
                    // If we don't have a method name but have built an argument, add it to the current args list  
                    currentArgs.Add(argBuilder.ToString());
                    argBuilder.Clear();
                }
                // Otherwise, do nothing (it's just whitespace)  
            }
            else
            {
                // If we're here, we're either building a method name or an argument  
                if (string.IsNullOrEmpty(currentMethod) && !char.IsWhiteSpace(c))
                {
                    // Start of a new method name  
                    currentMethod = c.ToString();
                }
                else
                {
                    // Building an argument or continuing a method name  
                    argBuilder.Append(c);
                }
            }
        }

        // Add the last method and its arguments (if any)  
        if (!string.IsNullOrEmpty(currentMethod))
        {
            parameters.Add(Tuple.Create(currentMethod, currentArgs));
        }
        else if (argBuilder.Length > 0)
        {
            // If there's a leftover argument without a method name, it's an error or the input is malformed  
            // You can decide how to handle this case  
            throw new ArgumentException("Malformed command line: argument without a method name.");
        }

        return parameters;
    }
    public static async Task Main()
    {
        var commandLine = "--launch '1.20.1-Fabric 0.15.10' --close create 'some value' another_value --ey";
        var parsedParameters = ParseCommandLine(commandLine);

        foreach (var parameter in parsedParameters)
        {
            Console.WriteLine($"Method: {parameter.Item1}, Args: {string.Join(", ", parameter.Item2)}");
        }
    }
}
