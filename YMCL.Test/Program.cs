namespace YMCL.Test
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            FileInfo file = new FileInfo("D:\\Desktop\\steve.png");
            MinecraftLaunch.Skin.SkinResolver resolver = new(file);
            var bytes = MinecraftLaunch.Skin.ImageHelper.ConvertToByteArray(resolver.CropSkinHeadBitmap());
            string base64 = Convert.ToBase64String(bytes);
            Console.WriteLine(base64);


            Console.ReadKey();
        }
    }
}
