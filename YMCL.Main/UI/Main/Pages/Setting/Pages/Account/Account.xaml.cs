using Newtonsoft.Json;
using System.Windows;
using System.Windows.Controls;
using YMCL.Main.Public.Class;
using YMCL.Main.Public;
using System.IO;
using Panuon.WPF.UI;
using YMCL.Main.Public.Lang;
using MinecraftLaunch.Components.Authenticator;
using MinecraftLaunch.Classes.Models.Auth;
using System.Diagnostics;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Security.Principal;

namespace YMCL.Main.UI.Main.Pages.Setting.Pages.Account
{
    /// <summary>
    /// Account.xaml 的交互逻辑
    /// </summary>
    public partial class Account : Page
    {
        public class WidthAndHeightToRectConverter : IMultiValueConverter
        {
            public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
            {
                double width = (double)values[0];
                double height = (double)values[1];
                return new Rect(0, 0, width, height);
            }

            public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }


        List<AccountInfo> accounts = JsonConvert.DeserializeObject<List<AccountInfo>>(File.ReadAllText(Const.AccountDataPath));
        public Account()
        {
            InitializeComponent();
            LoadAccounts();

        }

        void LoadAccounts()
        {
            accounts = JsonConvert.DeserializeObject<List<AccountInfo>>(File.ReadAllText(Const.AccountDataPath));
            AccountsListView.Items.Clear();
            var setting = JsonConvert.DeserializeObject<Public.Class.Setting>(File.ReadAllText(Const.SettingDataPath));
            accounts.ForEach(x =>
            {
                MinecraftLaunch.Skin.SkinResolver SkinResolver = new(Convert.FromBase64String(x.Skin));
                var bytes = MinecraftLaunch.Skin.ImageHelper.ConvertToByteArray(SkinResolver.CropSkinHeadBitmap());
                var skin = Function.BytesToBase64(bytes);
                AccountsListView.Items.Add(new
                {
                    x.Name,
                    x.AccountType,
                    x.AddTime,
                    x.Data,
                    Skin = Function.Base64ToImage(skin)
                });
            });

            if (AccountsListView.Items.Count > 0)
            {
                if (setting.AccountSelectionIndex + 1 <= AccountsListView.Items.Count)
                {
                    AccountsListView.SelectedIndex = setting.AccountSelectionIndex;
                }
                else
                {
                    AccountsListView.SelectedItem = AccountsListView.Items[0];
                    setting.AccountSelectionIndex = 0;
                    File.WriteAllText(Const.SettingDataPath, JsonConvert.SerializeObject(setting, Formatting.Indented));
                }
            }
            else
            {
                DateTime now = DateTime.Now;
                File.WriteAllText(Const.AccountDataPath, JsonConvert.SerializeObject(new List<AccountInfo>() { new AccountInfo
                {
                    AccountType = SettingItem.AccountType.Offline,
                    AddTime = now.ToString("yyyy-MM-ddTHH:mm:sszzz"),
                    Data = null,
                    Name = "Steve"
                }}, Formatting.Indented));
                setting.AccountSelectionIndex = 0;
                File.WriteAllText(Const.SettingDataPath, JsonConvert.SerializeObject(setting, Formatting.Indented));
                LoadAccounts();
            }
            if (setting.AccountSelectionIndex == -1 && accounts.Count > 0)
            {
                setting.AccountSelectionIndex = 0;
                File.WriteAllText(Const.SettingDataPath, JsonConvert.SerializeObject(setting, Formatting.Indented));
                LoadAccounts();
            }
        }

        private void AddAcountButton_Click(object sender, RoutedEventArgs e)
        {
            LoginTypeComboBox.SelectedIndex = 0;
            LoginTypeSelectionDialog.ShowAsync();
        }

        private void AccountsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var setting = JsonConvert.DeserializeObject<Public.Class.Setting>(File.ReadAllText(Const.SettingDataPath));
            if (AccountsListView.SelectedIndex >= 0)
            {
                DelAccountButton.IsEnabled = true;
            }
            if (AccountsListView.SelectedIndex == setting.AccountSelectionIndex || AccountsListView.SelectedIndex == -1)
            {
                return;
            }
            setting.AccountSelectionIndex = AccountsListView.SelectedIndex;
            File.WriteAllText(Const.SettingDataPath, JsonConvert.SerializeObject(setting, Formatting.Indented));
        }

