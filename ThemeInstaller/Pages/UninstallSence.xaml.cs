using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ThemeInstaller.Pages
{
    /// <summary>
    /// UninstallSence.xaml 的交互逻辑
    /// </summary>
    public partial class UninstallSence
    {
        public UninstallSence()
        {
            InitializeComponent();
            MainWindow.Instance.TitleText.Text = App.GetText("UninstallTitle");
        }


        private void BtnBackClick(object sender, RoutedEventArgs e)
        {
            FlowPages.GoPrevPage(this);
        }

        private void BtnNextClick(object sender, RoutedEventArgs e)
        {
            //if(IsFileInUse(Paths.))
            //{
            //    if (MessageBox.Show(App.FindString("UninstallMssInUseMes"), "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            //        if (NavigationService != null) NavigationService.Navigate(FlowPages.NextPage());
            //}
            //else
            //{
            //    if (NavigationService != null) NavigationService.Navigate(FlowPages.NextPage());
            //}
            if (NavigationService != null) NavigationService.Navigate(FlowPages.NextPage());
        }

        public static bool IsFileInUse(string fileName)
        {
            try
            {
                var inUse = true;
                FileStream fs = null;
                try
                {
                    fs = new FileStream(fileName, FileMode.Open, FileAccess.Read,
                        FileShare.None);
                    inUse = false;
                }
                catch
                {
                    Console.WriteLine("IsFileInUse Detect Error");
                }
                finally
                {
                    if (fs != null)
                        fs.Close();
                }
                return inUse; //true表示正在使用,false没有使用 
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return false;
        } 
    }
}
