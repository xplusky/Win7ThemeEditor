using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace ThemeInstaller
{
    public class Paths
    {
        public static string WinDir
        {
            get { return Environment.GetEnvironmentVariable("windir"); }
        }

        public static string SysThemeDir
        {
            get { return Path.Combine(Environment.GetEnvironmentVariable("windir"), @"Resources\Themes"); }
        }

        public static string WinSys32Dir
        {
            get { return Path.Combine(WinDir, "System32"); }
        }

        public static string ExplorerFrameFile
        {
            get { return Path.Combine(WinSys32Dir, "ExplorerFrame.dll"); }
        }

        public static string ThemeServiceFile
        {
            get { return Path.Combine(WinSys32Dir, "themeservice.dll"); }
        }

        public static string ThemeUiFile
        {
            get { return Path.Combine(WinSys32Dir, "themeui.dll"); }
        }

        public static string UxThemeFile
        {
            get { return Path.Combine(WinSys32Dir, "uxtheme.dll"); }
        }

        public static string AppDir
        {
            get { return Path.GetDirectoryName(Application.ResourceAssembly.Location); }
        }

        public static string ThisThemeContentInSysDir { get; set; }

        public static string ThisThemeInSysFile { get; set; }

        public static string SetupXmlFile
        {
            get { return Path.Combine(AppDir, "SetupConfig.xml"); }
        }

    }
}
