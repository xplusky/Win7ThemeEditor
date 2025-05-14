#region

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Media;
using System.Net;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Xml;
using Microsoft.Win32;
using Win7ThemeEditor.Helper;
using Win7ThemeEditor.Properties;
using System.Linq;

#endregion

namespace Win7ThemeEditor
{
    public partial class MainWindow
    {
        #region Property

        public static readonly DependencyProperty OpenThemeItemSourceProperty =
            DependencyProperty.Register("OpenThemeItemSource", typeof (ObservableCollection<OpenThemeItemClass>), typeof (MainWindow), new PropertyMetadata(new ObservableCollection<OpenThemeItemClass>()));


        public static readonly DependencyProperty AppVersionProperty =
            DependencyProperty.Register("AppVersion", typeof (string), typeof (MainWindow), new PropertyMetadata(App.AppVersion));


        public static readonly DependencyProperty LangItemsProperty =
            DependencyProperty.Register("LangItems", typeof (ObservableCollection<Lang>), typeof (AppSettings), new PropertyMetadata(new ObservableCollection<Lang>()));

        public ObservableCollection<OpenThemeItemClass> OpenThemeItemSource
        {
            get { return (ObservableCollection<OpenThemeItemClass>) GetValue(OpenThemeItemSourceProperty); }
            set { SetValue(OpenThemeItemSourceProperty, value); }
        }

        public string AppVersion
        {
            get { return (string) GetValue(AppVersionProperty); }
            set { SetValue(AppVersionProperty, value); }
        }

        public ObservableCollection<Lang> LangItems
        {
            get { return (ObservableCollection<Lang>) GetValue(LangItemsProperty); }
            set { SetValue(LangItemsProperty, value); }
        }

        public static MainWindow Instance = null;

        public Storyboard BakgroundAnimateHanaFadeStoryboard, BakgroundAnimateHanaSpinStoryboard,
                          BakgroundAnimateHnaSpinStoryboard, OpenSenceStoryboard, NewThemeStoryboard,
                          BoxShowStoryboard, BoxHideStoryboard, NewAppVersionStoryboard, TipBubbleShowStoryboard;


        #region Nested type: Lang

        public class Lang
        {
            public string Name { get; set; }
            public string Culture { get; set; }
            public string Path { get; set; }
        }

        #endregion

        #region Nested type: OpenThemeItemClass

        public class OpenThemeItemClass
        {
            public string ImageFilePath { get; set; }
            public string DisplayName { get; set; }
            public string FilePath { get; set; }
        }

        #endregion

        #endregion

        #region 初始化

        public MainWindow()
        {
            Instance = this;
            InitializeComponent();
        }

        private void MainWindowElementSourceInitialized(object sender, EventArgs e)
        {
            // 设置Aero界面
            Background = AeroHelper.ExtendAeroGlass(this) ? Brushes.Transparent : Brushes.White;
        }

        private void MainWindowElementLoaded(object sender, RoutedEventArgs e)
        {
            AppSettingsElement.Load();//读取设置
            AppDebug.Log("InitAppSetting Loaded");
            InitStoryboard();
            InitVisual();
            AppDebug.Log("InitVisual Loaded");
            InitFiles();
            AppDebug.Log("InitFiles Ok");
            UpdateApp();
            InitLanguage();
            InitEvent();

            AppDebug.Log("Load Ok");
        }

        /// <summary>
        /// 初始化语言设置
        /// </summary>
        private void InitLanguage()
        {
            foreach (var file in new DirectoryInfo(Path.Combine(Paths.AppDir, @"Res\Lang")).GetFiles())
            {
                var extension = Path.GetExtension(file.FullName);
                if (extension.ToLower() != ".xaml") continue;
                ResourceDictionary dict;
                using (var fs = new FileStream(file.FullName, FileMode.Open, FileAccess.Read))
                {
                    dict = XamlReader.Load(fs) as ResourceDictionary;
                }
                var culture = Path.GetFileNameWithoutExtension(file.FullName);
                if (dict != null) LangItems.Add(new Lang {Name = dict["LangName"].ToString(), Path = file.FullName, Culture = culture});
                if (culture == App.Culture) LangChooseBomboBox.SelectedIndex = LangItems.Count - 1;
            }

            try
            {
                InstVersonText.Text = string.Format(App.FindString("InstIcon"), App.FindString("ThemeInstallerVersion"));
            }
            catch (Exception ex)
            {
                AppDebug.Log(ex.Message);
            }
            AboutIconNameText.Text = string.Format(App.FindString("AboutAppName"), App.AppVersion);
            foreach (ComboBoxItem item in WallpaperSliderIntervalCombobox.Items)
            {
                item.Content = item.Content.ToString().Replace(" s", App.FindString("ComSec")).Replace(" m", App.FindString("ComMin")).Replace(" h", App.FindString("ComHour")).Replace(" d", App.FindString("ComDay"));
            }
            AboutCopyrightTextblock.Text = string.Format(App.FindString("AboutCopyrightText"), App.FindString("AppContributors"), ((DateTime) Application.Current.TryFindResource("AppLastUpdateTime")).ToLongDateString());

            //MessageBox.Show(System.Globalization.CultureInfo.CurrentCulture.Name);
        }

