#region

using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Win32;

#endregion

namespace ThemeInstaller.Pages
{
    /// <summary>
    /// 系统环境检测页面
    /// </summary>
    public partial class EnvironmentSence
    {
        
        public enum PatchStateEnum
        {
            Patched,
            Unknown,
            NotPatched
        }

        public EnvironmentSence()
        {
            InitializeComponent();
            MainWindow.Instance.TitleText.Text = App.GetText("EnviromentTitle");
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // NotSupportGrid
            if (GetWindowsBuildVersion() != 7601)
            {
                SystemThemePatchInfomationGrid.Visibility = Visibility.Collapsed;
                ExplorerFramePatchInfomationGrid.Visibility = Visibility.Collapsed;
                NotSupportGrid.Visibility = Visibility.Visible;
            }

            // Next button
            if (Setup.Mode == Setup.ModeEnum.Crack) BtnNext.IsEnabled = false;

            // System Version
            DetectSystemVersion();

            // theme patched
            if (DetectThemePatchState() == PatchStateEnum.NotPatched)
            {
                ButtonThemeFileCrack.IsEnabled = true;
                ButtonThemeFileCrack.ToolTip = App.GetText("EnviromentClickToPatchTheme");
                ButtonThemeFileCrack.Cursor = Cursors.Hand;
                ButtonThemeFileCrack.Click +=
                    (o, ev) =>
                    {
                        var mbr = MessageBox.Show(App.GetText("EnviromentIfPatchTheme"), "Tip", MessageBoxButton.YesNoCancel);
                        if (mbr != MessageBoxResult.Yes) return;
                        PatchThemeFile();
                        DetectThemePatchState();
                    };
            }

            // explorerframe patch
            if (DetectExplorerFramePatchState() == PatchStateEnum.NotPatched)
            {
                ButtonExplorerFrame.IsEnabled = true;
                ButtonExplorerFrame.ToolTip = App.GetText("EnviromentClickToPatchFrame");
                ButtonExplorerFrame.Cursor = Cursors.Hand;

                ButtonExplorerFrame.Click +=
                    (o, ev) =>
                    {
                        if (DetectExplorerFramePatchState() == PatchStateEnum.Patched)
                            return;
                        var mbr = MessageBox.Show(App.GetText("EnviromentIfPatchFrame"), "Tip", MessageBoxButton.YesNoCancel);
                        if (mbr != MessageBoxResult.Yes) return;
                        PatchExplorerFrameFile();
                        DetectExplorerFramePatchState();
                    };
            }
        }

