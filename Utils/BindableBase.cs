using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Threading;

/*
 * 源码已托管：https://gitee.com/dlgcy/WPFTemplateLib
 */
namespace WPFTemplateLib.WpfHelpers
{
    /// <summary>
    /// WPF绑定属性基类;
    /// </summary>
    /// <example>
    /// <code>
    /// class Sample : BindableBase
    /// {
    ///     private List&lt;string&gt; _stuList;
    ///     public List&lt;string&gt; StuList
    ///     {
    ///         get => _stuList;
    ///         set => SetProperty(ref _stuList, value);
    ///     }
    /// }
    /// </code>
    /// </example>
    public abstract class BindableBase : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        public Dispatcher Dispatcher => Application.Current?.Dispatcher;

        #region BindableBase

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 属性变动通知
        /// </summary>
        /// <param name="propertyName"></param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value)) return false;

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        protected bool SetPropertyWithoutCompare<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        #endregion

        #region 验证

        /// <summary>
        /// 错误列表
        /// </summary>
        private Dictionary<string, List<string>> _Errors = new Dictionary<string, List<string>>();

        private void SetErrors(string propertyName, List<string> propertyErrors)
        {
            //clear any errors that already exist for this property.
            _Errors.Remove(propertyName);

            //Add the list collection for the specified property.
            _Errors.Add(propertyName, propertyErrors);

            //Raise the error-notification event.
            if (ErrorsChanged != null)
                ErrorsChanged(this, new DataErrorsChangedEventArgs(propertyName));
        }

        private void ClearErrors(string propertyName)
        {
            //Remove the error list for this property.
            _Errors.Remove(propertyName);

            //Raise the error-notification event.
            if (ErrorsChanged != null)
                ErrorsChanged(this, new DataErrorsChangedEventArgs(propertyName));
        }

        /// <summary>
        /// 是否包含错误
        /// </summary>
        /// <param name="propertyName">属性名</param>
        public bool IsContainErrors(string propertyName)
        {
            return (GetErrors(propertyName) as List<string>)?.Count > 0;
        }

        /// <summary>
        /// 是否包含错误
        /// </summary>
        /// <param name="propertyName">属性名列表</param>
        public bool IsContainErrors(List<string> propertyNameList)
        {
            return propertyNameList.Exists(x => IsContainErrors(x));
        }

        /// <summary>
        /// 获取给定属性列表的错误列表（参数传空则获取所有错误列表）
        /// </summary>
        /// <param name="propertyName">属性名列表</param>
        /// <returns>错误列表（List＜string＞）</returns>
        public List<string> GetErrors(List<string> propertyNameList)
        {
            if (!propertyNameList?.Any() ?? true)
            {
                return _Errors.Values.SelectMany(x => x).ToList();
            }
            else
            {
                List<string> errors = new List<string>();
                foreach (string propertyName in propertyNameList)
                {
                    if (_Errors.ContainsKey(propertyName))
                    {
                        errors.AddRange(_Errors[propertyName]);
                    }
                }

                return errors;
            }
        }

        #region INotifyDataErrorInfo

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        /// <summary>
        /// 获取属性错误列表（属性名传空则获取所有错误列表）
        /// </summary>
        /// <param name="propertyName">属性名</param>
        /// <returns>错误列表(List＜List＜string＞＞)</returns>
        public IEnumerable GetErrors(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                //Provide all the error collections.
                return _Errors.Values;
            }
            else
            {
                //Provice the error collection for the requested property (if it has errors).
                if (_Errors.ContainsKey(propertyName))
                {
                    return _Errors[propertyName];
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 整个类是否存在错误
        /// </summary>
        public bool HasErrors => _Errors.Count > 0;

        #endregion

        #region 实用方法(供参考)

        /// <summary>
        /// 验证是否为空
        /// </summary>
        /// <returns>true-不为空，false-为空</returns>
        public virtual bool ValidateBlank(object value, string errMsg = "", [CallerMemberName] string propertyName = null)
        {
            bool valid = !string.IsNullOrWhiteSpace(value + "");
            if (!valid) //为空；
            {
                if (string.IsNullOrWhiteSpace(errMsg))
                {
                    errMsg = $"[{propertyName}] can't be blank";
                }

                SetErrors(propertyName, new List<string>() { errMsg });
            }
            else
            {
                ClearErrors(propertyName);
            }

            return valid;
        }

        #endregion

        #endregion
    }
}
