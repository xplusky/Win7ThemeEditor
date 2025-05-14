#region

using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

#endregion

namespace Win7ThemeEditor
{
    /// <summary>
    /// 主题设置依赖项属性
    /// </summary>
    //[Serializable]
    public class ThemeSettings : FrameworkElement //, ISerializable
    {
        #region ThemeFile

        public static readonly DependencyProperty ThemeDiaplayNmaeProperty = DependencyProperty.Register("ThemeDiaplayName", typeof (string), typeof (ThemeSettings), new PropertyMetadata(App.FindString("TabInfoDispNameText")));

        public static readonly DependencyProperty ThemeFileNmaeProperty = DependencyProperty.Register("ThemeFileName", typeof (string), typeof (ThemeSettings), new PropertyMetadata("My Theme"));

        public static readonly DependencyProperty ThemeFileInfoProperty = DependencyProperty.Register("ThemeFileInfo", typeof (string), typeof (ThemeSettings), new PropertyMetadata(App.FindString("TabInfoInfoText")));

        public static readonly DependencyProperty LogoFilePathProperty = DependencyProperty.Register("LogoFilePath", typeof (string), typeof (ThemeSettings), new PropertyMetadata(string.Empty));


        public static readonly DependencyProperty MsstylesFilePathProperty =
            DependencyProperty.Register("MsstylesFilePath", typeof (string), typeof (ThemeSettings), new PropertyMetadata(Environment.GetEnvironmentVariable("windir") + @"\Resources\Themes\Aero\aero.msstyles",
                (o, e) => MainWindow.UpdateMssFileInfo()));

        public static readonly DependencyProperty IconItemSourceProperty =
            DependencyProperty.Register("IconItemSource", typeof (ObservableCollection<IconItem>), typeof (ThemeSettings), new PropertyMetadata(new ObservableCollection<IconItem>
            {
                new IconItem {SystemIconName = App.FindString("TabIconCom"), IconFilePath = Paths.AppDir + @"\Res\Icon\Computer.png", DefaultIconPath = Path.Combine(Paths.AppDir , @"Res\Icon\Computer.png")},
                new IconItem {SystemIconName = App.FindString("TabIconUser"), IconFilePath = Paths.AppDir + @"\Res\Icon\UserFile.png", DefaultIconPath = Path.Combine(Paths.AppDir , @"Res\Icon\UserFile.png")},
                new IconItem {SystemIconName = App.FindString("TabIconNet"), IconFilePath = Paths.AppDir + @"\Res\Icon\Network.png", DefaultIconPath = Path.Combine(Paths.AppDir , @"Res\Icon\Network.png")},
                new IconItem {SystemIconName = App.FindString("TabIconDustFull"), IconFilePath = Paths.AppDir + @"\Res\Icon\DustFull.png", DefaultIconPath = Path.Combine(Paths.AppDir , @"Res\Icon\DustFull.png")},
                new IconItem {SystemIconName = App.FindString("TabIconDustEmpty"), IconFilePath = Paths.AppDir + @"\Res\Icon\DustEmpty.png", DefaultIconPath = Path.Combine(Paths.AppDir , @"Res\Icon\DustEmpty.png")}
            }));