        private static string GetMd5Hash(string pathName)
        {
            Console.WriteLine("MD5Hash: " + pathName);
            string strResult;
            var oMd5Hasher = new MD5CryptoServiceProvider();
            try
            {
                byte[] arrbytHashValue;
                using (var oFileStream = new FileStream(pathName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) 
                {
                    arrbytHashValue = oMd5Hasher.ComputeHash(oFileStream);
                }
                //由以连字符分隔的十六进制对构成的String，其中每一对表示value 中对应的元素；例如“F-2C-4A”
                var strHashData = BitConverter.ToString(arrbytHashValue);
                //替换-
                strHashData = strHashData.Replace("-", "");
                strResult = strHashData;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
            return strResult;
        }

        //系统版本
        private void DetectSystemVersion()
        {
            TextSystemVersion.Text = string.Format("{0} ({1} bit)", ReadSystemName(), Is64BitOs() ? "64" : "32");
            switch (IsAboveHomeBasic())
            {
                case true:
                    TextSystemVersion.Foreground = Brushes.Green;
                    break;
                case false:
                    TextSystemVersion.Foreground = Brushes.Red;
                    TextSystemVersion.ToolTip = "你的系统版本不适合安装主题，必须高于家庭基本版（包括家庭高级版、专业版、旗舰版）";
                    break;
                case null:
                    TextSystemVersion.Foreground = Brushes.Orange;
                    break;
            }
        }

        private static string ReadSystemName()
        {
            var rootKey = Registry.LocalMachine; //本地计算机数据的配置 
            var runKey = rootKey.CreateSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");
            string value = null;
            try
            {
                if (runKey != null) value = runKey.GetValue("ProductName").ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            rootKey.Close();

            return value;
        }

        private static bool? IsAboveHomeBasic()
        {
            var rootKey = Registry.LocalMachine; //本地计算机数据的配置 
            var runKey = rootKey.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");
            if (runKey == null) return null;
            var value = runKey.GetValue("EditionID").ToString();
            if (string.IsNullOrEmpty(value)) return null;
            rootKey.Close();

            string[] supportVersion =
            {
                "Professional", "Ultimate", "Home Premium", "Enterprise"
            };

            return supportVersion.Any(ver => value == ver);
        }

        public static bool Is64BitOs()
        {
            return (Environment.GetEnvironmentVariable("ProgramFiles(x86)") != null);
        }

        private static int GetWindowsBuildVersion()
        {
            return Environment.OSVersion.Version.Build;
        }

        private PatchStateEnum DetectThemePatchState()
        {
            string themeservice, themeui, uxtheme;
            using (new Wow64ToSys32IntPtr())
            {
                themeservice = GetMd5Hash(Paths.ThemeServiceFile);
                themeui = GetMd5Hash(Paths.ThemeUiFile);
                uxtheme = GetMd5Hash(Paths.UxThemeFile);
            }

            var themeserviceMd5 = Is64BitOs() ?
                "F0344071948D1A1FA732231785A0664C" :
                "42FB6AFD6B79D9FE07381609172E7CA4";
            if (themeservice == themeserviceMd5)
            {
                TextThemeservice.Text = App.GetText("EnviromentNotPatched");
                TextThemeservice.Foreground = Brushes.Red;
            }
            else
            {
                TextThemeservice.Text = App.GetText("EnviromentModify");
                TextThemeservice.Foreground = Brushes.Green;
            }

            var themeuiMd5 = Is64BitOs() ?
                "2C647ABE9A424E55B5F3DAE4629B4277" : 
                "5992A9DF57FD5E6960FDCC2DB69867F7";
            if (themeui == themeuiMd5)
            {
                TextThemeui.Text = App.GetText("EnviromentNotPatched");
                TextThemeui.Foreground = Brushes.Red;
            }
            else
            {
                TextThemeui.Text = App.GetText("EnviromentModify");
                TextThemeui.Foreground = Brushes.Green;
            }

            var uxthemeMd5 = Is64BitOs() ?
                "D29E998E8277666982B4F0303BF4E7AF" : 
                "63BFDF555DA2075A77D677829C3CCCD0";
            if (uxtheme == uxthemeMd5)
            {
                TextUxtheme.Text = App.GetText("EnviromentNotPatched");
                TextUxtheme.Foreground = Brushes.Red;
            }
            else
            {
                TextUxtheme.Text = App.GetText("EnviromentModify");
                TextUxtheme.Foreground = Brushes.Green;
            }

            if (TextThemeservice.Foreground.Equals(Brushes.Red) ||
                TextThemeui.Foreground.Equals(Brushes.Red) ||
                TextUxtheme.Foreground.Equals(Brushes.Red))
            {
                return PatchStateEnum.NotPatched;
            }
            return PatchStateEnum.Patched;
        }


        private static void PatchThemeFile()
        {
            var processFileName = string.Format(@"Tools\ThemePatcher\UniversalThemePatcher-x{0}.exe", Is64BitOs() ? "64" : "86");
            if (!File.Exists(processFileName))
            {
                MessageBox.Show("没有找到破解程序，破解失败");
                return;
            }
            var process = new Process
            {
                StartInfo =
                {
                    Arguments = "-silent",
                    FileName = processFileName
                }
            };
            
            try
            {
                process.Start();
                process.WaitForExit();
                var mr = MessageBox.Show(App.GetText("EnviromentPatchOk"), "Tip", MessageBoxButton.YesNo);
                if (mr == MessageBoxResult.Yes)
                {
                    Process.Start("shutdown.exe", "-r -t 1");
                }
                MainWindow.Instance.EndApp();
            }
            catch (Exception ex)
            {
                Console.WriteLine("破解失败\n" + ex);
            }

        }

        private PatchStateEnum DetectExplorerFramePatchState()
        {
            string ef;
            using (new Wow64ToSys32IntPtr())
            {
                ef = GetMd5Hash(Paths.ExplorerFrameFile);
            }

            var efMd5 = Is64BitOs() ?
                "953B9EB2B2801830F5909747D5F0492B" : 
                "02FEC66A07853D62FCD65628DC0997A3";
            if (ef == efMd5)
            {
                TextExplorerFrame.Text = App.GetText("EnviromentPatchedFrame");
                TextExplorerFrame.Foreground = Brushes.Green;
                return PatchStateEnum.Patched;
            }
            TextExplorerFrame.Text = App.GetText("EnviromentNotPatchedFrame");
            TextExplorerFrame.Foreground = Brushes.Orange;
            return PatchStateEnum.NotPatched;
        }

        private static void PatchExplorerFrameFile()
        {
            using (new Wow64ToSys32IntPtr())
            {
                var changedNameExplorerFrameFilePath = string.Format("{0}.{1}-{2}-{3}-{4}-{5}-{6}.bak",
                    Paths.ExplorerFrameFile, DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day,
                    DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);

                var loacalExplorerFrameFilename = string.Format(@"Tools\ExplorerFramePatcher\{0}Bit\ExplorerFrame.dll", Is64BitOs() ? "64" : "32");
                if (File.Exists(loacalExplorerFrameFilename))
                {
                    if (File.Exists(Paths.ExplorerFrameFile))
                    {
                        GetExplorerFrameFilePermission();
                        try
                        {
                            File.Move(Paths.ExplorerFrameFile, changedNameExplorerFrameFilePath); // Rename
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }

                    }
                    try
                    {
                        File.Copy(loacalExplorerFrameFilename, Paths.ExplorerFrameFile, true);
                        MessageBox.Show(App.GetText("EnviromentFramePatchOk"));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        if (!File.Exists(Paths.ExplorerFrameFile))
                            File.Move(changedNameExplorerFrameFilePath, Paths.ExplorerFrameFile);
                        MessageBox.Show("破解失败\n" + ex);
                    }

                }
                else
                {
                    MessageBox.Show("没有找到破解用的ExplorerFrame,破解失败");
                }
            }
        }

        private static void GetExplorerFrameFilePermission()
        {
            var process = Process.Start(
                "cmd.exe",
                string.Format("/c takeown /f \"{0}\" && icacls \"{1}\" /grant administrators:F", Paths.ExplorerFrameFile, Paths.ExplorerFrameFile));
            if (process != null) process.WaitForExit();
        }

        private void BtnNext_Click(object sender, RoutedEventArgs e)
        {
            if (IsAboveHomeBasic() == false)
            {
                if (MessageBox.Show(App.GetText("EnviromentNoticeSystemVersion"), "Tip", MessageBoxButton.YesNo) == MessageBoxResult.No)
                {
                    return;
                }
            }
            if (DetectThemePatchState() == PatchStateEnum.NotPatched)
            {
                if (MessageBox.Show(App.GetText("EnviromentNoticeNotPatch"), "Tip", MessageBoxButton.YesNo) == MessageBoxResult.No)
                {
                    return;
                }
            }
            if (NavigationService != null) NavigationService.Navigate(FlowPages.NextPage());
        }

        private void BtnPrev_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService != null) NavigationService.Navigate(FlowPages.PrevPage());
        }
    }
}