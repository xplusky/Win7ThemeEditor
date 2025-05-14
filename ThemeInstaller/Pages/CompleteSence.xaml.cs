#region

using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;

#endregion

namespace ThemeInstaller.Pages
{
    /// <summary>
    /// 完成页面
    /// </summary>
    public partial class CompleteSence
    {
        public static bool IsSetupComplete = false;
        public CompleteSence()
        {
            InitializeComponent();
            switch (Setup.Mode)
            {
                case Setup.ModeEnum.Install:
                    MainWindow.ChangeTitle("CompleteTitle");break;
                case Setup.ModeEnum.Uninstall:
                    MainWindow.ChangeTitle("UninstallCompleteTitle"); break;
                case Setup.ModeEnum.ApplyLogonImage:
                    MainWindow.Instance.TitleText.Text = "更换登录背景成功"; break;
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (CheckBox box in CheckBoxPanel.Children)
            {
                box.IsChecked = false;
                box.Visibility = Visibility.Collapsed;
            }

            switch (Setup.Mode)
            {
                case Setup.ModeEnum.Install:
                    CheckboxApplyTheme.IsChecked = true;
                    CheckboxApplyTheme.Visibility = Visibility.Visible;
                    if (Components.Install[Components.Enum.InputSkins].IsInstall == true)
                    {
                        CheckboxApplyInput.IsChecked = true;
                        CheckboxApplyInput.Visibility = Visibility.Visible;
                    }
                    if (!string.IsNullOrEmpty(Setup.Settings["Website"].Content))
                    {
                        CheckboxVisitWebsite.IsChecked = true;
                        CheckboxVisitWebsite.Visibility = Visibility.Visible;
                    }
                    if (Components.Install[Components.Enum.StartButton].IsInstall == true)
                    {
                        CheckboxApplyStartButton.IsChecked = false;
                        CheckboxApplyStartButton.Visibility = Visibility.Visible;
                    }
                    break;
                case Setup.ModeEnum.Uninstall:
                    if (!string.IsNullOrEmpty(Setup.Settings["Website"].Content))
                    {
                        CheckboxVisitWebsite.IsChecked = true;
                        CheckboxVisitWebsite.Visibility = Visibility.Visible;
                    }
                    Application.Current.Exit += (o, args) => DeleteTheme();
                    break;
                case Setup.ModeEnum.ApplyLogonImage:

                    break;
            }
        }


        private void BtnOkClick(object sender, RoutedEventArgs e)
        {
            
            //visit web
            if (CheckboxVisitWebsite.IsChecked == true)
            {
                try
                {
                    Process.Start("explorer.exe", Setup.Settings["Website"].Content);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                
            }

            //apply input
            if (CheckboxApplyInput.IsChecked == true)
            {
                //sougou
                string s = null;
                var sougouKey = Registry.ClassesRoot.OpenSubKey(@"SogouSkinFile\Shell\Open\Command");
                if (sougouKey != null) s = sougouKey.GetValue(string.Empty).ToString().ToLower();
                //qq
                string q = null;
                var qqPinyinKey = Registry.ClassesRoot.OpenSubKey(@"QQPinyin\Shell\Open\Command");
                if (qqPinyinKey != null) q = qqPinyinKey.GetValue(string.Empty).ToString().ToLower();

                var cantOpenWithShell = false;

                var direcInfo = new DirectoryInfo(Path.Combine(Paths.ThisThemeContentInSysDir, "Input"));
                foreach (var file in direcInfo.GetFiles())
                {
                    var extension = Path.GetExtension(file.FullName);
                    if (extension == string.Empty) continue;
                    var ext = extension.ToLower();
                    if (ext == ".ssf"  )
                    {
                        if (s != null) Process.Start("explorer.exe", file.FullName);
                        else cantOpenWithShell = true;
                    }
                    if(ext == ".qpys")
                    {
                        if (q != null) Process.Start("explorer.exe", file.FullName);
                        else cantOpenWithShell = true;
                    }
                }
                if (cantOpenWithShell) Process.Start("explorer.exe", direcInfo.FullName);
            }

            //apply start button
            if (CheckboxApplyStartButton.IsChecked == true && Setup.Settings["StartButtonPath"].Content != null)
            {
                Registry.SetValue(@"HKEY_CURRENT_USER\Software\W7SOC", "Method", "Resource");
                var w7Soc =
                    new Process
                    {
                        StartInfo =
                        {
                            Arguments =
                                string.Format("\"{0}\"",
                                    Path.Combine(Path.Combine(Paths.ThisThemeContentInSysDir, "StartButton"),
                                        Path.GetFileName(Setup.Settings["StartButtonPath"].Content))),
                            WorkingDirectory = Path.Combine(Paths.ThisThemeContentInSysDir, "StartButton"),
                            FileName = Path.Combine(Paths.ThisThemeContentInSysDir, @"StartButton\W7SOCV5.exe")
                        }
                    };
                w7Soc.Start();
                w7Soc.WaitForExit();
            }

            //aplly theme
            if (CheckboxApplyTheme.IsChecked == true)
            {
                Process.Start("explorer.exe", Paths.ThisThemeInSysFile);
            }

            MainWindow.Instance.EndApp();
        }

        public static void DeleteTheme()
        {
            try
            {
                File.Delete(Paths.ThisThemeInSysFile);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            var bat = new FileInfo(Path.Combine(Paths.ThisThemeContentInSysDir, "deltheme.bat"));
            using (var sw = new StreamWriter(bat.FullName, false, Encoding.Default))
            {
                sw.Write("ping -n 1 127.0.0.1 \r\n rd /s /q \"{0}\"", bat.DirectoryName);
            }
            var process = new Process
            {
                StartInfo =
                {
                    FileName = bat.FullName,
                    WorkingDirectory = Paths.SysThemeDir
                }
            };
            process.Start();
        }
    }
}