        // Using a DependencyProperty as the backing store for CursorsItemSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CursorsItemSourceProperty =
            DependencyProperty.Register("CursorsItemSource", typeof (ObservableCollection<CursorItem>), typeof (ThemeSettings), new PropertyMetadata(new ObservableCollection<CursorItem>
            {
                new CursorItem {DefaultCursorImage = "Res/Image/Cursor/AppStarting.png", IniKey = "AppStarting"},
                new CursorItem {DefaultCursorImage = "Res/Image/Cursor/Arrow.png", IniKey = "Arrow"},
                new CursorItem {DefaultCursorImage = "Res/Image/Cursor/Cross.png", IniKey = "Cross"},
                new CursorItem {DefaultCursorImage = "Res/Image/Cursor/Hand.png", IniKey = "Hand"},
                new CursorItem {DefaultCursorImage = "Res/Image/Cursor/Handwriting.png", IniKey = "Handwriting"},
                new CursorItem {DefaultCursorImage = "Res/Image/Cursor/Help.png", IniKey = "Help"},
                new CursorItem {DefaultCursorImage = "Res/Image/Cursor/IBeam.png", IniKey = "IBeam"},
                new CursorItem {DefaultCursorImage = "Res/Image/Cursor/No.png", IniKey = "No"},
                new CursorItem {DefaultCursorImage = "Res/Image/Cursor/SizeAll.png", IniKey = "SizeAll"},
                new CursorItem {DefaultCursorImage = "Res/Image/Cursor/SizeNESW.png", IniKey = "SizeNESW"},
                new CursorItem {DefaultCursorImage = "Res/Image/Cursor/SizeNS.png", IniKey = "SizeNS"},
                new CursorItem {DefaultCursorImage = "Res/Image/Cursor/SizeNWSE.png", IniKey = "SizeNWSE"},
                new CursorItem {DefaultCursorImage = "Res/Image/Cursor/SizeWE.png", IniKey = "SizeWE"},
                new CursorItem {DefaultCursorImage = "Res/Image/Cursor/UpArrow.png", IniKey = "UpArrow"},
                new CursorItem {DefaultCursorImage = "Res/Image/Cursor/Wait.png", IniKey = "Wait"}
            }));


        public static readonly DependencyProperty WallpaperFileItemsSourceProperty = DependencyProperty.Register("WallpaperFileItemsSource", typeof (ObservableCollection<WallpaperFileItem>), typeof (ThemeSettings), new PropertyMetadata(new ObservableCollection<WallpaperFileItem>()));

        public static readonly DependencyProperty SliderShowIntervalIndexProperty =
            DependencyProperty.Register("SliderShowIntervalIndex", typeof (int), typeof (ThemeSettings), new PropertyMetadata(3));

        public static readonly DependencyProperty SliderShowShuffleProperty =
            DependencyProperty.Register("SliderShowShuffle", typeof (bool?), typeof (ThemeSettings), new PropertyMetadata(false));

        public static readonly DependencyProperty ThemeColorProperty =
            DependencyProperty.Register("ThemeColor", typeof (Color), typeof (ThemeSettings), new PropertyMetadata(Color.FromArgb(100, 116, 184, 252)));

        public static readonly DependencyProperty IsUseDefaultSystemSoundProperty =
            DependencyProperty.Register("IsUseDefaultSystemSound", typeof (bool?), typeof (ThemeSettings), new PropertyMetadata(true));

        public static readonly DependencyProperty DefaultSystemSoundIndexProperty =
            DependencyProperty.Register("DefaultSystemSoundIndex", typeof (int), typeof (ThemeSettings), new PropertyMetadata(0));

