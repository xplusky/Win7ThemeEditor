#region

using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Xml;
using IWshRuntimeLibrary;
using Microsoft.Win32;
using ThemeInstaller.LogonImage;
using File = System.IO.File;

#endregion

namespace ThemeInstaller.Pages
{
    public partial class InstallingSence
    {
        public static string InstallFailMessage = string.Empty;

        public InstallingSence()
        {
            InitializeComponent();
            MainWindow.ChangeTitle("InstallingTitle");
        }

        public static void AddFailMessage(Exception ex, string title)
        {
            var str = string.Format("=={0}==\r\n{1}\r\n", title, ex);
            InstallFailMessage += str;
            Console.WriteLine(str);
        }

        private void InstallingSence_Loaded(object sender, RoutedEventArgs e)
        {
            InstallFailMessage = string.Empty;

            MainWindow.Instance.ButtonInfo.IsEnabled = false;
            MainWindow.Instance.ButtonClose.IsEnabled = false;

            Action<object> act;
            switch (Setup.Mode)
            {
                case Setup.ModeEnum.Install:
                    act = o => InstallFiles();
                    break;
                case Setup.ModeEnum.Uninstall:
                    act = o => UninstallFiles();
                    break;
                case Setup.ModeEnum.ApplyLogonImage:
                    act = o => ApplyLogonImage();
                    break;
                default:
                    if (NavigationService != null) NavigationService.Navigate(FlowPages.NextPage());
                    return;
            }

            act.BeginInvoke(act,
                ac =>
                {
                    act.EndInvoke(ac);
                    Dispatcher.BeginInvoke(new Action(
                        () =>
                        {
                            MainWindow.Instance.ButtonInfo.IsEnabled = true;
                            MainWindow.Instance.ButtonClose.IsEnabled = true;
                            if (NavigationService != null) NavigationService.Navigate(FlowPages.NextPage());
                        }));
                }
                , null);


        }

        #region Install

        private static void InstallFiles()
        {
            // 主题
            if (Components.Install[Components.Enum.Theme].IsInstall == true)
            {
                try
                {
                    CopyDirectory(Path.Combine(Paths.AppDir, @"Contents\Theme"), Paths.SysThemeDir);

                    // Cursors
                    if (Components.Install[Components.Enum.Cursors].IsInstall == false)
                    {
                        try
                        {
                            CancelCursorsIni();
                        }
                        catch (Exception ex)
                        {
                            AddFailMessage(ex, "Cursor Cancel Fail");
                        }
                    }

                    // Icon
                    if (Components.Install[Components.Enum.Icon].IsInstall == false)
                    {
                        try
                        {
                            CancelIconIni();
                        }
                        catch (Exception ex)
                        {
                            AddFailMessage(ex, "Icon Cancel Fail");
                        }
                    }
                }
                catch (Exception ex)
                {
                    AddFailMessage(ex, "Theme Install Fail");
                }
            }

            // Fonts
            if (Components.Install[Components.Enum.Fonts].IsInstall == true)
            {
                try
                {
                    foreach (var file in new DirectoryInfo(Path.Combine(Paths.AppDir , @"Contents\Fonts")).GetFiles())
                    {
                        var extension = Path.GetExtension(file.FullName);
                        if (extension != null && extension.ToLower() == ".ttf")
                        {
                            InstallFont(file.Name, file.Name);
                        }
                    }
                }
                catch (Exception ex)
                {
                    AddFailMessage(ex, "Font Install Fail");
                }
            }

            // inputskin
            if (Components.Install[Components.Enum.InputSkins].IsInstall == true)
            {
                try
                {
                    CopyDirectory(Path.Combine(Paths.AppDir, @"Contents\Input"), Path.Combine(Paths.ThisThemeContentInSysDir, "Input"));
                }
                catch (Exception ex)
                {
                    AddFailMessage(ex, "Input Skin Install Fail");
                }
            }

            // logon image
            if (Components.Install[Components.Enum.LogonImage].IsInstall == true)
            {
                try
                {
                    var path = Setup.Settings["LogonImagePath"].Content;
                    if (File.Exists(path))
                        Function.Apply(path);
                }
                catch (Exception ex)
                {
                    AddFailMessage(ex, "Logon Image Install Fail");
                }
            }

            // others
            if (Components.Install[Components.Enum.Others].IsInstall == true)
            {
                try
                {
                    CopyDirectory(Path.Combine(Paths.AppDir, @"Contents\Other"), 
                        Path.Combine(Paths.ThisThemeContentInSysDir, "Other"));
                }
                catch (Exception ex)
                {
                    AddFailMessage(ex, "Other Files Install Fail");
                }
            }

            // start button
            if (Components.Install[Components.Enum.StartButton].IsInstall == true)
            {
                try
                {
                    var sbsysdir = Path.Combine(Paths.ThisThemeContentInSysDir, "StartButton");
                    if (!Directory.Exists(sbsysdir)) Directory.CreateDirectory(sbsysdir);
                    var file = new FileInfo(Setup.Settings["StartButtonPath"].Content);
                    file.CopyTo(Path.Combine(sbsysdir, file.Name), true);
                    var w7Soc = new FileInfo(Path.Combine(Paths.AppDir, @"Tools\W7SOC\W7SOCV5.exe"));
                    w7Soc.CopyTo(Path.Combine(sbsysdir, w7Soc.Name), true);
                }
                catch (Exception ex)
                {
                    AddFailMessage(ex, "Start Button Install Fail");
                }
            }

            // Uninstall
            try
            {
                CreatUninstallInfo();
            }
            catch (Exception ex)
            {
                AddFailMessage(ex, "Install Uninstall Files Fail");
                Console.WriteLine(ex);
            }

            // Shortcut
            if (Components.Install[Components.Enum.DesktopIcons].IsInstall == true)
            {
                var deskDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), Setup.Settings["Title"].Content);
                CreatShortcuts(deskDir);
            }