        /// <summary>
        /// 初始化临时文件
        /// </summary>
        public static void InitFiles()
        {
            if (Directory.Exists(Paths.AppTempDir))
            {
                try
                {
                    Directory.Delete(Paths.AppTempDir, true);
                    Directory.CreateDirectory(Paths.AppTempDir);
                }
                catch (Exception ex)
                {
                    AppDebug.Log(ex.Message);
                    AdminHelper.AdminExecute("/initfiles");
                }
            }
            else Directory.CreateDirectory(Paths.AppTempDir);
        }

        /// <summary>
        /// 初始化视觉效果
        /// </summary>
        private void InitVisual()
        {
            // 阴影
            Panel[] frameworkElements =
            {
                ThemeContentGrid, BoxGrid
            };
            foreach (var child in frameworkElements.SelectMany(
                element => element.Children.OfType<FrameworkElement>()))
            {
                child.Effect = new DropShadowEffect
                {
                    ShadowDepth = 0, 
                    BlurRadius = 10, 
                    Color = Colors.White
                };
            }

            // 可见性
            HideBoxes();

            // 更新图标
            try
            {
                UpdateThemeIcon();
            }
            catch (Exception ex)
            {
                AppDebug.Log(ex.Message);
            }

            // 更新信息
            UpdateMssFileInfo();

        }

        private void InitStoryboard()
        {
            foreach (var member in typeof (MainWindow).GetFields().Where(m => m.FieldType == typeof (Storyboard)))
            {
                //AppDebug.Log(member.Name);
                var sb = TryFindResource(member.Name) as Storyboard;
                if (sb == null)
                {
                    MessageBox.Show(member.Name + "storyboard error");
                }
                member.SetValue(this , sb);
                //member.SetValue(member, TryFindResource(member.Name) as Storyboard);
            }

            Funcs.StoryboardAct(Funcs.StoryboardActionEnum.Begin,
                BakgroundAnimateHanaFadeStoryboard, BakgroundAnimateHanaSpinStoryboard,
                BakgroundAnimateHnaSpinStoryboard);
            if (AppSettingsElement.IsBackgroundAnimating == false)
            {
                Funcs.StoryboardAct(Funcs.StoryboardActionEnum.Pause,
                BakgroundAnimateHanaFadeStoryboard, BakgroundAnimateHanaSpinStoryboard,
                BakgroundAnimateHnaSpinStoryboard);
            }
            OpenSenceStoryboard.Begin();
        }

        #endregion

        #region 方法


