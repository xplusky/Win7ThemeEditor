using System.Windows;

namespace ThemeInstaller.Pages
{
	public partial class ReadmeSence
	{
		public ReadmeSence()
		{
			InitializeComponent();
            MainWindow.ChangeTitle("ReadmeTitle");
		}

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //读取txt
            TextboxReadme.Text = Setup.Settings["ReadmeInfo"].Content;
        }

        private void BtnStartInstall_Click(object sender, RoutedEventArgs e)
        {
            FlowPages.GoNextPage(this);
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            FlowPages.GoPrevPage(this);
        }
	}
}