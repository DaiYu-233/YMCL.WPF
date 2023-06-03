using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using WPFTemplateLib.WpfHelpers;
using WpfToast.Controls;

namespace WpfToast
{
    public class MyBindableBase : BindableBase
    {
        #region 气泡弹窗命令

        private RelayCommand<List<object>> _ToastToWindowCmd;
        /// <summary>
        /// 气泡弹窗(到窗体)命令
        /// </summary>
        public RelayCommand<List<object>> ToastToWindowCmd => _ToastToWindowCmd ?? (_ToastToWindowCmd = new RelayCommand<List<object>>(paras =>
        {
            Task.Run(delegate
            {
                string msg = string.Empty;
                if (paras.Any())
                {
                    msg = paras[0] + "";
                }

                Window win = null;
                if (paras.Count >= 2)
                {
                    win = paras[1] as Window;
                }

                Dispatcher.Invoke(delegate
                {
                    Toast.Show(win, msg, new ToastOptions()
                    {
                        Icon = ToastIcons.Information,
                        Location = ToastLocation.OwnerCenter,
                        Time = 5000,
                    });
                });
            });
        }));

        private RelayCommand<string> _ToastToScreenCmd;
        /// <summary>
        /// 气泡弹窗(到屏幕)命令
        /// </summary>
        public RelayCommand<string> ToastToScreenCmd => _ToastToScreenCmd ?? (_ToastToScreenCmd = new RelayCommand<string>(msg =>
        {
            if (string.IsNullOrWhiteSpace(msg))
            {
                return;
            }

            Task.Run(delegate
            {
                Dispatcher.Invoke(delegate
                {
                    Toast.Show(msg, new ToastOptions()
                    {
                        Icon = ToastIcons.Information,
                        Location = ToastLocation.ScreenCenter,
                        Time = 5000,
                    });
                });
            });
        }));

        #endregion
    }
}
