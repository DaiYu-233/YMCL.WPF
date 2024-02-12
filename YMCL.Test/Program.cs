using MinecraftLaunch.Utilities;
using System.Net;

namespace YMCL.Test
{
    internal class Program
    {
        static void Main(string[] args)
        {
            WebRequest request = WebRequest.Create("https://news.bugjump.net/News.xaml");
            WebResponse response = request.GetResponse();
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();
            Console.WriteLine(responseFromServer);
            Console.WriteLine("End");

            Console.ReadKey();
        }
    }
}
