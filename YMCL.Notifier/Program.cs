using Microsoft.Toolkit.Uwp.Notifications;

namespace YMCL.Notifier
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string title = string.Empty;
            string msg = string.Empty;
            string logoUri = string.Empty;

            if (args.Length > 0)
            {
                if (args.Length >= 1)
                {
                    title = args[0].Trim('"', '\'');
                }
                if (args.Length >= 2)
                {
                    msg = args[1].Trim('"', '\'');
                }
                if (args.Length >= 3)
                {
                    logoUri = args[2].Trim('"', '\'');
                }
            }
            ToastContentBuilder toastContentBuilder = new ToastContentBuilder();
            toastContentBuilder.AddArgument("action", "viewConversation").AddArgument("conversationId", 9813);
            if (!string.IsNullOrEmpty(title))
            {
                toastContentBuilder.AddText(title);
            }
            if (!string.IsNullOrEmpty(msg))
            {
                toastContentBuilder.AddText(msg);
            }
            if (!string.IsNullOrEmpty(logoUri))
            {
                var uri = new Uri(logoUri);
                toastContentBuilder.AddAppLogoOverride(uri);
            }
            toastContentBuilder.Show();
        }
    }
}