        public static readonly DependencyProperty SystemSoundItemSourceProperty = DependencyProperty.Register("SystemSoundItemSource", typeof (ObservableCollection<SystemSoundItem>), typeof (ThemeSettings), new PropertyMetadata(
            new ObservableCollection<SystemSoundItem>
            {
                new SystemSoundItem {IniSection = @"AppEvents\Schemes\Apps\.Default\ChangeTheme"},
                new SystemSoundItem {IniSection = @"AppEvents\Schemes\Apps\.Default\WindowsLogoff"},
                new SystemSoundItem {IniSection = @"AppEvents\Schemes\Apps\.Default\WindowsUAC"},
                new SystemSoundItem {IniSection = @"AppEvents\Schemes\Apps\.Default\WindowsLogon"},
                new SystemSoundItem {IniSection = @"AppEvents\Schemes\Apps\.Default\SystemHand"},
                new SystemSoundItem {IniSection = @"AppEvents\Schemes\Apps\.Default\Close"},
                new SystemSoundItem {IniSection = @"AppEvents\Schemes\Apps\.Default\RestoreUp"},
                new SystemSoundItem {IniSection = @"AppEvents\Schemes\Apps\.Default\RestoreDown"},
                new SystemSoundItem {IniSection = @"AppEvents\Schemes\Apps\.Default\MenuPopup"},
                new SystemSoundItem {IniSection = @"AppEvents\Schemes\Apps\.Default\SystemExclamation"},
                new SystemSoundItem {IniSection = @"AppEvents\Schemes\Apps\.Default\PrintComplete"},
                new SystemSoundItem {IniSection = @"AppEvents\Schemes\Apps\.Default\Open"},
                new SystemSoundItem {IniSection = @"AppEvents\Schemes\Apps\.Default\FaxBeep"},
                new SystemSoundItem {IniSection = @"AppEvents\Schemes\Apps\.Default\MailBeep"},
                new SystemSoundItem {IniSection = @"AppEvents\Schemes\Apps\.Default\SystemAsterisk"},
                new SystemSoundItem {IniSection = @"AppEvents\Schemes\Apps\.Default\ShowBand"},
                new SystemSoundItem {IniSection = @"AppEvents\Schemes\Apps\.Default\Maximize"},
                new SystemSoundItem {IniSection = @"AppEvents\Schemes\Apps\.Default\Minimize"},
                new SystemSoundItem {IniSection = @"AppEvents\Schemes\Apps\.Default\LowBatteryAlarm"},
                new SystemSoundItem {IniSection = @"AppEvents\Schemes\Apps\.Default\CriticalBatteryAlarm"},
                new SystemSoundItem {IniSection = @"AppEvents\Schemes\Apps\.Default\AppGPFault"},
                new SystemSoundItem {IniSection = @"AppEvents\Schemes\Apps\.Default\SystemNotification"},
                new SystemSoundItem {IniSection = @"AppEvents\Schemes\Apps\.Default\MenuCommand"},
                new SystemSoundItem {IniSection = @"AppEvents\Schemes\Apps\.Default\DeviceDisconnect"},
                new SystemSoundItem {IniSection = @"AppEvents\Schemes\Apps\.Default\DeviceFail"},
                new SystemSoundItem {IniSection = @"AppEvents\Schemes\Apps\.Default\DeviceConnect"},
                new SystemSoundItem {IniSection = @"AppEvents\Schemes\Apps\.Default\SystemExit"},
                new SystemSoundItem {IniSection = @"AppEvents\Schemes\Apps\.Default\CCSelect"},
                new SystemSoundItem {IniSection = @"AppEvents\Schemes\Apps\.Default\SystemQuestion"},
                new SystemSoundItem {IniSection = @"AppEvents\Schemes\Apps\.Default\.Default"},
            
                //windows资源管理器
                new SystemSoundItem {IniSection = @"AppEvents\Schemes\Apps\Explorer\FaxError"},
                new SystemSoundItem {IniSection = @"AppEvents\Schemes\Apps\Explorer\SecurityBand"},
                new SystemSoundItem {IniSection = @"AppEvents\Schemes\Apps\Explorer\Navigating"},
                new SystemSoundItem {IniSection = @"AppEvents\Schemes\Apps\Explorer\ActivatingDocument"},
                new SystemSoundItem {IniSection = @"AppEvents\Schemes\Apps\Explorer\SearchProviderDiscovered"},
                new SystemSoundItem {IniSection = @"AppEvents\Schemes\Apps\Explorer\FeedDiscovered"},
                new SystemSoundItem {IniSection = @"AppEvents\Schemes\Apps\Explorer\FaxSent"},
                new SystemSoundItem {IniSection = @"AppEvents\Schemes\Apps\Explorer\FaxLineRings"},
                new SystemSoundItem {IniSection = @"AppEvents\Schemes\Apps\Explorer\EmptyRecycleBin"},
                new SystemSoundItem {IniSection = @"AppEvents\Schemes\Apps\Explorer\MoveMenuItem"},
                new SystemSoundItem {IniSection = @"AppEvents\Schemes\Apps\Explorer\BlockedPopup"}
            }));

