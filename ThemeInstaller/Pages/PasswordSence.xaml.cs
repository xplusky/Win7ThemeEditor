using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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
    /// PasswordSence.xaml 的交互逻辑
    /// </summary>
    public partial class PasswordSence
    {
        public PasswordSence()
        {
            InitializeComponent();
            MainWindow.Instance.TitleText.Text = Setup.Settings["Title"].Content;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if(TextboxPassword.Text.Trim()!=Setup.Settings["Password"].Content)
            {
                MessageBox.Show(App.GetText("PasswordNotRight"));
            }
            else
            {
                NavigationService.Navigate(FlowPages.NextPage());
            }
        }

        public static string GetMd5(string myString)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            var fromData = Encoding.Unicode.GetBytes(myString);
            var targetData = md5.ComputeHash(fromData);
            string byte2String = null;

            for (int i = 0; i < targetData.Length; i++)
            {
                byte2String += targetData[i].ToString("x");
            }

            return byte2String;
        }
    }
}
