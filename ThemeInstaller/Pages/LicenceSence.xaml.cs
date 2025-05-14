using System.Windows;

namespace ThemeInstaller.Pages
{
	public partial class LicenceSence
	{
        

		public LicenceSence()
		{
			InitializeComponent();
		    MainWindow.Instance.TitleText.Text = App.GetText("LincenceTitle");

		}

        private void BtnAgree_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(FlowPages.NextPage());
        }

        private void BtnDisagree_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(FlowPages.PrevPage());
        }

        private void MainPageElement_Loaded(object sender, RoutedEventArgs e)
        {
            TextboxLicense.Text = Setup.Settings["LicenceInfo"].Content;
        }
	}
}