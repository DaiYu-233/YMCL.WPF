using MinecraftLaunch.Components.Fetcher;
using MinecraftLaunch.Components.Installer;

public class Program
{
    public static async Task Main()
    {
        using (var client = new HttpClient())
        {
            try
            {
                var response = await client.GetAsync("https://daiyu.fun/page/YmclComments/");
                response.EnsureSuccessStatusCode();
                string htmlContent = await response.Content.ReadAsStringAsync(); // 读取HTML内容  
                Console.WriteLine(htmlContent); // 输出HTML内容到控制台  
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }
        }

        Console.ReadKey();
    }
}
