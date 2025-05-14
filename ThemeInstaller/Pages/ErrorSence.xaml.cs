using System.Windows;
using System.Windows.Controls;

namespace ThemeInstaller.Pages
{
    /// <summary>
    /// 错误页面
    /// </summary>
    public partial class ErrorSence
    {
        public static string ErrorInfo { get; set; }
        public ErrorSence()
        {
            InitializeComponent();
            MainWindow.Instance.TitleText.Text = App.GetText("ErrorTitle");
            TextboxInfo.Text = ErrorInfo;
            BtnExit.Click += BtnExit_Click;
        }

        void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.EndApp();
        }
    }
}
