using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace Win7ThemeEditor
{
    public class Paths
    {
        public static string Windir
        {
            get { return Environment.GetEnvironmentVariable("windir"); }
        }

        public static string DefaultWallpaperFile
        {
            get { return Path.Combine(Windir, @"Web\Wallpaper\Windows\img0.jpg"); }
        }

        public static string SysThemeDir
        {
            get { return Path.Combine(Windir, @"Resources\Themes"); }
        }

        public static string AeroMssFile
        {
            get { return Path.Combine(Windir, @"Resources\Themes\aero\aero.msstyles"); }
        }

        public static string AppTempDir
        {
            get { return Path.Combine(Path.GetTempPath(), @"W7TETemp"); }
        }

        public static string WinrarExeFile
        {
            get { return Path.Combine(AppDir, @"Res\Tools\WinRAR\Rar.exe"); }
        }

        public static string AppDir
        {
            get { return Path.GetDirectoryName(Application.ResourceAssembly.Location); }
        }

        public static string ThemeInstallerExeFile
        {
            get { return Path.Combine(AppDir, @"ThemeInstaller\ThemeInstaller.exe"); }
        }

        public static string AppDataDir
        {
            get { return Environment.GetEnvironmentVariable("APPDATA"); }
        }

        public static string ThisAppDataDir
        {
            get
            {
                var path = Path.Combine(AppDataDir, "Win7ThemeEditor");
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                return path;
            }
        }

        public static string ThisAppSettingFile
        {
            get
            {
                var path = Path.Combine(ThisAppDataDir, "Setting.xml");
                //if (!Directory.Exists(Path.GetDirectoryName(path)))
                //{
                //    Directory.CreateDirectory(Path.GetDirectoryName(path));
                //}
                return path;
            }
        }

        public static string ThisAppLogFile
        {
            get
            {
                var path = Path.Combine(ThisAppDataDir, "DebugLog.txt");
                return path;
            }
        }

    }
}
