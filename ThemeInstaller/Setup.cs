using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Windows;
using System.Xml;
using ThemeInstaller.Pages;

namespace ThemeInstaller
{
    public class Setup
    {
        public class Property
        {
            public string Tip { get; set; }
            public string Content { get; set; }
        }

        public static Dictionary<string, Property> Settings { get; set; }

        public enum ModeEnum
        {
            Install,
            Uninstall,
            ApplyLogonImage,
            Crack
        }

        public static ModeEnum Mode { get; set; }

        public static bool LoadXml()
        {
            if (Settings == null) Settings = new Dictionary<string, Property>();
            var xml = new XmlDocument();
            xml.Load(Paths.SetupXmlFile);
            var setRoot = xml.SelectSingleNode("/Settings");
            if (setRoot == null) return false;
            foreach (XmlNode node in setRoot.ChildNodes)
            {
                var setupSetting = new Property();
                if (node.Attributes != null && node.Attributes["Tip"] != null)
                {
                    setupSetting.Tip = node.Attributes["Tip"].Value;
                }
                setupSetting.Content = node.InnerText;
                Settings.Add(node.Name, setupSetting);
            }
            if (Environment.CommandLine.Contains("/applylogonimage"))
            {
                Mode = ModeEnum.ApplyLogonImage;
                return true;
            }
            var modeNode = Settings["SetupMode"].Content;
            
            switch (modeNode)
            {
                case "Install":
                    Mode = ModeEnum.Install;
                    var themeDirPath = Path.Combine(Paths.AppDir, @"Contents\Theme");
                    if (!Directory.Exists(themeDirPath)) break;
                    foreach (var dir in new DirectoryInfo(themeDirPath).GetDirectories())
                    {
                        Paths.ThisThemeContentInSysDir = Path.Combine(Paths.SysThemeDir, dir.Name);
                        break;
                    }
                    foreach (var file in new DirectoryInfo(themeDirPath).GetFiles().Where(file => file.Extension.ToLower().Equals(".theme"))) 
                    {
                        Paths.ThisThemeInSysFile = Path.Combine(Paths.SysThemeDir, file.Name);
                        break;
                    }
                    break;
                case "Uninstall":
                    Mode = ModeEnum.Uninstall;
                    Paths.ThisThemeContentInSysDir = Path.Combine(Paths.SysThemeDir, Settings["UninstallThemeContentDirName"].Content);
                    Paths.ThisThemeInSysFile = Path.Combine(Paths.SysThemeDir, Settings["UninstallThemeFileName"].Content);
                    break;
                case "Crack":
                    Mode = ModeEnum.Crack;
                    break;
                default:
                    return false;
            }
            return true;

        }


        public static bool DetectErrors()
        {
            var b = false;
            NotSupportSence.Infos.Clear();
            var version = Environment.OSVersion.Version;
            if (version.Major != 6 || version.Minor != 1)
            {
                NotSupportSence.Infos.Add("只能安装在win7上");
                b = true;
            }

            var title = Settings["Title"].Content;
            if (title.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            {
                NotSupportSence.Infos.Add("名称含有非法字符");
                b = true;
            }

            if (!title.Trim().Equals(title))
            {
                NotSupportSence.Infos.Add("名称首尾不能含有空格");
                b = true;
            }

            if (title.Length < 5)
            {
                NotSupportSence.Infos.Add("名称必须大于五个字符");
                b = true;
            }
            return b;
        }
    }

    
}
