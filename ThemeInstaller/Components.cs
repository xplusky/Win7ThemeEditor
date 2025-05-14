using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace ThemeInstaller
{
    public class Components
    {
        public enum Enum
        {
            Theme,
            Fonts,
            Cursors,
            Icon,
            InputSkins,
            StartButton,
            LogonImage,
            Others,
            UninstallFiles,
            DesktopIcons
        }

        public class InstallState
        {
            public bool? IsInstall { get; set; }
            public CheckBox Box { get; set; }
        }

        public static Dictionary<Enum, InstallState> Install = new Dictionary<Enum, InstallState>
        {
            {Enum.Theme, new InstallState {IsInstall = true}},
            {Enum.Fonts, new InstallState {IsInstall = true}},
            {Enum.Cursors, new InstallState {IsInstall = true}},
            {Enum.Icon, new InstallState {IsInstall = true}},
            {Enum.InputSkins, new InstallState {IsInstall = true}},
            {Enum.StartButton, new InstallState {IsInstall = true}},
            {Enum.LogonImage, new InstallState {IsInstall = true}},
            {Enum.Others, new InstallState {IsInstall = true}},
            {Enum.UninstallFiles, new InstallState {IsInstall = true}},
            {Enum.DesktopIcons, new InstallState {IsInstall = true}}
        };

        public static void LinkCheckBox(params CheckBox[] checkBoxs)
        {
            var i = 0;
            foreach (var installState in Install)
            {
                installState.Value.Box = checkBoxs[i];
                i++;
            }
        }

        public static void ApplyCheck()
        {
            foreach (var installState in Install)
            {
                installState.Value.Box.IsChecked = installState.Value.IsInstall;
            }
        }

        public static void SaveCheck()
        {
            foreach (var installState in Install)
            {
                installState.Value.IsInstall = installState.Value.Box.IsChecked;
            }
        }

        public static bool IsExist(Enum com)
        {
            switch (com)
            {
                case Enum.Theme:
                {
                    var themeDir = Path.Combine(Paths.AppDir, @"Contents\Theme");
                    if (!Directory.Exists(themeDir)) return false;
                    bool bdir = false, bfile = false;
                    foreach (var file in new DirectoryInfo(themeDir).GetFiles())
                    {
                        if (!file.Extension.ToLower().Equals(".theme")) continue;
                        bfile = true;
                        break;
                    }
                    foreach (var dir in new DirectoryInfo(themeDir).GetDirectories())
                    {
                        if (!Directory.Exists(dir.FullName)) continue;
                        bdir = true;
                        break;
                    }
                    return bdir && bfile;
                }
                case Enum.InputSkins:
                {
                    foreach (var file in new DirectoryInfo(Path.Combine(Paths.AppDir, @"Contents\Input")).GetFiles())
                    {
                        var ext = file.Extension.ToLower();
                        if (ext == ".ssf" || ext == ".qpys") return true;
                    }
                    return false;
                }
                case Enum.Others:
                {
                    var direcInfo = new DirectoryInfo(@"Contents\Other");
                    var files = direcInfo.GetFiles();
                    foreach (var file in files)
                    {
                        if (file.Name == "Readme.txt") continue;
                        return true;
                    }
                    var dirs = direcInfo.GetDirectories();
                    return dirs.Length != 0;
                }
                case Enum.StartButton:
                {
                    return File.Exists(Setup.Settings["StartButtonPath"].Content);
                }
                case Enum.LogonImage:
                {
                    return File.Exists(Setup.Settings["LogonImagePath"].Content);
                }
                    case Enum.Fonts:
                {
                    foreach (var file in new DirectoryInfo(@"Contents\Fonts").GetFiles())
                    {
                        var ext = file.Extension.ToLower();
                        if (ext.ToLower() == ".ttf") return true;
                    }
                    return false;
                }
            }
            return false;
        }
    }
}