        private void Cancel_Login(object sender, RoutedEventArgs e)
        {
            LoginTypeSelectionDialog.Hide();
            LoginMicrosoftDialog.Hide();
            LoginOfflineDialog.Hide();
            LoginYggdrasilDialog.Hide();
        }

        private void NextStepButton_Click(object sender, RoutedEventArgs e)
        {
            LoginTypeSelectionDialog.Hide();
            if (LoginTypeComboBox.SelectedIndex == 0)
            {
                OfflineUserNameTextBox.Text = string.Empty;
                LoginOfflineDialog.ShowAsync();
            }
            if (LoginTypeComboBox.SelectedIndex == 1)
            {
                LoginMicrosoftDialog.ShowAsync();
                MicrosoftLogin();
            }
            if (LoginTypeComboBox.SelectedIndex == 2)
            {
                LoginYggdrasilDialog.ShowAsync();
            }
        }

        private void OfflineAccountAddBtn_Click(object sender, RoutedEventArgs e)
        {
            if (OfflineUserNameTextBox.Text == string.Empty)
            {
                Toast.Show(message: LangHelper.Current.GetText("Account_OfflineAccountAddBtn_Click_OfflineUserNameNull"), position: ToastPosition.Top, window: Const.Window.mainWindow);
                OfflineUserNameTextBox.Focus();
            }
            else
            {
                DateTime now = DateTime.Now;
                accounts.Add(new AccountInfo
                {
                    AccountType = SettingItem.AccountType.Offline,
                    AddTime = now.ToString("yyyy-MM-ddTHH:mm:sszzz"),
                    Data = null,
                    Name = OfflineUserNameTextBox.Text
                });
                LoadAccounts();
                File.WriteAllText(Const.AccountDataPath, JsonConvert.SerializeObject(accounts, Formatting.Indented));
                LoginOfflineDialog.Hide();
            }
        }

        private void DelAccountButton_Click(object sender, RoutedEventArgs e)
        {
            accounts.RemoveAt(AccountsListView.SelectedIndex);
            if (accounts.Count == 0)
            {
                accounts.Add(new AccountInfo()
                {
                    AccountType = SettingItem.AccountType.Offline,
                    AddTime = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"),
                    Name = "Steve",
                    Data = null,
                });
            }
            File.WriteAllText(Const.AccountDataPath, JsonConvert.SerializeObject(accounts, Formatting.Indented));
            LoadAccounts();
            AccountsListView.SelectedIndex = 0;
        }

        string verificationUrl;
        public async void MicrosoftLogin()
        {
            LoginCodeText.Text = LangHelper.Current.GetText("Account_Loading");
            verificationUrl = string.Empty;
            CopyCodeAndOpenBrowserBtn.IsEnabled = false;
            MicrosoftAuthenticator authenticator = new(Const.AzureClientId, true);
            await authenticator.DeviceFlowAuthAsync(device =>
            {
                LoginCodeText.Text = device.UserCode;
                verificationUrl = device.VerificationUrl;
                CopyCodeAndOpenBrowserBtn.IsEnabled = true;
            });

            var userProfile = await authenticator.AuthenticateAsync();
            Toast.Show(message: LangHelper.Current.GetText("VerifyingAccount"), position: ToastPosition.Top, window: Const.Window.mainWindow);

            MinecraftLaunch.Skin.Class.Fetchers.MicrosoftSkinFetcher skinFetcher = new(userProfile.Uuid.ToString());
            var bytes = await skinFetcher.GetSkinAsync();
            

            DateTime now = DateTime.Now;
            accounts.Add(new AccountInfo
            {
                AccountType = SettingItem.AccountType.Microsoft,
                AddTime = now.ToString("yyyy-MM-ddTHH:mm:sszzz"),
                Data = JsonConvert.SerializeObject(userProfile, formatting: Formatting.Indented),
                Name = userProfile.Name,
                Skin = Function.BytesToBase64(bytes)
            });

            File.WriteAllText(Const.AccountDataPath, JsonConvert.SerializeObject(accounts, Formatting.Indented));
            LoadAccounts();
            LoginMicrosoftDialog.Hide();
            Const.Window.mainWindow.Activate();
            AccountsListView.SelectedIndex = AccountsListView.Items.Count - 1;
        }

        private void CopyCodeAndOpenBrowserBtn_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer.exe", verificationUrl);
            LoginMicrosoftDialog.Hide();
            System.Windows.Clipboard.SetText(LoginCodeText.Text);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadAccounts();
        }
    }
}
