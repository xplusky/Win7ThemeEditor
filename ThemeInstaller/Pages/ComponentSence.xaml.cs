#region

using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;

#endregion

namespace ThemeInstaller.Pages
{
    /// <summary>
    /// 组件选择页面
    /// </summary>
    public partial class ComponentSence
    {
        public ComponentSence()
        {
            
            InitializeComponent();
            MainWindow.Instance.TitleText.Text = App.GetText("ComponentTitle");
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            CheckBox[] checkBoxs = 
            {
                CheckboxTheme, CheckboxFonts, CheckboxCursors, CheckboxIcon, CheckboxInput,
                CheckboxStartButton, CheckboxLogonImage, CheckboxOthers, CheckboxCreatUninstall ,
                CheckboxCreatDesktopIcon
            };
            Components.LinkCheckBox(checkBoxs);
            Components.ApplyCheck();
            if (!Components.IsExist(Components.Enum.Theme)) HideCheckbox(CheckboxTheme, CheckboxCursors, CheckboxIcon);
            if (!Components.IsExist(Components.Enum.Fonts)) HideCheckbox(CheckboxFonts);
            if (!Components.IsExist(Components.Enum.InputSkins)) HideCheckbox(CheckboxInput);
            if (!Components.IsExist(Components.Enum.Others)) HideCheckbox(CheckboxOthers);
            if (!Components.IsExist(Components.Enum.StartButton)) HideCheckbox(CheckboxStartButton);
            if (!Components.IsExist(Components.Enum.LogonImage)) HideCheckbox(CheckboxLogonImage);
            Components.SaveCheck();
        }

        private static void HideCheckbox(params CheckBox[] checkboxes)
        {
            foreach (var checkbox in checkboxes)
            {
                checkbox.Visibility = Visibility.Collapsed;
                checkbox.IsChecked = false;
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService != null) NavigationService.Navigate(FlowPages.PrevPage());
        }

        private void BtnNext_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService != null) NavigationService.Navigate(FlowPages.NextPage());
        }

        private void CheckboxComponent_Click(object sender, RoutedEventArgs e)
        {
            Components.SaveCheck();
        }

        private void CheckboxStartButton_Click(object sender, RoutedEventArgs e)
        {
            Components.SaveCheck();
            if (CheckboxStartButton.IsChecked == true)
            {
                MessageBox.Show(App.GetText("ComponentStartButtonTip"));
            }
        }
    }
}