        public string ThemeDiaplayName
        {
            get { return (string) GetValue(ThemeDiaplayNmaeProperty); }
            set { SetValue(ThemeDiaplayNmaeProperty, value); }
        }

        public string ThemeFileName
        {
            get
            {
                string[] illegalStrings = {";", "\\", "/", "|", "?", "*", ">", "<", ":"};
                var name = (string) GetValue(ThemeFileNmaeProperty);
                foreach (var s in illegalStrings)
                {
                    name = name.Replace(s, string.Empty);
                }
                name = name.Trim();
                if (name == string.Empty || name.ToLower() == "aero") return null;
                return name;
            }
            set { SetValue(ThemeFileNmaeProperty, value); }
        }

        public string ThemeFileInfo
        {
            get { return (string) GetValue(ThemeFileInfoProperty); }
            set { SetValue(ThemeFileInfoProperty, value); }
        }

        public string LogoFilePath
        {
            get { return (string) GetValue(LogoFilePathProperty); }
            set { SetValue(LogoFilePathProperty, value); }
        }

        public string MsstylesFilePath
        {
            get { return (string) GetValue(MsstylesFilePathProperty); }
            set { SetValue(MsstylesFilePathProperty, value); }
        }

        public ObservableCollection<IconItem> IconItemSource
        {
            get { return (ObservableCollection<IconItem>) GetValue(IconItemSourceProperty); }
            set { SetValue(IconItemSourceProperty, value); }
        }

        public ObservableCollection<CursorItem> CursorsItemSource
        {
            get { return (ObservableCollection<CursorItem>) GetValue(CursorsItemSourceProperty); }
            set { SetValue(CursorsItemSourceProperty, value); }
        }

        public ObservableCollection<WallpaperFileItem> WallpaperFileItemsSource
        {
            get { return (ObservableCollection<WallpaperFileItem>) GetValue(WallpaperFileItemsSourceProperty); }
            set { SetValue(WallpaperFileItemsSourceProperty, value); }
        }


        public int SliderShowIntervalIndex
        {
            get { return (int) GetValue(SliderShowIntervalIndexProperty); }
            set { SetValue(SliderShowIntervalIndexProperty, value); }
        }


        public bool? SliderShowShuffle
        {
            get { return (bool?) GetValue(SliderShowShuffleProperty); }
            set { SetValue(SliderShowShuffleProperty, value); }
        }


        public Color ThemeColor
        {
            get { return (Color) GetValue(ThemeColorProperty); }
            set { SetValue(ThemeColorProperty, value); }
        }


        public bool? IsUseDefaultSystemSound
        {
            get { return (bool?) GetValue(IsUseDefaultSystemSoundProperty); }
            set { SetValue(IsUseDefaultSystemSoundProperty, value); }
        }


        public int DefaultSystemSoundIndex
        {
            get { return (int) GetValue(DefaultSystemSoundIndexProperty); }
            set { SetValue(DefaultSystemSoundIndexProperty, value); }
        }


        public ObservableCollection<SystemSoundItem> SystemSoundItemSource
        {
            get { return (ObservableCollection<SystemSoundItem>) GetValue(SystemSoundItemSourceProperty); }
            set { SetValue(SystemSoundItemSourceProperty, value); }
        }

        #region Nested type: CursorItem

        public class CursorItem
        {
            public string DefaultCursorImage { get; set; }
            public string CursorFilePath { get; set; }
            public string IniKey { get; set; }
        }

        #endregion

        #region Nested type: IconItem

        public class IconItem
        {
            public string SystemIconName { get; set; }
            public string IconFilePath { get; set; }
            public bool? IsDefaultIcon { get { return IconFilePath == DefaultIconPath; } }
            public string DefaultIconPath { get; set; }
        }

