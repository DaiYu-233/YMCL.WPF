using System.Windows;
using YMCL.Main.Public;

namespace YMCL.Main.Views.TaskManage.TaskCenter
{
    /// <summary>
    /// TaskEntry.xaml 的交互逻辑
    /// </summary>
    public partial class TaskEntry : System.Windows.Controls.UserControl
    {
        public TaskEntry(string taskName, bool showProgressBar = false)
        {
            InitializeComponent();
            TaskName.Text = taskName;
            if (showProgressBar)
            {
                ShowProgressBar.Visibility = Visibility.Visible;
            }
        }
        public void UpdateProgress(double progress)
        {
            try
            {
                TaskProgressBar.Value = progress;
                TaskProgressBarText.Content = $"{Math.Round(progress, 2)}%";
            }
            catch { }
        }
        public void UpdateTaskName(string name)
        {
            TaskName.Text = name;
        }
        public void AppendText(string text, bool time = true)
        {
            if (time)
            {
                TaskProgressTextBox.AppendText($"[{DateTime.Now.ToString("HH:mm:ss")}] {text}\n");
            }
            else
            {
                TaskProgressTextBox.AppendText($"{text}\n");
            }
            //TaskProgressTextBox.Focus();
            //TaskProgressTextBox.CaretIndex = TaskProgressTextBox.Text.Length;
            //TaskProgressTextBox.ScrollToEnd();
            if (TaskProgressTextBox.LineCount >= 9)
            {
                TaskProgressTextBox.Text = string.Empty;
                if (time)
                {
                    TaskProgressTextBox.AppendText($"[{DateTime.Now.ToString("HH:mm:ss")}] {text}\n");
                }
                else
                {
                    TaskProgressTextBox.AppendText($"{text}\n");
                }
            }
        }
        public async void Destory()
        {
            await Task.Delay(2000);
            Const.Window.tasks.TaskContainer.Children.Remove(this);
        }
    }
}
