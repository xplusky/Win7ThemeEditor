using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media.Imaging;

namespace ThemeInstaller.Pages
{
	public partial class WelcomeSence
	{
        public WelcomeSence()
		{
			InitializeComponent();
            MainWindow.Instance.TitleText.Text = Setup.Settings["Title"].Content;
            InitLanguage();
		}

        public static void LoadLanguageFile(string languagefileName)
        {
            Application.Current.Resources.MergedDictionaries[1] = new ResourceDictionary
            {
                Source = new Uri(languagefileName, UriKind.RelativeOrAbsolute)
            };

        }

	    public static bool IsInitOnce = false;

	    private void InitLanguage()
	    {
	        //LangChooseBomboBox.Items.Clear();
	        foreach (var file in new DirectoryInfo(Path.Combine(Paths.AppDir, @"Tools\Lang")).GetFiles().Where(file => file.Extension == ".xaml"))
            {
                ResourceDictionary dict;
                using (var fs = new FileStream(file.FullName, FileMode.Open, FileAccess.Read))
                {
                    dict = XamlReader.Load(fs) as ResourceDictionary;
                }
                var culture = Path.GetFileNameWithoutExtension(file.FullName);

                var lang = new Lang {Name = dict["LangName"].ToString(), Path = file.FullName, Culture = culture};
                if (!IsInitOnce) LangItems.Add(lang);
                if (culture == App.Culture) LangChooseBomboBox.SelectedIndex = LangItems.IndexOf(lang);
            }
            LangChooseBomboBox.SelectionChanged += LangChooseBomboBoxOnSelectionChanged;
	        IsInitOnce = true;
	    }

	    private void LangChooseBomboBoxOnSelectionChanged(object sender, SelectionChangedEventArgs selectionChangedEventArgs)
	    {
            //XmlConfigHelper.WriteConfigData("Culture", LangItems[LangChooseBomboBox.SelectedIndex].Culture);
            //var filepath = LangItems[LangChooseBomboBox.SelectedIndex].Path;
            //ResourceDictionary dict;
            //using (var fs = new FileStream(filepath, FileMode.Open, FileAccess.Read))
            //{
            //    dict = XamlReader.Load(fs) as ResourceDictionary;
            //}
	        if (LangChooseBomboBox.SelectedIndex == -1) return;
            LoadLanguageFile(string.Format("pack://siteOfOrigin:,,,/Tools/Lang/{0}.xaml", LangItems[LangChooseBomboBox.SelectedIndex].Culture));
	        App.Culture = LangItems[LangChooseBomboBox.SelectedIndex].Culture;
	        //if (dict != null)
	        //{
	        //    var message = dict["AboutHelpSetLangChangeMes"];
	        //    MessageBox.Show(message.ToString());
	        //}
	    }

        public ObservableCollection<Lang> LangItems
        {
            get { return (ObservableCollection<Lang>)GetValue(LangItemsProperty); }
            set { SetValue(LangItemsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LangItems.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LangItemsProperty =
            DependencyProperty.Register("LangItems", typeof(ObservableCollection<Lang>), typeof(WelcomeSence), new PropertyMetadata(new ObservableCollection<Lang>()));

        

	    private void EnterSetupButtonClick(object sender, RoutedEventArgs e)
        {
            if (NavigationService != null) NavigationService.Navigate(FlowPages.NextPage());
        }

	    private void MainPageElementLoaded(object sender, RoutedEventArgs e)
        {
            var welcomeImage = Setup.Settings["WelcomeImagePath"].Content;
            if (File.Exists(welcomeImage))
            {
                var bitImg = new BitmapImage(new Uri(Paths.AppDir + "\\" + welcomeImage));
                WelcomeImage.Source = bitImg;
                WelcomeImage.Width = bitImg.PixelWidth;
                WelcomeImage.Height = bitImg.PixelHeight;
            }
        }
	}
}