        #endregion

        #region Nested type: SystemSoundItem

        public class SystemSoundItem
        {
            public string Name
            {
                get
                {
                    string name = IniSection.Remove(0, IniSection.LastIndexOf("\\", StringComparison.Ordinal) + 1).Replace(".", string.Empty);
                    return App.FindString("Sound" + name);
                }
            }

            public string IniSection { get; set; }
            public string Path { get; set; }

            public Visibility Custom
            {
                get { return Path == null ? Visibility.Hidden : Visibility.Visible; }
            }
        }

        #endregion

        #region Nested type: WallpaperFileItem

        public class WallpaperFileItem
        {
            public string ImageFilePath { get; set; }
        }

        #endregion

        #endregion

        #region Sfx

        // Using a DependencyProperty as the backing store for SfxLicence.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SfxLicenceProperty =
            DependencyProperty.Register("SfxLicence", typeof (string), typeof (ThemeSettings), new PropertyMetadata(string.Empty));


        // Using a DependencyProperty as the backing store for SfxReadme.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SfxReadmeProperty =
            DependencyProperty.Register("SfxReadme", typeof (string), typeof (ThemeSettings), new PropertyMetadata(App.FindString("SfxReadmeText")));


        // Using a DependencyProperty as the backing store for SfxIsUseLicence.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SfxIsUseLicenceProperty =
            DependencyProperty.Register("SfxIsUseLicence", typeof (bool?), typeof (ThemeSettings), new PropertyMetadata(false));


        // Using a DependencyProperty as the backing store for SfxTitle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SfxTitleProperty =
            DependencyProperty.Register("SfxTitle", typeof (string), typeof (ThemeSettings), new PropertyMetadata(string.Empty));


        // Using a DependencyProperty as the backing store for SfxRunAfterExtract.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SfxRunAfterExtractProperty =
            DependencyProperty.Register("SfxRunAfterExtract", typeof (bool?), typeof (ThemeSettings), new PropertyMetadata(false));

        public string SfxLicence
        {
            get { return (string) GetValue(SfxLicenceProperty); }
            set { SetValue(SfxLicenceProperty, value); }
        }

        public string SfxReadme
        {
            get { return (string) GetValue(SfxReadmeProperty); }
            set { SetValue(SfxReadmeProperty, value); }
        }

        public bool? SfxIsUseLicence
        {
            get { return (bool?) GetValue(SfxIsUseLicenceProperty); }
            set { SetValue(SfxIsUseLicenceProperty, value); }
        }

        public string SfxTitle
        {
            get { return (string) GetValue(SfxTitleProperty); }
            set { SetValue(SfxTitleProperty, value); }
        }

        public bool? SfxRunAfterExtract
        {
            get { return (bool?) GetValue(SfxRunAfterExtractProperty); }
            set { SetValue(SfxRunAfterExtractProperty, value); }
        }

        #endregion

        #region Installer

        // Using a DependencyProperty as the backing store for InstIsUseEditorTheme.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InstIsUseEditorThemeProperty =
            DependencyProperty.Register("InstIsUseEditorTheme", typeof (bool?), typeof (ThemeSettings), new PropertyMetadata(true));


        // Using a DependencyProperty as the backing store for InstCostomThemeFilePath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InstCostomThemeFilePathProperty =
            DependencyProperty.Register("InstCostomThemeFilePath", typeof (string), typeof (ThemeSettings), new PropertyMetadata(string.Empty));


        // Using a DependencyProperty as the backing store for InstBackgroundImage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InstBackgroundImageProperty =
            DependencyProperty.Register("InstBackgroundImage", typeof (string), typeof (ThemeSettings), new PropertyMetadata(string.Empty));


        // Using a DependencyProperty as the backing store for InstSplashLogo.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InstSplashLogoProperty =
            DependencyProperty.Register("InstSplashLogo", typeof (string), typeof (ThemeSettings), new PropertyMetadata(string.Empty));


