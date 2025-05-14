using System;
using System.Collections.Generic;
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
    /// NotSupportSence.xaml 的交互逻辑
    /// </summary>
    public partial class NotSupportSence
    {
        public static List<string> Infos = new List<string>(); 

        public NotSupportSence()
        {
            InitializeComponent();
            MainWindow.Instance.TitleText.Text = App.GetText("NotSupportTitle");
            Loaded += NotSupportSence_Loaded;
        }

        void NotSupportSence_Loaded(object sender, RoutedEventArgs e)
        {
            var text = string.Empty;
            foreach (var info in Infos)
            {
                text += "● " + info + Environment.NewLine;
            }
            InfoText.Text = text;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.EndApp();
        }
    }
}
