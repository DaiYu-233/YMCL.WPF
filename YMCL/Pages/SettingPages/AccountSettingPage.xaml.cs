using MinecraftLaunch.Modules.Utilities;
using MinecraftLaunch.Skin.Class.Models;
using MinecraftOAuth.Authenticator;
using Newtonsoft.Json;
using Panuon.WPF.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using MinecraftLaunch.Skin.Class;
using YMCL.Class;
using MinecraftLaunch.Skin.Class.Fetchers;
using MinecraftLaunch.Skin;
using SixLabors.ImageSharp.PixelFormats;
using System.Windows.Media.Imaging;
using System.Security.Cryptography;
using System.Windows.Shapes;

namespace YMCL.Pages.SettingPages
{
    /// <summary>
    /// AccountSettingPage.xaml 的交互逻辑
    /// </summary>
    public partial class AccountSettingPage : System.Windows.Controls.Page
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
            AccountsListView.Items.Clear();
            var i = 0;
            var setting = JsonConvert.DeserializeObject<Class.Setting>(File.ReadAllText(Const.YMCLSettingDataPath));
            accounts = JsonConvert.DeserializeObject<List<AccountInfo>>(File.ReadAllText(Const.YMCLAccountDataPath));
            accounts.ForEach(x =>
            {
                try
                {
                    var head = Base64ToImage(x.Skin);
                    using (var memoryStream = new MemoryStream())
                    {
                        head.Save(memoryStream, head.RawFormat);
                        var bytes = memoryStream.ToArray();
                        using (var ms = new System.IO.MemoryStream(bytes))
                        {
                            Image save = Image.FromStream(ms);
                            save.Save(Const.YMCLTempDataRoot + "\\" + i + ".png");
                        }
                    }
                }
                catch (Exception) { }
                AccountsListView.Items.Add(new
                {
                    Name = x.Name,
                    AccountType = x.AccountType,
                    AddTime = x.AddTime,
                    Data = x.Data,
                    SkinBase64 = Const.YMCLTempDataRoot + "\\" + i + ".png"
                });
                i++;
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
                File.WriteAllText(Const.YMCLAccountDataPath, Const.YMCLDefaultAccount);
            }
        }
        void WriteAccountData()
        {
            var data = JsonConvert.SerializeObject(accounts, Formatting.Indented);
            File.WriteAllText(Const.YMCLAccountDataPath, data);
        }
        private void AccountsListView_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
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
            LoginYggdrasilDialog.Hide();
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
                    LoginYggdrasilDialog.ShowAsync();
                    YggdrasilEmailTextBox.Text = string.Empty;
                    YggdrasilPasswordTextBox.Text = string.Empty;
                    YggdrasilServerUrlTextBox.IsEnabled = true;
                    YggdrasilServerUrlTextBox.Text = string.Empty;
                    isLittleSkin.IsChecked = false;
                    break;
            }
        }

        private void OfflineAccountAddBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(OfflineUserNameTextBox.Text))
            {
                string skin;
                LoginOfflineDialog.Hide();
                accounts.Add(new AccountInfo()
                {
                    AccountType = "离线模式",
                    AddTime = DateTime.Now.ToString(),
                    Name = OfflineUserNameTextBox.Text,
                    Data = null,
                    Skin = BitmapToBase64(Image.FromFile(Const.YMCLAssetsPath + "\\Steve.png"))
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
        #region Base64
        public static string BitmapToBase64(System.Drawing.Image bitmap)
        {
            MemoryStream ms1 = new MemoryStream();
            bitmap.Save(ms1, System.Drawing.Imaging.ImageFormat.Jpeg);
            byte[] arr1 = new byte[ms1.Length];
            ms1.Position = 0;
            ms1.Read(arr1, 0, (int)ms1.Length);
            ms1.Close();
            return Convert.ToBase64String(arr1);
        }
        public static Image Base64ToImage(string base64String)
        {
            // Convert Base64 String to byte[]
            byte[] imageBytes = Convert.FromBase64String(base64String);
            MemoryStream ms = new MemoryStream(imageBytes, 0,
              imageBytes.Length);

            // Convert byte[] to Image
            ms.Write(imageBytes, 0, imageBytes.Length);
            Image image = Image.FromStream(ms, true);
            return image;
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
                MinecraftOAuth.Authenticator.MicrosoftAuthenticator microsoftAuthenticatorRefresh = new(MinecraftOAuth.Module.Enum.AuthType.Refresh)
                {
                    ClientId = Const.YMCLClientId,
                    RefreshToken = refreshToken
                };
                var result = await microsoftAuthenticatorRefresh.AuthAsync(x =>
                {
                    Toast.Show(Const.Window.main, "Refresh：" + x, ToastPosition.Top);
                });
                MicrosoftSkinFetcher microsoftSkinFetcher = new(result.Uuid.ToString());
                var skin = await microsoftSkinFetcher.GetSkinAsync();
                SkinResolver skinResolver = new(skin);
                var head = ImageHelper.ConvertToByteArray(skinResolver.CropSkinHeadBitmap());
                using (var ms = new MemoryStream(head))
                {
                    Image img = Image.FromStream(ms);
                    accounts.Add(new AccountInfo() { Data = result.ToJson(), AccountType = "微软登录", Name = result.Name.ToString(), AddTime = DateTime.Now.ToString(), Skin = BitmapToBase64(img) });
                }
                WriteAccountData();
                LoadAccounts();
                Toast.Show(Const.Window.main, "登录完成", ToastPosition.Top);
            }
            catch (Exception ex)
            {
                MessageBoxX.Show("Refresh登录失败：\n" + ex.Message, "Yu Minecraft Launcher");
            }


        }

        private void isLittleSkin_Click(object sender, RoutedEventArgs e)
        {
            if (isLittleSkin.IsChecked == true)
            {
                YggdrasilServerUrlTextBox.IsEnabled = false;
                YggdrasilServerUrlTextBox.Text = "https://littleskin.cn/api/yggdrasil";
            }
            else
            {
                YggdrasilServerUrlTextBox.IsEnabled = true;
                YggdrasilServerUrlTextBox.Text = string.Empty;
            }

        }

        private async void YggdrasilAccountAddBtn_Click(object sender, RoutedEventArgs e)
        {
            var server = YggdrasilServerUrlTextBox.Text;
            var email = YggdrasilEmailTextBox.Text;
            var password = YggdrasilPasswordTextBox.Text;
            if (string.IsNullOrWhiteSpace(server))
            {
                Toast.Show(Const.Window.main, "验证服务器不可为空", ToastPosition.Top);
                return;
            }
            if (string.IsNullOrWhiteSpace(email))
            {
                Toast.Show(Const.Window.main, "邮箱不可为空", ToastPosition.Top);
                return;
            }
            Regex emailRegex = new Regex("^\\s*([A-Za-z0-9_-]+(\\.\\w+)*@(\\w+\\.)+\\w{1,50})\\s*$");
            if (!emailRegex.IsMatch(email))
            {
                Toast.Show(Const.Window.main, "邮箱格式不正确", ToastPosition.Top);
                return;
            }
            if (string.IsNullOrWhiteSpace(password))
            {
                Toast.Show(Const.Window.main, "密码不可为空", ToastPosition.Top);
                return;
            }
            YggdrasilAuthenticator authenticator = new(server, email, password);
            var result = await authenticator.AuthAsync();

            foreach (var item in result)
            {
                var time = DateTime.Now.ToString();
                SkinResolver skinResolver = new(File.ReadAllBytes(Const.YMCLAssetsPath + "\\Steve.png"));
                var head = ImageHelper.ConvertToByteArray(skinResolver.CropSkinHeadBitmap());
                using (var ms = new MemoryStream(head))
                {
                    Image img = Image.FromStream(ms);
                    accounts.Add(new AccountInfo()
                    {
                        Data = item.ToJson(),
                        AccountType = "第三方登录",
                        Name = item.Name.ToString(),
                        AddTime = time,
                        Skin = BitmapToBase64(img)
                    });
                }
            }
            WriteAccountData();
            LoadAccounts();
            Toast.Show(Const.Window.main, "登录完成", ToastPosition.Top);
            LoginYggdrasilDialog.Hide();
        }
    }
}
