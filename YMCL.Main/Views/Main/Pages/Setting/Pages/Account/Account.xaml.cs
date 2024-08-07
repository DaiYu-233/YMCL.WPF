﻿using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using MinecraftLaunch.Classes.Models.Auth;
using MinecraftLaunch.Components.Authenticator;
using Newtonsoft.Json;
using Panuon.WPF.UI;
using YMCL.Main.Public;
using YMCL.Main.Public.Class;
using YMCL.Main.Public.Lang;

namespace YMCL.Main.Views.Main.Pages.Setting.Pages.Account
{
    /// <summary>
    /// Account.xaml 的交互逻辑
    /// </summary>
    public partial class Account : Page
    {
        List<AccountInfo> accounts = JsonConvert.DeserializeObject<List<AccountInfo>>(File.ReadAllText(Const.AccountDataPath));
        public Account()
        {
            InitializeComponent();
            //LoadAccounts();
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
                var skin = Method.BytesToBase64(bytes);
                AccountsListView.Items.Add(new
                {
                    x.Name,
                    x.AccountType,
                    x.AddTime,
                    x.Data,
                    Skin = Method.Base64ToImage(skin)
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
                Toast.Show(message: LangHelper.Current.GetText("Account_OfflineAccountAddBtn_Click_OfflineUserNameNull"), position: ToastPosition.Top, window: Const.Window.main);
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

                File.WriteAllText(Const.AccountDataPath, JsonConvert.SerializeObject(accounts, Formatting.Indented));
                LoadAccounts();
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
            MicrosoftAccount userProfile = new();
            try
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
                userProfile = await authenticator.AuthenticateAsync();
            }
            catch (Exception ex)
            {
                Toast.Show(message: LangHelper.Current.GetText("LoginFail"), position: ToastPosition.Top, window: Const.Window.main);
                return;
            }
            try
            {
                Toast.Show(message: LangHelper.Current.GetText("VerifyingAccount"), position: ToastPosition.Top, window: Const.Window.main);
                MinecraftLaunch.Skin.Class.Fetchers.MicrosoftSkinFetcher skinFetcher = new(userProfile.Uuid.ToString());
                var bytes = await skinFetcher.GetSkinAsync();
                DateTime now = DateTime.Now;
                accounts.Add(new AccountInfo
                {
                    AccountType = SettingItem.AccountType.Microsoft,
                    AddTime = now.ToString("yyyy-MM-ddTHH:mm:sszzz"),
                    Data = JsonConvert.SerializeObject(userProfile, formatting: Formatting.Indented),
                    Name = userProfile.Name,
                    Skin = Method.BytesToBase64(bytes)
                });

                File.WriteAllText(Const.AccountDataPath, JsonConvert.SerializeObject(accounts, Formatting.Indented));
                LoadAccounts();
                LoginMicrosoftDialog.Hide();
                Const.Window.main.Activate();
                AccountsListView.SelectedIndex = AccountsListView.Items.Count - 1;
            }
            catch (Exception)
            {
                Toast.Show(message: LangHelper.Current.GetText("LoginFail"), position: ToastPosition.Top, window: Const.Window.main);
            }
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

        private async void YggdrasilAccountAddBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(YggdrasilServerUrlTextBox.Text))
            {
                Toast.Show(message: LangHelper.Current.GetText("YggdrasilServerUrlIsEmpty"), position: ToastPosition.Top, window: Const.Window.main);
                return;
            }
            if (string.IsNullOrEmpty(YggdrasilEmailTextBox.Text))
            {
                Toast.Show(message: LangHelper.Current.GetText("YggdrasilEmailIsEmpty"), position: ToastPosition.Top, window: Const.Window.main);
                return;
            }
            if (string.IsNullOrEmpty(YggdrasilPasswordTextBox.Password))
            {
                Toast.Show(message: LangHelper.Current.GetText("YggdrasilPasswordIsEmpty"), position: ToastPosition.Top, window: Const.Window.main);
                return;
            }
            IEnumerable<YggdrasilAccount> yggdrasilAccounts = null;
            try
            {
                YggdrasilAuthenticator authenticator = new(YggdrasilServerUrlTextBox.Text, YggdrasilEmailTextBox.Text, YggdrasilPasswordTextBox.Password);
                yggdrasilAccounts = await authenticator.AuthenticateAsync();
            }
            catch (Exception)
            {
                Toast.Show(message: LangHelper.Current.GetText("LoginFail"), position: ToastPosition.Top, window: Const.Window.main);
                return;
            }
            try
            {
                Toast.Show(message: LangHelper.Current.GetText("VerifyingAccount"), position: ToastPosition.Top, window: Const.Window.main);

                foreach (var account in yggdrasilAccounts)
                {
                    DateTime now = DateTime.Now;
                    try
                    {
                        MinecraftLaunch.Skin.Class.Fetchers.YggdrasilSkinFetcher skinFetcher = new(account.YggdrasilServerUrl, account.Uuid.ToString());
                        var bytes = await skinFetcher.GetSkinAsync();
                        await Dispatcher.BeginInvoke(() =>
                        {
                            accounts.Add(new AccountInfo
                            {
                                AccountType = SettingItem.AccountType.ThirdParty,
                                AddTime = now.ToString("yyyy-MM-ddTHH:mm:sszzz"),
                                Data = JsonConvert.SerializeObject(account, formatting: Formatting.Indented),
                                Name = account.Name,
                                Skin = Method.BytesToBase64(bytes)
                            });
                        });
                    }
                    catch
                    {
                        await Dispatcher.BeginInvoke(() =>
                        {
                            accounts.Add(new AccountInfo
                            {
                                AccountType = SettingItem.AccountType.ThirdParty,
                                AddTime = now.ToString("yyyy-MM-ddTHH:mm:sszzz"),
                                Data = JsonConvert.SerializeObject(account, formatting: Formatting.Indented),
                                Name = account.Name
                            });
                        });
                    }
                }

                File.WriteAllText(Const.AccountDataPath, JsonConvert.SerializeObject(accounts, Formatting.Indented));
                LoadAccounts();
                LoginYggdrasilDialog.Hide();
                Const.Window.main.Activate();
                AccountsListView.SelectedIndex = AccountsListView.Items.Count - 1;
            }
            catch (Exception)
            {
                Toast.Show(message: LangHelper.Current.GetText("LoginFail"), position: ToastPosition.Top, window: Const.Window.main);
            }
        }
    }
}