            // Set File Sheild
            new DirectoryInfo(Paths.ThisThemeContentInSysDir).GetFiles()
                .Where(info => info.Extension.ToLower() == ".dll" || info.Extension.ToLower() == ".xml").ToList()
                .ForEach(
                    info =>
                    {
                        info.Attributes |= FileAttributes.Hidden;
                    });
            string[] noInvisibleDir =
            {
                "Cursors", "Icon", "Sound", "Wallpapers", "StartButton", "Other", "Input" ,
                "en-US","ja-JP","Shell","zh-CN","zh-TW"
            };
            new DirectoryInfo(Paths.ThisThemeContentInSysDir).GetDirectories()
                .Where(info => noInvisibleDir.All(s => !String.Equals(s, info.Name, StringComparison.CurrentCultureIgnoreCase))).ToList()
                .ForEach(
                    info =>
                    {
                        info.Attributes |= FileAttributes.Hidden;
                    });

        }

        private static void InstallFont(string fontFileName, string fontName)
        {
            var winFontDir = Path.Combine(Paths.WinDir, "fonts");
            //const int WM_FONTCHANGE = 0x001D;
            //const int HWND_BROADCAST = 0xffff;
            var fontPath = Path.Combine(winFontDir, fontFileName);
            if (File.Exists(fontPath)) return;
            File.Copy(Path.Combine(Paths.AppDir, @"Contents\Fonts\" + fontFileName), fontPath);
            External.AddFontResource(fontPath);

            //Res = SendMessage(HWND_BROADCAST, WM_FONTCHANGE, 0, 0);
            //WIN7下编译这句会出错，不知道是不是系统的问题，这里应该是发送一个系统消息关系不大不影响字体安装，所以我注释掉了
            External.WriteProfileString("fonts", fontName + "(TrueType)", fontFileName);
        }

        private static void CancelCursorsIni()
        {
            var ini = new IniHelper(Paths.ThisThemeInSysFile);
            const string st = @"Control Panel\Cursors";
            ini.WriteString(st, "AppStarting", @"%SystemRoot%\cursors\aero_working.ani");
            ini.WriteString(st, "Arrow", @"%SystemRoot%\cursors\aero_arrow.cur");
            ini.WriteString(st, "Crosshair", @"");
            ini.WriteString(st, "Hand", @"%SystemRoot%\cursors\aero_link.cur");
            ini.WriteString(st, "Help", @"%SystemRoot%\cursors\aero_helpsel.cur");
            ini.WriteString(st, "IBeam", @"");
            ini.WriteString(st, "No", @"%SystemRoot%\cursors\aero_unavail.cur");
            ini.WriteString(st, "NWPen", @"%SystemRoot%\cursors\aero_pen.cur");
            ini.WriteString(st, "SizeAll", @"%SystemRoot%\cursors\aero_move.cur");
            ini.WriteString(st, "SizeNESW", @"%SystemRoot%\cursors\aero_nesw.cur");
            ini.WriteString(st, "SizeNS", @"%SystemRoot%\cursors\aero_ns.cur");
            ini.WriteString(st, "SizeNWSE", @"%SystemRoot%\cursors\aero_nwse.cur");
            ini.WriteString(st, "SizeWE", @"%SystemRoot%\cursors\aero_ew.cur");
            ini.WriteString(st, "UpArrow", @"%SystemRoot%\cursors\aero_up.cur");
            ini.WriteString(st, "Wait", @"%SystemRoot%\cursors\aero_busy.ani");
            ini.WriteString(st, "DefaultValue", @"Windows Aero");
            ini.WriteString(st, "DefaultValue.MUI", @"@main.cpl,-1020");
        }

        private static void CancelIconIni()
        {
            var ini = new IniHelper(Paths.ThisThemeInSysFile);
            string[] secs =
            {
                "{20D04FE0-3AEA-1069-A2D8-08002B30309D}", "{59031A47-3F72-44A7-89C5-5595FE6B30EE}",
                "{F02C1A0D-BE21-4350-88B0-7367FC96EF3C}", "{645FF040-5081-101B-9F08-00AA002F954E}",
                "{645FF040-5081-101B-9F08-00AA002F954E}"
            };
            string[] vals =
            {
                "DefaultValue", "DefaultValue",
                "DefaultValue", "Full",
                "Empty"
            };
            string[] nums =
            {
                "109", "123", "25", "54", "55"
            };

            for (var i = 0; i < 5; i++)
            {
                ini.WriteString(string.Format(@"CLSID\{0}\DefaultIcon",secs[i]), vals[i], string.Format(@"%SystemRoot%\System32\imageres.dll,-{0}", nums[i]));
            }
        }

        private static void CreatShortcuts(string dirPath)
        {
            if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);
            var title = Setup.Settings["Title"].Content; 

            // 卸载快捷图标
            if (Components.Install[Components.Enum.UninstallFiles].IsInstall == true)
            {
                var scName = Path.Combine(dirPath, string.Format("{0} {1}.lnk", App.GetText("Uninstall"), title));
                if (File.Exists(scName)) File.Delete(scName);
                var sc = (IWshShortcut)(new WshShell().CreateShortcut(scName));
                sc.TargetPath = Path.Combine(Paths.ThisThemeContentInSysDir, "Uninstall.exe");
                //sc.Description = "";
                var ico = Path.Combine(Paths.ThisThemeContentInSysDir, "icon.ico");
                if (File.Exists(ico)) sc.IconLocation = ico;
                sc.Save();
            }

            // 网站快捷图标
            if (!string.IsNullOrEmpty(Setup.Settings["Website"].Content))
            {
                var scName = Path.Combine(dirPath, App.GetText("UninstallStartUrl") + ".lnk");
                if (File.Exists(scName)) File.Delete(scName);
                var sc = (IWshShortcut)(new WshShell().CreateShortcut(scName));
                sc.TargetPath = @"%HOMEDRIVE%/Program Files\Internet Explorer\IEXPLORE.EXE";
                sc.Arguments = Setup.Settings["Website"].Content;
                sc.IconLocation = @"%HOMEDRIVE%/Program Files\Internet Explorer\IEXPLORE.EXE, 0";
                sc.Save();
            }

            // 主题应用快捷图标
            {
                var scName = Path.Combine(dirPath, "应用" + title + ".lnk");
                if (File.Exists(scName)) File.Delete(scName);
                var sc = (IWshShortcut)(new WshShell().CreateShortcut(scName));
                sc.TargetPath = Paths.ThisThemeInSysFile;
                sc.Save();
            }

            
            if (Components.Install[Components.Enum.StartButton].IsInstall == true)
            {
                // 应用开始图标
                {
                    var name = new FileInfo(Setup.Settings["StartButtonPath"].Content).Name;
                    var path = Path.Combine(Path.Combine(Paths.ThisThemeContentInSysDir, "StartButton"), name);
                    var scName = Path.Combine(dirPath, "更换开始图标" + ".lnk");
                    if (File.Exists(scName)) File.Delete(scName);
                    var sc = (IWshShortcut)(new WshShell().CreateShortcut(scName));
                    sc.TargetPath = Path.Combine(Paths.ThisThemeContentInSysDir, @"StartButton\W7SOCV5.exe");
                    sc.Arguments = "\"" + path + "\"";
                    sc.Save();
                }

                // 还原开始图标
                {
                    var scName = Path.Combine(dirPath, "还原开始图标" + ".lnk");
                    if (File.Exists(scName)) File.Delete(scName);
                    var sc = (IWshShortcut)(new WshShell().CreateShortcut(scName));
                    sc.TargetPath = Path.Combine(Paths.ThisThemeContentInSysDir, @"StartButton\W7SOCV5.exe");
                    sc.Save();
                }
            }

            // 应用登录背景
            if (Components.Install[Components.Enum.LogonImage].IsInstall == true)
            {
                //var name = new FileInfo(Setup.Settings["LogonImagePath"].Content).Name;
                //var path = Path.Combine(Path.Combine(Paths.ThisThemeContentInSysDir, @"Contents\Files"), name);
                var scName = Path.Combine(dirPath, "应用登录背景图片（请在应用主题后使用）" + ".lnk");
                if (File.Exists(scName)) File.Delete(scName);
                var sc = (IWshShortcut)(new WshShell().CreateShortcut(scName));
                sc.TargetPath = Path.Combine(Paths.ThisThemeContentInSysDir, "Uninstall.exe");
                sc.Arguments = "/applylogonimage";
                sc.Save();
            }
        }

        private static void CreatUninstallInfo()
        {
            var title = Setup.Settings["Title"].Content;

            // 复制卸载文件
            try
            {
                CopyFolderUninstall(Paths.AppDir, Paths.ThisThemeContentInSysDir);
                var unistall = new FileInfo(Path.Combine(Paths.ThisThemeContentInSysDir, "Uninstall.exe"));
                if (unistall.Exists) unistall.Delete();
                File.Move(Path.Combine(Paths.ThisThemeContentInSysDir, "ThemeInstaller.exe"),
                    Path.Combine(Paths.ThisThemeContentInSysDir, "Uninstall.exe"));
                var setup = new FileInfo(Path.Combine(Paths.ThisThemeContentInSysDir, "setup.exe"));
                if (setup.Exists) setup.Delete();
            }
            catch (Exception ex)
            {
                AddFailMessage(ex, "Uninstall Error");
                return;
            }

            // xml save
            try
            {
                var xml = new XmlDocument();
                var xmlPath = Path.Combine(Paths.ThisThemeContentInSysDir, "SetupConfig.xml");
                xml.Load(xmlPath);
                var root = xml.SelectSingleNode("Settings");
                if (root != null)
                {
                    foreach (XmlNode node in root.ChildNodes)
                    {
                        if (node.Name == "SetupMode") node.InnerText = "Uninstall";
                        if (node.Name == "UninstallThemeFileName") node.InnerText = new FileInfo(Paths.ThisThemeInSysFile).Name;
                        if (node.Name == "UninstallThemeContentDirName") node.InnerText = new DirectoryInfo(Paths.ThisThemeContentInSysDir).Name;
                    }
                }
                xml.Save(xmlPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            if (Components.Install[Components.Enum.UninstallFiles].IsInstall == true)
            {
                // 添加到注册表(添加删除程序)
                try
                {
                    var uninstKey = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\" + title);
                    if (uninstKey != null)
                    {
                        uninstKey.SetValue("DisplayName", title);
                        uninstKey.SetValue("DisplayVersion", Setup.Settings["ThemeVersion"].Content);
                        uninstKey.SetValue("InstallLocation", Paths.ThisThemeInSysFile);
                        uninstKey.SetValue("UninstallString", Path.Combine(Paths.ThisThemeContentInSysDir, "Uninstall.exe"));
                        var icon = new FileInfo(Path.Combine(Paths.ThisThemeContentInSysDir, "icon.ico"));
                        if (icon.Exists) uninstKey.SetValue("DisplayIcon", icon.FullName);
                        uninstKey.SetValue("Publisher", Setup.Settings["Author"].Content);
                        uninstKey.SetValue("URLInfoAbout", Setup.Settings["Website"].Content);
                    }
                }
                catch (Exception ex)
                {
                    AddFailMessage(ex, "Reg Error");
                }

                // 创建开始菜单
                try
                {
                    var progDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Programs), title);
                    CreatShortcuts(progDir);

                }
                catch (Exception ex)
                {
                    AddFailMessage(ex, "Start Menu Error!");
                }
            }
            
        }

        #endregion

        #region Uninstall
        
        private static void UninstallFiles()
        {
            // 删除注册表信息
            var uninstallKey = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall");
            try
            {
                if (uninstallKey != null)
                    uninstallKey.DeleteSubKey(Setup.Settings["Title"].Content);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex + "UninstallRegKey not exist");
            }

            // 删除开始菜单
            var progDir = new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Programs), Setup.Settings["Title"].Content));
            if (progDir.Exists)
            {
                try
                {
                    progDir.Delete(true);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            // 删除桌面快捷方式
            var deskDir = new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), Setup.Settings["Title"].Content));
            if (deskDir.Exists)
            {
                try
                {
                    deskDir.Delete(true);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        public static void CopyFolderUninstall(string direcSource, string direcTarget)
        {
            if (!Directory.Exists(direcTarget))
                Directory.CreateDirectory(direcTarget);

            var direcInfo = new DirectoryInfo(direcSource);
            var files = direcInfo.GetFiles();
            foreach (var file in files)
            {
                try
                {
                    if (file.Name == "Readme.txt") continue;
                    if (file.Name == "setup.exe") continue;

                    file.CopyTo(Path.Combine(direcTarget, file.Name), true);
                }
                catch (Exception ex)
                {
                    AddFailMessage(ex, "File Copy Fail");
                }
            }

            var direcInfoArr = direcInfo.GetDirectories();
            foreach (var dir in direcInfoArr)
            {
                Console.WriteLine(dir.Name);
                if (dir.Name == "Theme")
                {
                    continue;
                }
                CopyFolderUninstall(Path.Combine(direcSource, dir.Name), Path.Combine(direcTarget, dir.Name));
            }
        }

        #endregion

        public static void ApplyLogonImage()
        {
            try
            {
                var path = Setup.Settings["LogonImagePath"].Content;
                if (File.Exists(path))
                    Function.Apply(path);
            }
            catch (Exception ex)
            {
                AddFailMessage(ex, "Logon Image Install Fail");
            }
        }

        public static void DeleteFolder(string dirPath)
        {
            if (!Directory.Exists(dirPath)) return;
            var direcInfo = new DirectoryInfo(dirPath);
            foreach (var file in direcInfo.GetFiles())
            {
                try
                {
                    if (file.Attributes.ToString().IndexOf("ReadOnly", StringComparison.Ordinal) != -1)
                        file.Attributes = FileAttributes.Normal;
                    file.Delete();
                    Console.WriteLine(file.FullName + " Deleted");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(file.FullName + " " + ex.Message);
                }
            }
            foreach (var dir in direcInfo.GetDirectories())
            {
                DeleteFolder(dir.FullName);
                try
                {
                    dir.Delete(true);
                    Console.WriteLine(dir.FullName + " Deleted");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(dir.FullName + " " + ex.Message);
                }
            }
        }
        public static void CopyDirectory(string from, string to)
        {
            if (!Directory.Exists(to)) Directory.CreateDirectory(to);

            var direcInfo = new DirectoryInfo(from);
            var files = direcInfo.GetFiles();
            foreach (var file in files)
            {
                try
                {
                    if (file.Name != "Readme.txt")
                        file.CopyTo(Path.Combine(to, file.Name), true);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex + "File Copy Fail");
                    //if (!isKillOnce)
                    //{
                    //    var process = Process.GetProcessesByName("explorer");
                    //    foreach (Process process1 in process)
                    //    {
                    //        try { process1.Kill(); }
                    //        catch { }
                    //        isKillOnce = true;
                    //    }
                    //    while (Process.GetProcessesByName("explorer").Length == 0) ;
                    //    Thread.Sleep(1500);
                    //    InstallFiles();
                    //}
                    //else
                    //{
                    //    ComponentSence.ErrorInfo = "对不起，安装错误，可能是因为主题文件被系统占用\r\n" + ex.ToString();
                    //    ComponentSence.IsError = true;
                    //    return;
                    //}
                }
            }

            var direcInfoArr = direcInfo.GetDirectories();
            foreach (var dir in direcInfoArr)
            {
                CopyDirectory(Path.Combine(from, dir.Name), Path.Combine(to, dir.Name));
            }
        }

    }
}