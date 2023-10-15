using MinecraftLaunch.Modules.Models.Auth;
using MinecraftLaunch.Modules.Utils;
using MinecraftOAuth.Module.Models;
using Newtonsoft.Json;
using Panuon.WPF.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using YMCL.Class;

namespace YMCL.Pages.SettingPages
{
    /// <summary>
    /// AccountSettingPage.xaml 的交互逻辑
    /// </summary>
    public partial class AccountSettingPage : Page
    {
        public AccountSettingPage()
        {
            InitializeComponent();
            LoadAccounts();
        }

        #region AccountsUI
        string loginUrl;
        List<AccountInfo> accounts = new List<AccountInfo>();
        void LoadAccounts()
        {
            var setting = JsonConvert.DeserializeObject<Class.Setting>(File.ReadAllText(Const.YMCLSettingDataPath));
            accounts = JsonConvert.DeserializeObject<List<AccountInfo>>(File.ReadAllText(Const.YMCLAccountDataPath));
            AccountsListView.ItemsSource = accounts;
            if (AccountsListView.Items.Count > 0)
            {
                if (setting.AccountSelectionIndex <= AccountsListView.Items.Count)
                {
                    AccountsListView.SelectedIndex = setting.AccountSelectionIndex;
                }
                else
                {
                    AccountsListView.SelectedItem = AccountsListView.Items[0];
                }
            }
            else
            {
                File.WriteAllText(Const.YMCLAccountDataPath, Const.YMCLDefaultAccount);
            }
        }
        void WriteAccountData()
        {
            var data = JsonConvert.SerializeObject(accounts, Formatting.Indented);
            File.WriteAllText(Const.YMCLAccountDataPath, data);
        }
        private void AccountsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var setting = JsonConvert.DeserializeObject<Setting>(File.ReadAllText(Const.YMCLSettingDataPath));
            setting.AccountSelectionIndex = AccountsListView.SelectedIndex;
            File.WriteAllText(Const.YMCLSettingDataPath, JsonConvert.SerializeObject(setting, Formatting.Indented));
            if (AccountsListView.SelectedIndex >= 0)
            {
                DelAccountButton.IsEnabled = true;
            }
            else DelAccountButton.IsEnabled = false;
            if (AccountsListView.SelectedIndex >= 0)
            {
                var data = JsonConvert.SerializeObject(accounts[AccountsListView.SelectedIndex], Formatting.Indented);
                File.WriteAllText(Const.YMCLTempAccountDataPath, data);
            }

        }
        private void DelAccountButton_Click(object sender, RoutedEventArgs e)
        {
            accounts.RemoveAt(AccountsListView.SelectedIndex);
            WriteAccountData();
            LoadAccounts();
            if (AccountsListView.Items.Count == 0)
            {
                File.WriteAllText(Const.YMCLAccountDataPath, Const.YMCLDefaultAccount);
                LoadAccounts();
            }
            AccountsListView.SelectedIndex = 0;
        }

        private void AddAcountButton_Click(object sender, RoutedEventArgs e)
        {
            LoginTypeSelectionDialog.ShowAsync();
        }
        private void CancelAddAccountBtn_Click(object sender, RoutedEventArgs e)
        {
            LoginTypeSelectionDialog.Hide();
            LoginOfflineDialog.Hide();
            LoginMicrosoftDialog.Hide();
            LoginTypeSelectionDialog.Hide();
        }

        private void NextStepButton_Click(object sender, RoutedEventArgs e)
        {
            switch (LoginTypeComboBox.SelectedIndex)
            {
                case 0:
                    OfflineUserNameTextBox.Text = string.Empty;
                    LoginTypeSelectionDialog.Hide();
                    LoginOfflineDialog.ShowAsync();
                    break;
                case 1:
                    LoginTypeSelectionDialog.Hide();
                    MicrosoftLogin();
                    break;
                case 2:
                    LoginTypeSelectionDialog.Hide();
                    break;
            }
        }

        private void OfflineAccountAddBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(OfflineUserNameTextBox.Text))
            {
                LoginOfflineDialog.Hide();
                accounts.Add(new AccountInfo()
                {
                    AccountType = "离线模式",
                    AddTime = DateTime.Now.ToString(),
                    Name = OfflineUserNameTextBox.Text,
                    Data = null
                });
                WriteAccountData();
                LoadAccounts();
            }
            else
            {
                Toast.Show(Const.Window.main, "用户名不可为空", ToastPosition.Top);
            }
        }
        private void CopyCodeAndOpenBrowserBtn_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer.exe", loginUrl);
            LoginMicrosoftDialog.Hide();
            Clipboard.SetText(LoginCodeText.Text);
        }

        #endregion



        private async void MicrosoftLogin()
        {
            CopyCodeAndOpenBrowserBtn.IsEnabled = false;
            string refreshToken = string.Empty;
            _ = LoginMicrosoftDialog.ShowAsync();

            try
            {
                LoginCodeText.Text = "加载中...";
                MinecraftOAuth.Authenticator.MicrosoftAuthenticator microsoftAuthenticator = new(MinecraftOAuth.Module.Enum.AuthType.Access)
                {
                    ClientId = Const.YMCLClientId
                };
                var deviceInfo = await microsoftAuthenticator.GetDeviceInfo();
                LoginCodeText.Text = deviceInfo.UserCode;
                loginUrl = deviceInfo.VerificationUrl;
                CopyCodeAndOpenBrowserBtn.IsEnabled = true;
                var token = await microsoftAuthenticator.GetTokenResponse(deviceInfo);
                var userProfile = await microsoftAuthenticator.AuthAsync(x =>
                {
                    Toast.Show(Const.Window.main, "Access：" + x, ToastPosition.Top);
                });
                refreshToken = userProfile.RefreshToken;
            }
            catch (Exception ex)
            {
                MessageBoxX.Show("Access登录失败：\n" + ex.Message, "Yu Minecraft Launcher");
                return;
            }

            try
            {
                MinecraftOAuth.Authenticator.MicrosoftAuthenticator microsoftAuthenticator = new(MinecraftOAuth.Module.Enum.AuthType.Refresh)
                {
                    ClientId = "c06d4d68-7751-4a8a-a2ff-d1b46688f428",
                    RefreshToken = refreshToken
                };
                var result = await microsoftAuthenticator.AuthAsync(x =>
                {
                    Toast.Show(Const.Window.main, "Refresh：" + x, ToastPosition.Top);
                });

                accounts.Add(new AccountInfo() { Data = result.ToJson(), AccountType = "微软登录", Name = result.Name.ToString(), AddTime = DateTime.Now.ToString() });
                WriteAccountData();
                LoadAccounts();
                Toast.Show(Const.Window.main, "登录完成", ToastPosition.Top);
            }
            catch (Exception ex)
            {
                MessageBoxX.Show("Refresh登录失败：\n" + ex.Message, "Yu Minecraft Launcher");
            }


        }


    }
}