        private void HideBoxes()
        {
            FrameworkElement[] boxCollects =
            {
                SfxGird, AboutGrid, WallpaperViewerGrid, 
                OpenThemeFileGrid, InstallerGrid
            };
            foreach (var ui in boxCollects) ui.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// 更新软件
        /// </summary>
        public void UpdateApp()
        {
            using (var webClient = new WebClient())
            {
                webClient.OpenReadCompleted +=
                    (o, e) =>
                    {
                        AppDebug.Log(e.Error);
                        try
                        {
                            var xml = new XmlDocument();
                            xml.Load(e.Result);
                            var ver = xml.SelectSingleNode("Root/Ver");
                            if (ver != null)
                            {
                                var version = new Version(ver.InnerText);
                                AppDebug.Log("Online Version: " + version);
                                if (version > Assembly.GetExecutingAssembly().GetName().Version)
                                {
                                    AboutUpdateButton.Content = string.Format(App.FindString("AboutBtnUpdateMessage"), version.ToString(3));
                                    NewAppVersionStoryboard.Begin();
                                }
                            }
                            var url = xml.SelectSingleNode("Root/Url");
                            if (url != null) AboutUpdateButtonAction.Path = url.InnerText;
                        }
                        catch (Exception ex)
                        {
                            AppDebug.Log("WebOpenReadFail: " + ex.Message);
                        }
                    };
                try
                {
                    var uri = new Uri(App.FindString("AppUpdateXmlUrl"), UriKind.RelativeOrAbsolute);
                    webClient.OpenReadAsync(uri);
                }
                catch (Exception ex)
                {
                    AppDebug.Log("OpenReadAsyncFailed: " + ex.Message);
                }
            }
        }
        
        private void UpdateThemeIcon()
        {
            var source = ThemeSettingsElement.WallpaperFileItemsSource;
            switch (source.Count)
            {
                case 0:
                {
                    ThemeIconControl.Wallpaper0.ImagePath = Paths.DefaultWallpaperFile;
                    ThemeIconControl.Wallpaper1.ImagePath = null;
                    ThemeIconControl.Wallpaper2.ImagePath = null;
                    ThemeIconControl.Wallpaper3.ImagePath = null;
                    break;
                }
                case 1:
                {
                    ThemeIconControl.Wallpaper0.ImagePath = source[0].ImageFilePath;
                    ThemeIconControl.Wallpaper1.ImagePath = null;
                    ThemeIconControl.Wallpaper2.ImagePath = null;
                    ThemeIconControl.Wallpaper3.ImagePath = null;
                    break;
                }
                case 2:
                {
                    ThemeIconControl.Wallpaper0.ImagePath = null;
                    ThemeIconControl.Wallpaper1.ImagePath = source[0].ImageFilePath;
                    ThemeIconControl.Wallpaper2.ImagePath = source[1].ImageFilePath;
                    ThemeIconControl.Wallpaper3.ImagePath = null;
                    break;
                }
                default:
                {
                    ThemeIconControl.Wallpaper0.ImagePath = null;
                    ThemeIconControl.Wallpaper1.ImagePath = source[0].ImageFilePath;
                    ThemeIconControl.Wallpaper2.ImagePath = source[1].ImageFilePath;
                    ThemeIconControl.Wallpaper3.ImagePath = source[2].ImageFilePath;
                    break;
                }
            }
        }

        private bool IsFileNameIllegal()
        {
            if (ThemeSettingsElement.ThemeFileName == null || ThemeSettingsElement.ThemeDiaplayName == null)
            {
                MessageBox.Show(App.FindString("TopSaveSameNameMessage"));
                return true;
            }
            return false;
        }

        /// <summary>
        /// 更新打开主题列表
        /// </summary>
        private void UpdateOpenThemeFileList()
        {
            OpenThemeItemSource.Clear();
            var themeFiles = (new DirectoryInfo(Paths.SysThemeDir)).GetFiles();

            foreach (var file in themeFiles)
            {
                if (Path.GetExtension(file.FullName.ToLower()) != ".theme") continue;
                var ini = new IniHelper(file.FullName);
                var displayName = ini.ReadInivalue("Theme", "DisplayName");
                var imgFile = ini.ReplaceEnviromentVariable(ini.ReadInivalue(@"Control Panel\Desktop", "Wallpaper"));
                if (!File.Exists(imgFile))
                    imgFile = null;
                if (displayName.IndexOf("themeui.dll", StringComparison.Ordinal) == -1)
                {
                    OpenThemeItemSource.Add(new OpenThemeItemClass {FilePath = file.FullName, DisplayName = displayName, ImageFilePath = imgFile});
                }
            }
        }

        public static void ShowBubbleMessage(string message)
        {
            Instance.MessageBubbleText.Text = message;
            Instance.TipBubbleShowStoryboard.Begin();
        }

        public static void UpdateMssFileInfo()
        {
            var path = Instance.ThemeSettingsElement.MsstylesFilePath;
            if (!File.Exists(path)) return;
            Instance.MsstylesFileFileInfoText.Text = string.Format(App.FindString("TabStyleFileInfo"), Math.Round(((double)new FileInfo(path).Length) / 1024 / 1024, 2));
        }

        #endregion

        #region 事件相应

        #region Main

        /// <summary>
        /// 初始化注册事件
        /// </summary>
        private void InitEvent()
        {
            ThemeSettingsElement.WallpaperFileItemsSource.CollectionChanged +=
                (o, e) =>
                {
                    var source = ThemeSettingsElement.WallpaperFileItemsSource;
                    WallpaperUpDownGroup.IsEnabled = source.Count > 1;
                    WallpaperSliderSettingGroup.IsEnabled = source.Count > 1;
                };

            BoxHideStoryboard.Completed += (o, e) => HideBoxes();

            LangChooseBomboBox.SelectionChanged +=
                (o, e) =>
                {
                    XmlConfigHelper.WriteConfigData("Culture", LangItems[LangChooseBomboBox.SelectedIndex].Culture);
                    var filepath = LangItems[LangChooseBomboBox.SelectedIndex].Path;
                    ResourceDictionary dict;
                    using (var fs = new FileStream(filepath, FileMode.Open, FileAccess.Read))
                    {
                        dict = XamlReader.Load(fs) as ResourceDictionary;
                    }
                    if (dict != null)
                    {
                        var message = dict["AboutHelpSetLangChangeMes"];
                        MessageBox.Show(message.ToString());
                    }
                };
        }

        private void MainWindowElementClosing(object sender, CancelEventArgs e)
        {
            AppSettingsElement.Save();
        }

        private void MainWindowElementKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.F6:
                {
                    OpenSenceStoryboard.Begin();
                    break;
                }
                case Key.F7:
                {
                    ShowBubbleMessage("其实没有出错啦。");
                    break;
                }
                case Key.F8:
                {
                    App.ShowErrorForm(App.FindString("SurpriseMessage"));
                    break;
                }
                case Key.F9:
                {
                    break;
                }
            }
        }

