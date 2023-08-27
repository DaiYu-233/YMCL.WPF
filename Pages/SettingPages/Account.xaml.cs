using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using MinecraftLaunch.Modules.Utils;
using ModernWpf.Controls;
using Newtonsoft.Json;
using Panuon.WPF.UI;
using YMCL.Class;
using static YMCL.Class.NeteasyCloudMusic;
using Page = System.Windows.Controls.Page;

namespace YMCL.Pages.SettingPages
{
    /// <summary>
    /// Account.xaml 的交互逻辑
    /// </summary>
    public partial class Account : Page
    {
        public class AccountsList
        {
            public string? Name;
            public string? AccountType;
            public string? AddTime;

        }
        public int indexAnd;
        string loginUrl;
        public List<AccountInfo> accountInfos = new List<AccountInfo>();
        List<AccountsList> accounts = new List<AccountsList>();

        public Account()
        {
            InitializeComponent();

            LoginTypeComboBox.SelectedItem = LoginTypeComboBox.Items[0];

            var obj = JsonConvert.DeserializeObject<SettingInfo>(File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\YMCL\\YMCL.Setting.json"));

            datagrid();
            if ((accountInfos.Count) == 0)
            {
                System.IO.File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\YMCL\\YMCL.Account.json", "[{\"AccountType\": \"离线登录\",\"Data\": \"Null\",\"Name\": \"Steve\",\"AddTime\": \"Null\"}]");
                string tstr = System.IO.File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\YMCL\\YMCL.Account.json");
                accountInfos = JsonConvert.DeserializeObject<List<AccountInfo>>(tstr);
            }
            AccountsListView.SelectedItem = AccountsListView.Items[Convert.ToInt32(obj.LoginIndex)];



            System.IO.File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\YMCL\Temp\LoginName.log", accountInfos[AccountsListView.SelectedIndex].Name.ToString());
            System.IO.File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\YMCL\Temp\LoginType.log", accountInfos[AccountsListView.SelectedIndex].AccountType.ToString());

        }

        private async void MicrosoftLogin()
        {
            CopyCodeAndOpenBrowserBtn.IsEnabled = false;
            string refreshToken = string.Empty;
            loginMicrosoftDialog.ShowAsync();

            try
            {
                LoginCodeText.Text = "加载中...";
                MinecraftOAuth.Authenticator.MicrosoftAuthenticator microsoftAuthenticator = new(MinecraftOAuth.Module.Enum.AuthType.Access)
                {
                    ClientId = "c06d4d68-7751-4a8a-a2ff-d1b46688f428"
                };
                var deviceInfo = await microsoftAuthenticator.GetDeviceInfo();
                //Clipboard.SetText(deviceInfo.UserCode);
                LoginCodeText.Text = deviceInfo.UserCode;
                loginUrl = deviceInfo.VerificationUrl;
                CopyCodeAndOpenBrowserBtn.IsEnabled = true;
                var token = await microsoftAuthenticator.GetTokenResponse(deviceInfo);
                var userProfile = await microsoftAuthenticator.AuthAsync(x =>
                {
                    Panuon.WPF.UI.Toast.Show(Global.form_main, "Access：" + x, ToastPosition.Top);
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
                    Panuon.WPF.UI.Toast.Show(Global.form_main, "Refresh：" + x, ToastPosition.Top);
                });

                //MessageBoxX.Show(result.ToJson(), "账户Json信息");
                //System.IO.File.WriteAllText("./YMCL/Accounts/Microsoft-" + result.Name + ".json", );
                accountInfos.Add(new AccountInfo() { Data = result.ToJson(), AccountType = "微软登录", Name = result.Name.ToString(), AddTime = DateTime.Now.ToString() });
                WriteFile();

                datagrid();
                Panuon.WPF.UI.Toast.Show(Global.form_main, "登录完成", ToastPosition.Top);
            }
            catch (Exception ex)
            {
                MessageBoxX.Show("Refresh登录失败：\n" + ex.Message, "Yu Minecraft Launcher");
            }


        }












        private void datagrid()
        {
            int a = 0;
            if (!System.IO.File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\YMCL\\YMCL.Account.json"))
            {
                System.IO.File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\YMCL\\YMCL.Account.json", "[{\"AccountType\": \"离线登录\",\"Data\": \"Null\",\"Name\": \"Steve\",\"AddTime\": \"Null\"}]");
            }
            string str = System.IO.File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\YMCL\\YMCL.Account.json");
            dynamic model = JsonConvert.DeserializeObject<List<AccountInfo>>(str);
            accountInfos = JsonConvert.DeserializeObject<List<AccountInfo>>(str);
            AccountsListView.ItemsSource = accountInfos;
            DataGr.ItemsSource = model;
            AccountsListView.SelectedItem = AccountsListView.Items[0];
        }
        private void AddAcount_Click(object sender, RoutedEventArgs e)
        {
            loginTypeSelectionDialog.ShowAsync();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (LoginTypeComboBox.SelectedItem.ToString() == "System.Windows.Controls.ComboBoxItem: 离线登录")
            {
                offlineUserNameTextBox.Text = string.Empty;
                loginTypeSelectionDialog.Hide();
                loginOfflineDialog.ShowAsync();
            }
            else if (LoginTypeComboBox.SelectedItem.ToString() == "System.Windows.Controls.ComboBoxItem: Mojang迁移")
            {
                loginTypeSelectionDialog.Hide();
                Process.Start("explorer.exe", "https://www.minecraft.net/zh-hans/account-security");
            }
            else if (LoginTypeComboBox.SelectedItem.ToString() == "System.Windows.Controls.ComboBoxItem: 微软登录")
            {
                loginTypeSelectionDialog.Hide();
                MicrosoftLogin();
            }
            else if (LoginTypeComboBox.SelectedItem.ToString() == "System.Windows.Controls.ComboBoxItem: 第三方登录")
            {
                loginTypeSelectionDialog.Hide();
                loginYggdrasilDialog.ShowAsync();
                isLittleSkin.IsChecked = false;
                YggdrasilServerUrlTextBox.IsEnabled = true;
                YggdrasilServerUrlTextBox.Text = "";
                YggdrasilEmailTextBox.Text = "";
                YggdrasilPasswordTextBox.Text = "";

            }
            else
            {
                Panuon.WPF.UI.Toast.Show(Global.form_main, "请选择登录方式", ToastPosition.Top);
                //MessageBoxX.Show("请选择登录方式");
            }

        }

        private void WriteFile()
        {
            string str = JsonConvert.SerializeObject(accountInfos, Newtonsoft.Json.Formatting.Indented);
            System.IO.File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\YMCL\\YMCL.Account.json", str);
        }



        private void DataGr_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGr.SelectedIndex >= 0)
            {

                Del.IsEnabled = true;
                NowLoginType.Text = accountInfos[DataGr.SelectedIndex].AccountType.ToString() + " - " + accountInfos[DataGr.SelectedIndex].Name.ToString();
                System.IO.File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\YMCL\Temp\LoginName.log", accountInfos[DataGr.SelectedIndex].Name.ToString());
                System.IO.File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\YMCL\Temp\LoginType.log", accountInfos[DataGr.SelectedIndex].AccountType.ToString());

                var obj = JsonConvert.DeserializeObject<SettingInfo>(File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\YMCL\\YMCL.Setting.json"));
                obj.LoginIndex = DataGr.SelectedIndex.ToString();
                obj.LoginName = accountInfos[DataGr.SelectedIndex].Name.ToString();
                obj.LoginType = accountInfos[DataGr.SelectedIndex].AccountType.ToString();
                System.IO.File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\YMCL\\YMCL.Setting.json", JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented));

            }
            else
            {
                if ((accountInfos.Count) == 0)
                {
                    System.IO.File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\YMCL\\YMCL.Account.json", "[{\"AccountType\": \"离线登录\",\"Data\": \"Null\",\"Name\": \"Steve\",\"AddTime\": \"Null\"}]");
                    string tstr = System.IO.File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\YMCL\\YMCL.Account.json");
                    accountInfos = JsonConvert.DeserializeObject<List<AccountInfo>>(tstr);
                }
                AccountsListView.SelectedItem = DataGr.Items[0];
            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            accountInfos.RemoveAt(AccountsListView.SelectedIndex);
            if ((accountInfos.Count) == 0)
            {
                System.IO.File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\YMCL\\YMCL.Account.json", "[{\"AccountType\": \"离线登录\",\"Data\": \"Null\",\"Name\": \"Steve\",\"AddTime\": \"Null\"}]");
                string tstr = System.IO.File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\YMCL\\YMCL.Account.json");
                accountInfos = JsonConvert.DeserializeObject<List<AccountInfo>>(tstr);
            }

            WriteFile();
            datagrid();
            AccountsListView.SelectedItem = AccountsListView.Items[0];
        }



        private void AccountsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AccountsListView.SelectedIndex >= 0)
            {

                Del.IsEnabled = true;
                NowLoginType.Text = accountInfos[AccountsListView.SelectedIndex].AccountType.ToString() + " - " + accountInfos[AccountsListView.SelectedIndex].Name.ToString();
                System.IO.File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\YMCL\Temp\LoginName.log", accountInfos[AccountsListView.SelectedIndex].Name.ToString());
                System.IO.File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\YMCL\Temp\LoginType.log", accountInfos[AccountsListView.SelectedIndex].AccountType.ToString());

                var obj = JsonConvert.DeserializeObject<SettingInfo>(File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\YMCL\\YMCL.Setting.json"));
                obj.LoginIndex = AccountsListView.SelectedIndex.ToString();
                obj.LoginName = accountInfos[AccountsListView.SelectedIndex].Name.ToString();
                obj.LoginType = accountInfos[AccountsListView.SelectedIndex].AccountType.ToString();
                System.IO.File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\YMCL\\YMCL.Setting.json", JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented));

            }
            else
            {
                if ((accountInfos.Count) == 0)
                {
                    System.IO.File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\YMCL\\YMCL.Account.json", "[{\"AccountType\": \"离线登录\",\"Data\": \"Null\",\"Name\": \"Steve\",\"AddTime\": \"Null\"}]");
                    string tstr = System.IO.File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\YMCL\\YMCL.Account.json");
                    accountInfos = JsonConvert.DeserializeObject<List<AccountInfo>>(tstr);
                }
                AccountsListView.SelectedItem = DataGr.Items[0];
            }
        }

        private void CancelAddAccountBtn_Click(object sender, RoutedEventArgs e)
        {
            loginTypeSelectionDialog.Hide();
            loginOfflineDialog.Hide();
            loginMicrosoftDialog.Hide();
            loginYggdrasilDialog.Hide();
        }

        private void offlineAccountAddBtn_Click(object sender, RoutedEventArgs e)
        {
            if (offlineUserNameTextBox.Text != string.Empty)
            {
                accountInfos.Add(new AccountInfo() { AccountType = "离线登录", Name = offlineUserNameTextBox.Text, AddTime = DateTime.Now.ToString() });
                WriteFile();
                datagrid();
                loginOfflineDialog.Hide();
            }
            else
            {
                Panuon.WPF.UI.Toast.Show(Global.form_main, "用户名不可为空", ToastPosition.Top);
                //MessageBoxX.Show("用户名为空！");
            }

        }

        private void CopyCodeAndOpenBrowserBtn_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", loginUrl);
            loginMicrosoftDialog.Hide();
        }

        private async void YggdrasilAccountAddBtn_Click(object sender, RoutedEventArgs e)
        {
            if (YggdrasilServerUrlTextBox.Text == string.Empty)
            {
                Panuon.WPF.UI.Toast.Show(Global.form_main, "验证服务器不可为空", ToastPosition.Top);
                return;
            }
            if (YggdrasilEmailTextBox.Text == string.Empty)
            {
                Panuon.WPF.UI.Toast.Show(Global.form_main, "邮箱不可为空", ToastPosition.Top);
                return;
            }
            if (YggdrasilPasswordTextBox.Text == string.Empty)
            {
                Panuon.WPF.UI.Toast.Show(Global.form_main, "密码不可为空", ToastPosition.Top);
                return;
            }

            try
            {
                if (isLittleSkin.IsChecked == true)
                {
                    MinecraftOAuth.Authenticator.YggdrasilAuthenticator authenticator
                        = new(true, YggdrasilEmailTextBox.Text, YggdrasilPasswordTextBox.Text);
                    var result = await authenticator.AuthAsync();
                    var json = result.ToJson();
                    var list = JsonConvert.DeserializeObject<List<YggdrasilAccount>>(json);
                    list.ForEach(x =>
                    {
                        accountInfos.Add(new AccountInfo()
                        {
                            Data = json,
                            AccountType = "第三方登录",
                            Name = x.Name,
                            AddTime = DateTime.Now.ToString()
                        });
                    });
                }
                else
                {
                    MinecraftOAuth.Authenticator.YggdrasilAuthenticator authenticator
                        = new(YggdrasilServerUrlTextBox.Text, YggdrasilEmailTextBox.Text, YggdrasilPasswordTextBox.Text);
                    var result = await authenticator.AuthAsync();
                    var json = result.ToJson();
                    var list = JsonConvert.DeserializeObject<List<YggdrasilAccount>>(json);
                    list.ForEach(x =>
                    {
                        accountInfos.Add(new AccountInfo()
                        {
                            Data = json,
                            AccountType = "第三方登录",
                            Name = x.Name,
                            AddTime = DateTime.Now.ToString()
                        });
                    });
                }
                WriteFile();
                datagrid();
                loginYggdrasilDialog.Hide();
                Panuon.WPF.UI.Toast.Show(Global.form_main, "登录完成", ToastPosition.Top);

            }
            catch (Exception ex)
            {
                Panuon.WPF.UI.Toast.Show(Global.form_main, "登录失败："+ex.Message, ToastPosition.Top);
                //MessageBoxX.Show(ex.Message);
            }


            //result.ToJson();
        }

        private void isLittleSkin_Click(object sender, RoutedEventArgs e)
        {
            if (isLittleSkin.IsChecked == true)
            {
                YggdrasilServerUrlTextBox.IsEnabled = false;
                YggdrasilServerUrlTextBox.Text = "{LittleSkin}";
            }
            else
            {
                YggdrasilServerUrlTextBox.IsEnabled = true;
                YggdrasilServerUrlTextBox.Text = "";
            }
        }
    }
}
