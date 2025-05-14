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

namespace Win7ThemeEditor
{
	/// <summary>
	/// ThemeIcon.xaml 的交互逻辑
	/// </summary>
	public partial class ThemeIcon
	{
        public string Image0Path
        {
            get { return (string)GetValue(Image0PathProperty); }
            set { SetValue(Image0PathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Image1Path.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty Image0PathProperty =
            DependencyProperty.Register("Image0Path", typeof(string), typeof(ThemeIcon), new PropertyMetadata(null,
                (o, e) =>
                {
                    var themeIcon = o as ThemeIcon;
                    if (themeIcon == null || !themeIcon.IsLoaded) return;
                    themeIcon.UpdateDisplay();
                }));

        public string Image1Path
        {
            get { return (string)GetValue(Image1PathProperty); }
            set { SetValue(Image1PathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Image1Path.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty Image1PathProperty =
            DependencyProperty.Register("Image1Path", typeof(string), typeof(ThemeIcon), new PropertyMetadata(null,
                (o, e) =>
                {
                    var themeIcon = o as ThemeIcon;
                    if (themeIcon == null || !themeIcon.IsLoaded) return;
                    themeIcon.UpdateDisplay();
                }));



        public string Image2Path
        {
            get { return (string)GetValue(Image2PathProperty); }
            set { SetValue(Image2PathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Image2Path.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty Image2PathProperty =
            DependencyProperty.Register("Image2Path", typeof(string), typeof(ThemeIcon), new PropertyMetadata(null));



        public string Image3Path
        {
            get { return (string)GetValue(Image3PathProperty); }
            set { SetValue(Image3PathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Image3Path.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty Image3PathProperty =
            DependencyProperty.Register("Image3Path", typeof(string), typeof(ThemeIcon), new PropertyMetadata(null));



        public Visibility NameVisibility
        {
            get { return (Visibility)GetValue(NameVisibilityProperty); }
            set { SetValue(NameVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NameVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NameVisibilityProperty =
            DependencyProperty.Register("NameVisibility", typeof(Visibility), typeof(ThemeIcon), new PropertyMetadata(Visibility.Visible));


        public string ThemeName
        {
            get { return (string)GetValue(ThemeNameProperty); }
            set { SetValue(ThemeNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ThemeName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ThemeNameProperty =
            DependencyProperty.Register("ThemeName", typeof(string), typeof(ThemeIcon), new PropertyMetadata(null));



        public Color ThemeColor
        {
            get { return (Color)GetValue(ThemeColorProperty); }
            set { SetValue(ThemeColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ThemeColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ThemeColorProperty =
            DependencyProperty.Register("ThemeColor", typeof(Color), typeof(ThemeIcon), new PropertyMetadata(null));



        public string BrandImagePath
        {
            get { return (string)GetValue(BrandImagePathProperty); }
            set { SetValue(BrandImagePathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BrandImagePath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BrandImagePathProperty =
            DependencyProperty.Register("BrandImagePath", typeof(string), typeof(ThemeIcon), new PropertyMetadata(null));



		public ThemeIcon()
		{
			InitializeComponent();
            Loaded += ThemeIcon_Loaded;
		}

        void ThemeIcon_Loaded(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        public void UpdateDisplay()
        {
            
        }
	}
}