        private void MainWindowElementMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                DragMove();
            }
            catch (Exception ex)
            {
                AppDebug.Log("DragMove Fail " + ex.Message);
            }
        }

        #endregion

        #region Theme

        private void WallpaperAddButtonClick(object sender, RoutedEventArgs e)
        {
            var files =Funcs.OpenFile(App.FindString("FileTypePngJpgGifBmp") + "|*.jpg;*.png;*.bmp;*.gif", true);
            if (files == null) return;
            foreach (var file in files)
                ThemeSettingsElement.WallpaperFileItemsSource.Add(new ThemeSettings.WallpaperFileItem {ImageFilePath = file});
        }

        private void WallpaperClearButtonClick(object sender, RoutedEventArgs e)
        {
            ThemeSettingsElement.WallpaperFileItemsSource.Clear();
        }

        private void WallpaperDeleteButtonClick(object sender, RoutedEventArgs e)
        {
            if (WallpaperItemsListBox.SelectedIndex != -1) ThemeSettingsElement.WallpaperFileItemsSource.RemoveAt(WallpaperItemsListBox.SelectedIndex);
        }

        private void WallpaperUpButtonClick(object sender, RoutedEventArgs e)
        {
            if (WallpaperItemsListBox.SelectedIndex != -1 && WallpaperItemsListBox.SelectedIndex != 0) ThemeSettingsElement.WallpaperFileItemsSource.Move(WallpaperItemsListBox.SelectedIndex, WallpaperItemsListBox.SelectedIndex - 1);
        }

        private void WallpaperDownButtonClick(object sender, RoutedEventArgs e)
        {
            if (WallpaperItemsListBox.SelectedIndex != -1 && WallpaperItemsListBox.SelectedIndex != WallpaperItemsListBox.Items.Count - 1) ThemeSettingsElement.WallpaperFileItemsSource.Move(WallpaperItemsListBox.SelectedIndex, WallpaperItemsListBox.SelectedIndex + 1);
        }

        private void SaveThemeFileToSystemButtonClick(object sender, RoutedEventArgs e)
        {
            if (IsFileNameIllegal()) return;
            var tempThemeFolder = Paths.AppTempDir + @"\Theme";
            if (MessageBox.Show(App.FindString("TopSaveMessage"), "Tip", MessageBoxButton.OKCancel) != MessageBoxResult.OK) return;
            var themeMssDirPath = Path.Combine(Paths.SysThemeDir, ThemeSettingsElement.ThemeFileName);
            var themeFilePath = Path.Combine(Paths.SysThemeDir , ThemeSettingsElement.ThemeFileName + ".theme");
            if ((Directory.Exists(themeMssDirPath) || File.Exists(themeFilePath)) &&
                MessageBox.Show(App.FindString("TopSaveOverwriteMessage"), "Tip", MessageBoxButton.YesNo) == MessageBoxResult.No) return;
            var saveArgs = "/save";
            if (Directory.Exists(themeMssDirPath)) saveArgs += " \"" + themeMssDirPath + "\"";
            InitFiles();
            if (!Directory.Exists(tempThemeFolder)) Directory.CreateDirectory(tempThemeFolder);
            ThemeSettingsElement.SaveThemeFiles(tempThemeFolder);
            AppDebug.Log("Save to temp dir OK");
            AdminHelper.AdminExecute(saveArgs);
            ShowBubbleMessage(App.FindString("TopSaveOk"));
        }

        private void IconChangeButtonClick(object sender, RoutedEventArgs e)
        {
            if (IconItemsListbox.SelectedIndex == -1) return;
            var files =Funcs.OpenFile(App.FindString("FileTypeIco") + "|*.ico", true);
            if (files == null) return;
            if (files.Count() > 1)
            {
                ThemeSettingsElement.BatchReadIcons(files);
            }
            else
            {
                var item = ThemeSettingsElement.IconItemSource[IconItemsListbox.SelectedIndex];
                item.IconFilePath = files[0];
            }
            IconItemsListbox.Items.Refresh();
        }

        private void IconRestoreButtonClick(object sender, RoutedEventArgs e)
        {
            var item = ThemeSettingsElement.IconItemSource[IconItemsListbox.SelectedIndex];
            if (IconItemsListbox.SelectedIndex == -1 || item.DefaultIconPath == null) return;
            item.IconFilePath = item.DefaultIconPath;
            IconItemsListbox.Items.Refresh();
        }

        private void CursorChangeButton_Click(object sender, RoutedEventArgs e)
        {
            if (CursorsListbox.SelectedIndex == -1) return;
            var files =Funcs.OpenFile(App.FindString("FileTypeCurAni") + "|*.cur;*.ani", false);
            if (files == null) return;
            ThemeSettingsElement.CursorsItemSource[CursorsListbox.SelectedIndex].CursorFilePath = files[0];
            CursorsListbox.Items.Refresh();
        }

        private void CursorRestoreButton_Click(object sender, RoutedEventArgs e)
        {
            if (CursorsListbox.SelectedIndex == -1) return;
            ThemeSettingsElement.CursorsItemSource[CursorsListbox.SelectedIndex].CursorFilePath = null;
            CursorsListbox.Items.Refresh();
        }

        private void NewThemeButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(App.FindString("TopNewMessage"), "Tip", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                ThemeSettingsElement.SetDefault();
                NewThemeStoryboard.Begin();
            }
            CursorsListbox.Items.Refresh();
            IconItemsListbox.Items.Refresh();
            UpdateThemeIcon();
        }

        private void WallpaperItemsListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (WallpaperItemsListBox.SelectedIndex == -1) return;
            var pic = ThemeSettingsElement.WallpaperFileItemsSource[WallpaperItemsListBox.SelectedIndex].ImageFilePath;

            WallpaperViewerImage.ImagePath = pic;
            var bitimg = Funcs.GetBitmapImage(pic);
            WallpaperViewerInfoText.Text = string.Format(App.FindString("TabWallpaperImageInfo"), bitimg.PixelWidth, bitimg.PixelHeight, (new FileInfo(pic)).Length/1024);
            BoxShowStoryboard.Begin();
            WallpaperViewerGrid.Visibility = Visibility.Visible;
        }

        private void WallpaperItemsListBoxDragEnter(object sender, DragEventArgs e)
        {
            //判断拖入的对象格式
            if (e.Data.GetDataPresent(DataFormats.FileDrop, true))
            {
                //允许拖放
                e.Effects = DragDropEffects.All;
            }
        }

        private void WallpaperItemsListBox_Drop(object sender, DragEventArgs e)
        {
            //获取文件列表（文件夹会被当作文件处理）
            foreach (var file in (string[]) e.Data.GetData(DataFormats.FileDrop))
            {
                var ext = new FileInfo(file).Extension.ToLower();
                if (ext == ".jpg" || ext == ".png" || ext == ".bmp" || ext == ".gif")
                    ThemeSettingsElement.WallpaperFileItemsSource.Add(new ThemeSettings.WallpaperFileItem {ImageFilePath = file});
            }
        }

        private void SytemSoundChangeButton_Click(object sender, RoutedEventArgs e)
        {
            if (SytemSoundItemListbox.SelectedIndex == -1) return;
            var files =Funcs.OpenFile(App.FindString("FileTypeWav") + "|*.wav", false);
            if (files != null) ThemeSettingsElement.SystemSoundItemSource[SytemSoundItemListbox.SelectedIndex].Path = files[0];
            SytemSoundItemListbox.Items.Refresh();
        }

        private void SytemSoundRestoreButton_Click(object sender, RoutedEventArgs e)
        {
            if (SytemSoundItemListbox.SelectedIndex != -1)
            {
                ThemeSettingsElement.SystemSoundItemSource[SytemSoundItemListbox.SelectedIndex].Path = null;
            }
            SytemSoundItemListbox.Items.Refresh();
        }

        private void SytemSoundPlay_Click(object sender, RoutedEventArgs e)
        {
            var path = ThemeSettingsElement.SystemSoundItemSource[SytemSoundItemListbox.SelectedIndex].Path;
            if (SytemSoundItemListbox.SelectedIndex == -1 || !File.Exists(path)) return;
            using (var player = new SoundPlayer(path))
            {
                try
                {
                    player.Play();
                }
                catch (Exception ex)
                {
                    AppDebug.Log(ex.Message);
                }
            }
        }

        private void ThemeLogoChangeButton_Click(object sender, RoutedEventArgs e)
        {
            var files =Funcs.OpenFile(App.FindString("FileTypePng") + "|*.png", false);
            if (files != null) ThemeSettingsElement.LogoFilePath = files[0];
        }

        private void ThemeLogoRestoreButtonClick(object sender, RoutedEventArgs e)
        {
            ThemeSettingsElement.LogoFilePath = null;
        }

        private void OpenThemeButtonClick(object sender, RoutedEventArgs e)
        {
            BoxShowStoryboard.Begin();
            OpenThemeFileGrid.Visibility = Visibility.Visible;
            try
            {
                UpdateOpenThemeFileList();
            }
            catch (Exception ex)
            {
                App.ShowErrorForm(ex);
            }
        }

        private void OpenThemeFileDeleteButtonClick(object sender, RoutedEventArgs e)
        {
            if (OpenThemeFileListbox.SelectedIndex == -1 ||
                MessageBox.Show(App.FindString("OpenDelMessage"), "Tip", MessageBoxButton.YesNo) != MessageBoxResult.Yes) 
                return;
            var delArgs = "/deltheme";
            var themePath = OpenThemeItemSource[OpenThemeFileListbox.SelectedIndex].FilePath;
            delArgs += " \"" + themePath + "\"";
            var ini = new IniHelper(themePath);
            var mss = ini.GetRealPath(ini.ReadInivalue("VisualStyles", "Path"));
            
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(mss);
            if (fileNameWithoutExtension != null && fileNameWithoutExtension.ToLower() != "aero")
            {
                var mssDir = Path.GetDirectoryName(mss);
                if (mssDir != null) delArgs += " \"" + mssDir + "\"";
            }
            AdminHelper.AdminExecute(delArgs);
            UpdateOpenThemeFileList();
        }

        private void InstallerPanelButtonClick(object sender, RoutedEventArgs e)
        {
            BoxShowStoryboard.Begin();
            InstallerGrid.Visibility = Visibility.Visible;
        }

        private void AboutPanelButton_Click(object sender, RoutedEventArgs e)
        {
            BoxShowStoryboard.Begin();
            AboutGrid.Visibility = Visibility.Visible;
        }

        private void OpenThemeFileCancelButton_Click(object sender, RoutedEventArgs e)
        {
            BoxHideStoryboard.Begin();
        }

        private void OpenThemeFileListbox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            OpenThemeFileOkButton_Click(null, null);
        }

        private void OpenThemeFileOkButton_Click(object sender, RoutedEventArgs e)
        {
            if (OpenThemeFileListbox.SelectedIndex != -1)
            {
                ThemeSettingsElement.SetDefault();
                ThemeSettingsElement.ReadThemeFile(OpenThemeItemSource[OpenThemeFileListbox.SelectedIndex].FilePath);
                BoxHideStoryboard.Begin();
                NewThemeStoryboard.Begin();
            }

            CursorsListbox.Items.Refresh();
            IconItemsListbox.Items.Refresh();
            SytemSoundItemListbox.Items.Refresh();
            UpdateThemeIcon();
        }

        private void AboutBackButton_Click(object sender, RoutedEventArgs e)
        {
            BoxHideStoryboard.Begin();
        }

        private void WallpaperViewerBackButton_Click(object sender, RoutedEventArgs e)
        {
            BoxHideStoryboard.Begin();
        }

        private void MsstylesFileChangeButton_Click(object sender, RoutedEventArgs e)
        {
            var files =Funcs.OpenFile(App.FindString("FileTypeMsstyles") + "|*.msstyles", false);
            if (files != null)
            {
                ThemeSettingsElement.MsstylesFilePath = files[0];
            }
        }

        private void MsstylesFileRestoreButton_Click(object sender, RoutedEventArgs e)
        {
            ThemeSettingsElement.MsstylesFilePath = Environment.GetEnvironmentVariable("windir") + @"\Resources\Themes\Aero\aero.msstyles";
        }

        private void MsstylesFileEditButton_Click(object sender, RoutedEventArgs e)
        {
            var dir = AppSettingsElement.WsbExeFilePath;
            if (string.IsNullOrEmpty(dir) || !File.Exists(dir))
            {
                MessageBox.Show(App.FindString("TabStyleBtnEditMssNotSet"));
                MsstylesFileEditAppPathSetButton_Click(sender, null);
            }
            else Process.Start("Explorer.exe", dir);
        }

        private void MsstylesFileEditAppPathSetButton_Click(object sender, RoutedEventArgs e)
        {
            var op = new OpenFileDialog
            {
                RestoreDirectory = true,
                Filter = App.FindString("FileTypeExe") + "|*.exe"
            };
            op.ShowDialog();
            if (op.FileName != string.Empty)
            {
                AppSettingsElement.WsbExeFilePath = op.FileName;
            }
        }

        private void HanaSpinSwichButton_Click(object sender, RoutedEventArgs e)
        {
            AppSettingsElement.IsBackgroundAnimating = !AppSettingsElement.IsBackgroundAnimating;
            Funcs.StoryboardAct(
                AppSettingsElement.IsBackgroundAnimating == true ? Funcs.StoryboardActionEnum.Resume : Funcs.StoryboardActionEnum.Pause,
                BakgroundAnimateHanaFadeStoryboard, BakgroundAnimateHanaSpinStoryboard,
                BakgroundAnimateHnaSpinStoryboard);
        }

        private void ContentTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ContentTabControl.SelectedIndex == 0) UpdateThemeIcon();
        }

        private void SytemSoundDefaultCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var box = SytemSoundDefaultCombobox;
            switch (box.SelectedIndex)
            {
                case 0:
                {
                    using (var rootKey = Registry.CurrentUser)
                    {
                        foreach (var item in ThemeSettingsElement.SystemSoundItemSource)
                        {
                            var key = rootKey.OpenSubKey(item.IniSection + @"\.Default");
                            if (key == null) continue;
                            var value = key.GetValue("");
                            if (value == null) continue;
                            if (File.Exists(value.ToString()))
                                item.Path = value.ToString();
                        }
                    }
                    break;
                }
                default:
                {
                    foreach (var item in ThemeSettingsElement.SystemSoundItemSource)
                    {
                        item.Path = null;
                    }
                    break;
                }
            }
            if (SytemSoundItemListbox != null)
                SytemSoundItemListbox.Items.Refresh();
        }

        private void CursorReadInfButton_Click(object sender, RoutedEventArgs e)
        {
            var files =Funcs.OpenFile(App.FindString("FileTypeInf") + "|*.inf;*.ini", false);
            if (files != null)
            {
                ThemeSettingsElement.ReadCursorInf(files[0]);
            }
            CursorsListbox.Items.Refresh();
        }

        #endregion

        #region ThemeInstaller

        private void SfxBack_Click(object sender, RoutedEventArgs e)
        {
            BoxHideStoryboard.Begin();
        }

        private void SfxPanelButton_Click(object sender, RoutedEventArgs e)
        {
            BoxShowStoryboard.Begin();
            SfxGird.Visibility = Visibility.Visible;
        }

        private void SfxCreatExeButton_Click(object sender, RoutedEventArgs e)
        {
            var sp = new SaveFileDialog
            {
                RestoreDirectory = true, 
                Filter = App.FindString("FileTypeExe") + "|*.exe"
            };
            sp.ShowDialog();
            if (sp.FileName == "") return;
            try
            {
                InitFiles();
                ThemeSettingsElement.SfxCreatExe(sp.FileName);
                InitFiles();
            }
            catch (Exception ex)
            {
                MessageBox.Show("安装包生成失败。T_T" + Environment.NewLine + ex);
            }
        }

        private void InstCreatExePack_Click(object sender, RoutedEventArgs e)
        {
            var op = new SaveFileDialog
            {
                Filter = App.FindString("FileTypeExe") + "|*.exe", 
                RestoreDirectory = true
            };
            op.ShowDialog();
            if (op.FileName == string.Empty) return;
            try
            {
                InitFiles();
                ThemeSettingsElement.InstallerCreatExe(op.FileName);
                InitFiles();
            }
            catch (Exception ex)
            {
                MessageBox.Show("安装包生成失败。T_T" + Environment.NewLine + ex);
            }
        }

        private void InstCostomThemeButton_Click(object sender, RoutedEventArgs e)
        {
            var files =Funcs.OpenFile(App.FindString("FileTypeTheme") + "|*.theme", false);
            if (files != null)
            {
                ThemeSettingsElement.InstCostomThemeFilePath = files[0];
            }
            //if (MessageBox.Show("检测到主题内容文件夹为：\"" + 
            //    Path.GetDirectoryName(ThemeSettingsElement.GetMssFileNameFromTheme(files[0])) + 
            //    "\",是否是该文件夹？", "", MessageBoxButton.YesNo) == MessageBoxResult.No)
            //{
            //    var diaglog = new System.Windows.Forms.FolderBrowserDialog
            //    {
            //        s
            //    };
            //}
        }

        private void InstBgImageButton_Click(object sender, RoutedEventArgs e)
        {
            var files =Funcs.OpenFile(App.FindString("FileTypePngJpgGifBmp") + "|*.png;*.jpg;*.gif;*.bmp", false);
            if (files != null)
            {
                ThemeSettingsElement.InstBackgroundImage = files[0];
            }
        }

        private void InstBgImageRestoreButton_Click(object sender, RoutedEventArgs e)
        {
            ThemeSettingsElement.InstBackgroundImage = null;
        }

        private void InstSplashLogoButton_Click(object sender, RoutedEventArgs e)
        {
            var files =Funcs.OpenFile(App.FindString("FileTypePngJpgGifBmp") + "|*.png;*.jpg;*.gif;*.bmp", false);
            if (files != null)
            {
                ThemeSettingsElement.InstSplashLogo = files[0];
            }
        }

        private void InstSplashLogoRestoreButton_Click(object sender, RoutedEventArgs e)
        {
            ThemeSettingsElement.InstSplashLogo = null;
        }

        private void InstWelcomeLogoButton_Click(object sender, RoutedEventArgs e)
        {
            var files =Funcs.OpenFile(App.FindString("FileTypePngJpgGifBmp") + "|*.png;*.jpg;*.gif;*.bmp", false);
            if (files != null)
            {
                ThemeSettingsElement.InstWelcomeLogo = files[0];
            }
        }

        private void InstWelcomeLogoRestoreButton_Click(object sender, RoutedEventArgs e)
        {
            ThemeSettingsElement.InstWelcomeLogo = null;
        }

        private void InstBgmButton_Click(object sender, RoutedEventArgs e)
        {
            var files =Funcs.OpenFile(App.FindString("FileTypeMp3") + "|*.mp3", false);
            if (files != null)
            {
                ThemeSettingsElement.InstBgm = files[0];
            }
        }

        private void InstBgmRestoreButton_Click(object sender, RoutedEventArgs e)
        {
            ThemeSettingsElement.InstBgm = null;
        }

        private void InstSetupIconButton_Click(object sender, RoutedEventArgs e)
        {
            var files =Funcs.OpenFile(App.FindString("FileTypeIco") + "|*.ico", false);
            if (files != null)
            {
                ThemeSettingsElement.InstSetupIcon = files[0];
            }
        }

        private void InstSetupIconRestoreButton_Click(object sender, RoutedEventArgs e)
        {
            ThemeSettingsElement.InstSetupIcon = null;
        }

        private void InstFontsAddButton_Click(object sender, RoutedEventArgs e)
        {
            Funcs.OpenFilesToInstCollection(ThemeSettingsElement.InstFonts, App.FindString("FileTypeTtf") + "|*.ttf");
        }

        private void InstFontsClearButton_Click(object sender, RoutedEventArgs e)
        {
            ThemeSettingsElement.InstFonts.Clear();
        }

        private void InstStartButtonButton_Click(object sender, RoutedEventArgs e)
        {
            var files =Funcs.OpenFile(App.FindString("FileTypePngBmp") + "|*.png;*.bmp", false);
            if (files == null) return;
            var img = Funcs.GetBitmapImage(files[0]);
            if((img.PixelWidth != 54 || img.PixelHeight!= 162) &&
                (img.PixelWidth != 106 || img.PixelHeight != 318))
            {
                MessageBox.Show(App.FindString("InstStartButtonError"));
                return;
            }
            //WallpaperViewerInfoText.Text = string.Format(App.FindString("TabWallpaperImageInfo"), bitimg.PixelWidth, bitimg.PixelHeight, (new FileInfo(pic)).Length / 1024);
            ThemeSettingsElement.InstStartButton = files[0];
        }

        private void InstStartButtonRestoreButton_Click(object sender, RoutedEventArgs e)
        {
            ThemeSettingsElement.InstStartButton = null;
        }

        private void InstInputSkinAddButton_Click(object sender, RoutedEventArgs e)
        {
            Funcs.OpenFilesToInstCollection(ThemeSettingsElement.InstInputSkin, App.FindString("FileTypeInput") + "|*.ssf;*.qpys");
        }

        private void InstInputSkinClearButton_Click(object sender, RoutedEventArgs e)
        {
            ThemeSettingsElement.InstInputSkin.Clear();
        }

        private void InstOtherFileAddButton_Click(object sender, RoutedEventArgs e)
        {
            Funcs.OpenFilesToInstCollection(ThemeSettingsElement.InstOtherFile, App.FindString("FileTypeAll") + "|*.*");
        }

        private void InstOtherFileClearButton_Click(object sender, RoutedEventArgs e)
        {
            ThemeSettingsElement.InstOtherFile.Clear();
        }

        private void InstBack_Click(object sender, RoutedEventArgs e)
        {
            BoxHideStoryboard.Begin();
        }

        private void InstLogonButton_Click(object sender, RoutedEventArgs e)
        {
            var files = Funcs.OpenFile(App.FindString("FileTypePngJpgGifBmp") + "|*.png;*.jpg;*.gif;*.bmp", false);
            if (files != null)
            {
                ThemeSettingsElement.InstLogonImage = files[0];
            }
        }

        private void InstLogonRestoreButton_Click(object sender, RoutedEventArgs e)
        {
            ThemeSettingsElement.InstLogonImage = null;
        }

        #endregion

        #endregion
    }
}