        // Using a DependencyProperty as the backing store for InstWelcomeLogo.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InstWelcomeLogoProperty =
            DependencyProperty.Register("InstWelcomeLogo", typeof (string), typeof (ThemeSettings), new PropertyMetadata(string.Empty));


        // Using a DependencyProperty as the backing store for InstSetupIcon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InstSetupIconProperty =
            DependencyProperty.Register("InstSetupIcon", typeof (string), typeof (ThemeSettings), new PropertyMetadata(string.Empty));


        // Using a DependencyProperty as the backing store for InstBgm.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InstBgmProperty =
            DependencyProperty.Register("InstBgm", typeof (string), typeof (ThemeSettings), new PropertyMetadata(string.Empty));


        // Using a DependencyProperty as the backing store for InstTitle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InstTitleProperty =
            DependencyProperty.Register("InstTitle", typeof (string), typeof (ThemeSettings), new PropertyMetadata(App.FindString("TabInfoDispNameText")));


        public static readonly DependencyProperty InstAuthorProperty =
            DependencyProperty.Register("InstAuthor", typeof (string), typeof (ThemeSettings), new PropertyMetadata(string.Empty));


        // Using a DependencyProperty as the backing store for InstWebsite.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InstWebsiteProperty =
            DependencyProperty.Register("InstWebsite", typeof (string), typeof (ThemeSettings), new PropertyMetadata(string.Empty));


        // Using a DependencyProperty as the backing store for InstIsUseLicence.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InstIsUseLicenceProperty =
            DependencyProperty.Register("InstIsUseLicence", typeof (bool?), typeof (ThemeSettings), new PropertyMetadata(false));


        // Using a DependencyProperty as the backing store for InstLicence.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InstLicenceProperty =
            DependencyProperty.Register("InstLicence", typeof (string), typeof (ThemeSettings), new PropertyMetadata(string.Empty));


        // Using a DependencyProperty as the backing store for InstIsUseReadme.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InstIsUseReadmeProperty =
            DependencyProperty.Register("InstIsUseReadme", typeof (bool?), typeof (ThemeSettings), new PropertyMetadata(false));


        // Using a DependencyProperty as the backing store for InstReadme.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InstReadmeProperty =
            DependencyProperty.Register("InstReadme", typeof (string), typeof (ThemeSettings), new PropertyMetadata(string.Empty));


        // Using a DependencyProperty as the backing store for InstFonts.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InstFontsProperty =
            DependencyProperty.Register("InstFonts", typeof (ObservableCollection<ListBoxItem>), typeof (ThemeSettings), new PropertyMetadata(new ObservableCollection<ListBoxItem>()));


        // Using a DependencyProperty as the backing store for InstStartButton.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InstStartButtonProperty =
            DependencyProperty.Register("InstStartButton", typeof (string), typeof (ThemeSettings), new PropertyMetadata(string.Empty));


        // Using a DependencyProperty as the backing store for InstInputSkin.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InstInputSkinProperty =
            DependencyProperty.Register("InstInputSkin", typeof (ObservableCollection<ListBoxItem>), typeof (ThemeSettings), new PropertyMetadata(new ObservableCollection<ListBoxItem>()));


        public static readonly DependencyProperty InstOtherFileProperty =
            DependencyProperty.Register("InstOtherFile", typeof (ObservableCollection<ListBoxItem>), typeof (ThemeSettings), new PropertyMetadata(new ObservableCollection<ListBoxItem>()));


