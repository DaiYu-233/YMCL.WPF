using System;
using System.Windows.Input;
/*
 * 源码已托管：https://gitee.com/dlgcy/WPFTemplate
 */
namespace WPFTemplateLib.WpfHelpers
{
    /// <summary>
    /// WPF 命令公共类;
    /// </summary>
    /// <example>
    /// <code>
    /// class Sample
    /// {
    ///     public ICommand DoSomethingCommand { get; set; }
    /// 
    ///     public Sample()
    ///     {
    ///         SetCommandMethod();
    ///     }
    /// 
    ///     /// <summary>
    ///     /// 命令方法赋值（在构造方法中调用）
    ///     /// </summary>
    ///     private void SetCommandMethod()
    ///     {
    ///         DoSomethingCommand ??= new RelayCommand(o => true, async o =>
    ///         {
    ///             // do something
    ///         });
    ///     }
    /// }
    /// </code>
    /// </example>
    public class RelayCommand : ICommand
    {
        private Predicate<object> _canExecute;
        private Action<object> _execute;

        public RelayCommand(Predicate<object> canExecute, Action<object> execute)
        {
            _canExecute = canExecute;
            _execute = execute;
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }
    }

    /// <summary>
    /// WPF 命令公共类（泛型版）
    /// </summary>
    /// <typeparam name="T">命令参数类型</typeparam>
    public class RelayCommand<T> : ICommand
    {
        private Predicate<T> _canExecute;
        private Action<T> _execute;

        public RelayCommand(Action<T> execute) : this(null, execute) { }

        public RelayCommand(Predicate<T> canExecute, Action<T> execute)
        {
            _canExecute = canExecute;
            _execute = execute;
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object parameter)
        {
            if (_canExecute == null)
            {
                return true;
            }
            return _canExecute((T)parameter);
        }

        public void Execute(object parameter)
        {
            _execute((T)parameter);
        }
    }
}
