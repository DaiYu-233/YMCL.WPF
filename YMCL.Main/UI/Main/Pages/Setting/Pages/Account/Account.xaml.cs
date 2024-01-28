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
            AccountsListView.Items.Clear();
            var setting = JsonConvert.DeserializeObject<Public.Class.Setting>(File.ReadAllText(Const.SettingDataPath));
            accounts.ForEach(x =>
            {
                AccountsListView.Items.Add(new
                {
                    Name = x.Name,
                    AccountType = x.AccountType,
                    AddTime = x.AddTime,
                    Data = x.Data,
                });
            });

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
                DateTime now = DateTime.Now;
                File.WriteAllText(Const.AccountDataPath, JsonConvert.SerializeObject(new List<AccountInfo>() { new AccountInfo
                {
                    AccountType = SettingItem.AccountType.Offline,
                    AddTime = now.ToString("yyyy-MM-ddTHH:mm:sszzz"),
                    Data = null,
                    Name = "Steve"
                }}, Formatting.Indented));
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
            if (AccountsListView.SelectedIndex == setting.AccountSelectionIndex)
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
            LoadAccounts();
            File.WriteAllText(Const.AccountDataPath, JsonConvert.SerializeObject(accounts, Formatting.Indented));
            AccountsListView.SelectedIndex = 0;
        }

        MicrosoftAuthenticator authenticator;
        DeviceCodeResponse deviceInfo;
        public async void MicrosoftLogin()
        {
            MicrosoftAuthenticator authenticator = new("c06d4d68-7751-4a8a-a2ff-d1b46688f428");
            await authenticator.DeviceFlowAuthAsync(dc =>
            {
                Debug.WriteLine(dc.UserCode);
            });

            var userProfile = await authenticator.AuthenticateAsync();
        }

        private void CopyCodeAndOpenBrowserBtn_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer.exe", deviceInfo.VerificationUrl);
            LoginMicrosoftDialog.Hide();
            System.Windows.Clipboard.SetText(LoginCodeText.Text);
        }
    }

    

}