        public static readonly DependencyProperty InstPasswordProperty =
            DependencyProperty.Register("InstPassword", typeof (string), typeof (ThemeSettings), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty InstLogonImageProperty =
            DependencyProperty.Register("InstLogonImage", typeof (string), typeof (ThemeSettings), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty InstThemeVersionProperty =
            DependencyProperty.Register("InstThemeVersion", typeof (string), typeof (ThemeSettings), new PropertyMetadata("1.0"));

        public bool? InstIsUseEditorTheme
        {
            get { return (bool?) GetValue(InstIsUseEditorThemeProperty); }
            set { SetValue(InstIsUseEditorThemeProperty, value); }
        }

        public string InstCostomThemeFilePath
        {
            get { return (string) GetValue(InstCostomThemeFilePathProperty); }
            set { SetValue(InstCostomThemeFilePathProperty, value); }
        }

        public string InstBackgroundImage
        {
            get { return (string) GetValue(InstBackgroundImageProperty); }
            set { SetValue(InstBackgroundImageProperty, value); }
        }

        public string InstSplashLogo
        {
            get { return (string) GetValue(InstSplashLogoProperty); }
            set { SetValue(InstSplashLogoProperty, value); }
        }

        public string InstWelcomeLogo
        {
            get { return (string) GetValue(InstWelcomeLogoProperty); }
            set { SetValue(InstWelcomeLogoProperty, value); }
        }

        public string InstSetupIcon
        {
            get { return (string) GetValue(InstSetupIconProperty); }
            set { SetValue(InstSetupIconProperty, value); }
        }

        public string InstBgm
        {
            get { return (string) GetValue(InstBgmProperty); }
            set { SetValue(InstBgmProperty, value); }
        }

        public string InstTitle
        {
            get { return (string) GetValue(InstTitleProperty); }
            set { SetValue(InstTitleProperty, value); }
        }

        public string InstAuthor
        {
            get { return (string) GetValue(InstAuthorProperty); }
            set { SetValue(InstAuthorProperty, value); }
        }

        public string InstWebsite
        {
            get { return (string) GetValue(InstWebsiteProperty); }
            set { SetValue(InstWebsiteProperty, value); }
        }

        public bool? InstIsUseLicence
        {
            get { return (bool?) GetValue(InstIsUseLicenceProperty); }
            set { SetValue(InstIsUseLicenceProperty, value); }
        }

        public string InstLicence
        {
            get { return (string) GetValue(InstLicenceProperty); }
            set { SetValue(InstLicenceProperty, value); }
        }

        public bool? InstIsUseReadme
        {
            get { return (bool?) GetValue(InstIsUseReadmeProperty); }
            set { SetValue(InstIsUseReadmeProperty, value); }
        }

        public string InstReadme
        {
            get { return (string) GetValue(InstReadmeProperty); }
            set { SetValue(InstReadmeProperty, value); }
        }

        public ObservableCollection<ListBoxItem> InstFonts
        {
            get { return (ObservableCollection<ListBoxItem>) GetValue(InstFontsProperty); }
            set { SetValue(InstFontsProperty, value); }
        }

        public string InstStartButton
        {
            get { return (string) GetValue(InstStartButtonProperty); }
            set { SetValue(InstStartButtonProperty, value); }
        }

        public ObservableCollection<ListBoxItem> InstInputSkin
        {
            get { return (ObservableCollection<ListBoxItem>) GetValue(InstInputSkinProperty); }
            set { SetValue(InstInputSkinProperty, value); }
        }

        public ObservableCollection<ListBoxItem> InstOtherFile
        {
            get { return (ObservableCollection<ListBoxItem>) GetValue(InstOtherFileProperty); }
            set { SetValue(InstOtherFileProperty, value); }
        }

        public string InstPassword
        {
            get { return (string) GetValue(InstPasswordProperty); }
            set { SetValue(InstPasswordProperty, value); }
        }


        public string InstLogonImage
        {
            get { return (string) GetValue(InstLogonImageProperty); }
            set { SetValue(InstLogonImageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for InstLogonImage.  This enables animation, styling, binding, etc...


        public string InstThemeVersion
        {
            get { return (string) GetValue(InstThemeVersionProperty); }
            set { SetValue(InstThemeVersionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for InstThemeVersion.  This enables animation, styling, binding, etc...

        #endregion

    }
}