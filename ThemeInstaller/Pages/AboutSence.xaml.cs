using System;
using System.Reflection;
using System.Windows;

namespace ThemeInstaller.Pages
{
    /// <summary>
    /// 关于页面
    /// </summary>
    public partial class AboutSence
    {
        public AboutSence()
        {
            InitializeComponent();
            MainWindow.ChangeTitle("AboutTitle");
            MainWindow.Instance.ButtonInfo.IsEnabled = false;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            TextboxAbout.Text = string.Format(App.GetText("AboutMessage"),
                                              Setup.Settings["Author"].Content,
                                              Assembly.GetExecutingAssembly().GetName().Version.ToString(3),
                                              Setup.Settings["ThemeVersion"].Content);
        }

        private void BtnOkClick(object sender, RoutedEventArgs e)
        {
            if (NavigationService == null) return;
            MainWindow.Instance.ButtonInfo.IsEnabled = true;
            NavigationService.Navigate(FlowPages.CurrentPage());
        }
    }
}
