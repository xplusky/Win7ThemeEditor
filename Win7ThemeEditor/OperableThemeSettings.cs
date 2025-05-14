#region

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using Win7ThemeEditor.Helper;

#endregion

namespace Win7ThemeEditor
{
    internal class OperableThemeSettings : ThemeSettings
    {
        private readonly string[] _systemWaveDefaultNum =
            {
                "800", "801", "810", "811", "812", "813", "814", "815", "816"
                , "817", "818", "819", "820", "821", "822"
            };

        private readonly int[] _wallpaperSliderTimes =
            {
                10000, 30000, 60000, 180000, 300000, 600000, 900000, 1200000,
                1800000, 3600000, 7200000, 10800000, 14400000, 21600000,
                43200000, 86400000
            };

        /// <summary>
        /// ThemeSettings 各个依赖项属性恢复默认值
        /// </summary>
        public void SetDefault()
        {
            foreach (var field in typeof (ThemeSettings).GetFields())
            {
                if (field.FieldType == typeof (DependencyProperty))
                    ClearValue((DependencyProperty) field.GetValue(field));
            }

            foreach (var item in IconItemSource)
            {
                item.IconFilePath = item.DefaultIconPath;
            }
            foreach (var item in CursorsItemSource)
                item.CursorFilePath = null;
            foreach (var item in SystemSoundItemSource)
                item.Path = null;
            WallpaperFileItemsSource.Clear();

            InstFonts.Clear();
            InstInputSkin.Clear();
            InstOtherFile.Clear();
            ThemeColor = Color.FromArgb(100, 116, 184, 252);
        }

        /// <summary>
        /// 解压7zip文件
        /// </summary>
        /// <param name="file">输入路径</param>
        /// <param name="path">输出路径</param>
        public void ExtractFile(string file, string path)
        {
            var filename = Path.GetFileName(file);
            File.Copy(file, path + @"\" + filename, true);
            var exe7Zip =
                new Process
                {
                    StartInfo =
                    {
                        WorkingDirectory = path,
                        FileName = Paths.AppDir + @"\Res\Tools\7zip\7zr.exe",
                        Arguments = "x " + filename
                        //UseShellExecute = false,
                        //CreateNoWindow = true,
                        //RedirectStandardOutput = true
                    }
                };
            
            exe7Zip.Start();
            exe7Zip.WaitForExit();
            File.Delete(path + @"\" + filename);
        }

        #region OpenThemeFile

        /// <summary>
        /// 读取主题文件（theme）内容
        /// </summary>
        /// <param name="themeFilePath">主题文件路径</param>
        public void ReadThemeFile(string themeFilePath)
        {
            var ini = new IniHelper(themeFilePath);
            var iniText = File.ReadAllText(themeFilePath, Encoding.Default);

            // Info
            ThemeDiaplayName = ini.ReadInivalue("Theme", "DisplayName");
            ThemeFileName = Path.GetFileNameWithoutExtension(ini.GetRealPath(ini.ReadInivalue("VisualStyles", "Path")));
            ThemeFileInfo =
                iniText.Remove(iniText.IndexOf("\n[", StringComparison.Ordinal) - 1).Replace("; ", "").Replace(";", "").Replace(
                    "此主题文件由樱茶Win7主题生成器生成\r\n", "").Replace("\r\n\r\n", "");
            LogoFilePath = ini.GetFilePathInIni("Theme", "BrandImage");
            MsstylesFilePath = ini.GetFilePathInIni("VisualStyles", "Path", MsstylesFilePath);

            // Icon
            string[] iconSections =
            {
                @"CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\DefaultIcon",// Computer
                @"CLSID\{59031A47-3F72-44A7-89C5-5595FE6B30EE}\DefaultIcon",// User
                @"CLSID\{F02C1A0D-BE21-4350-88B0-7367FC96EF3C}\DefaultIcon",// network
                @"CLSID\{645FF040-5081-101B-9F08-00AA002F954E}\DefaultIcon",// Full
                @"CLSID\{645FF040-5081-101B-9F08-00AA002F954E}\DefaultIcon"// Empty
            };

            string[] iconKeys =
            {
                "DefaultValue", "DefaultValue", "DefaultValue", "Full", "Empty"
            };

            for (var i = 0; i < 5; i++)
            {
                IconItemSource[i].IconFilePath =
                ini.GetFilePathInIni(iconSections[i], iconKeys[i], IconItemSource[i].IconFilePath);
            }

            // Cursors
            foreach (var item in CursorsItemSource)
            {
                var path = ini.ReadInivalue(@"Control Panel\Cursors", item.IniKey);
                var defaultPath =
                    (new IniHelper(Paths.AppDir + @"\Res\File\aero.theme").ReadInivalue(@"Control Panel\Cursors",
                        item.IniKey));
                if (path != null && ini.GetRealPath(path).ToLower() != ini.GetRealPath(defaultPath).ToLower())
                    item.CursorFilePath = ini.GetFilePathInIni(@"Control Panel\Cursors", item.IniKey);
            }

            // Wallpapers
            var firstWallper = ini.GetFilePathInIni(@"Control Panel\Desktop", "Wallpaper");
            if (firstWallper != null)
                WallpaperFileItemsSource.Add(new WallpaperFileItem {ImageFilePath = firstWallper});
            var wallpaperPath = ini.GetRealPath(ini.ReadInivalue("Slideshow", "ImagesRootPath"));
            if (Directory.Exists(wallpaperPath))
            {
                foreach (var file in new DirectoryInfo(wallpaperPath).GetFiles())
                {
                    var extension = Path.GetExtension(file.FullName);
                    if (extension == "") continue;
                    var ext = extension.ToLower();
                    if (ext != ".jpg" && ext != ".png" && ext != ".bmp" && ext != ".gif") continue;
                    if (firstWallper != null && !String.Equals(file.FullName, firstWallper, StringComparison.CurrentCultureIgnoreCase))
                    {
                        WallpaperFileItemsSource.Add(
                            new WallpaperFileItem
                            {
                                ImageFilePath = file.FullName
                            });
                    }
                }
            }
            var sliderTime = ini.ReadInivalue("Slideshow", "Interval");
            AppDebug.Log("slidertime:" + sliderTime);
            if (!string.IsNullOrEmpty(sliderTime))
            {
                try
                {
                    var sltime = int.Parse(sliderTime);
                    for (var i = 0; i < _wallpaperSliderTimes.Length; i++)
                    {
                        if (sltime != _wallpaperSliderTimes[i]) continue;
                        SliderShowIntervalIndex = i;
                        AppDebug.Log("SliderShowIntervalIndex: " + i);
                        break;
                    }
                }
                catch (Exception ex)
                {
                    AppDebug.Log(ex.Message + ex.StackTrace);
                }
            }
            SliderShowShuffle = ini.ReadInivalue("Slideshow", "Shuffle") == "1";

            // Color
            try
            {
                var convertFromString =
                    ColorConverter.ConvertFromString("#" + ini.ReadInivalue("VisualStyles", "ColorizationColor").ToUpper().Replace("0X", "")) as Color?;
                if (convertFromString != null) ThemeColor = convertFromString.Value;
            }
            catch (Exception ex)
            {
                AppDebug.Log(ex);
            }

            //Sound
            var soundThemeName = ini.ReadInivalue("Sounds", "SchemeName");
            if (soundThemeName.ToLower().Contains(@"\mmres.dll"))
            {
                var num = soundThemeName.Remove(0, soundThemeName.LastIndexOf("-", StringComparison.Ordinal) + 1);
                for (var i = 0; i < _systemWaveDefaultNum.Length; i++)
                    if (num == _systemWaveDefaultNum[i]) DefaultSystemSoundIndex = i;
            }
            else
            {
                foreach (var item in SystemSoundItemSource)
                    item.Path = ini.GetFilePathInIni(item.IniSection, "DefaultValue");
                IsUseDefaultSystemSound = false;
            }
        }

        /// <summary>
        /// 读取鼠标指针INF文件
        /// </summary>
        /// <param name="path"></param>
        public void ReadCursorInf(string path)
        {
            var ini = new IniHelper(path);
            string[] cursStrs =
            {
                "work", "pointer", "cross", "link", "hand", "help", "text", "unavailiable",
                "move", "dgn2", "vert", "dgn1", "horz", "alternate", "busy"
            };
            for (var i = 0; i < cursStrs.Length; i++)
            {
                CursorsItemSource[i].CursorFilePath = null;
                var res = ini.ReadInivalue("Strings", cursStrs[i]);
                var path2 = Path.Combine(Path.GetDirectoryName(path), res);
                if (File.Exists(path2))
                    CursorsItemSource[i].CursorFilePath = path2;

                AppDebug.Log(CursorsItemSource[i].CursorFilePath);
            }

        }

        public void BatchReadIcons(string[] paths)
        {
            var list = new List<string>();
            for (var i = 0; i < 5; i++)
            {
                list.Add(null);
            }
            var count = paths.Count() > 5 ? 5 : paths.Count();
            for (var i = 0; i < count; i++)
            {
                list[i] = paths[i];
            }
            for (var i = 0; i < 5; i++)
            {
                ChangeIconsItem(ref list, i, 0, "com", "计算机", "电脑");
                ChangeIconsItem(ref list, i, 1, "user", "file", "文件", "个人");
                ChangeIconsItem(ref list, i, 2, "net", "网");
                ChangeIconsItem(ref list, i, 3, "full", "满");
                ChangeIconsItem(ref list, i, 4, "empty", "空");
            }
            for (var i = 0; i < 5; i++)
            {
                if (list[i] != null) IconItemSource[i].IconFilePath = list[i];
            }
        }

        private void ChangeIconsItem(ref List<string> list,  int rep1, int rep2, params string[] str)
        {
            var org = Path.GetFileNameWithoutExtension(list[rep1]);
            if (org == null) return;
            var name = org.ToLower();
            foreach (var s in str)
            {
                if (!name.Contains(s) || rep1 == rep2) continue;
                var temp = list[rep1];
                list[rep1] = list[rep2];
                list[rep2] = temp;
                break;
            }

        }

        #endregion

        #region Installer

        /// <summary>
        /// 生成自解压安装包
        /// </summary>
        /// <param name="filename"></param>
        public void SfxCreatExe(string filename)
        {
            var tempThemePath = Paths.AppTempDir + @"\Theme";
            if (!Directory.Exists(tempThemePath)) Directory.CreateDirectory(tempThemePath);
            var noteFileName = tempThemePath + @"\note.txt";
            using (var fs = new FileStream(noteFileName, FileMode.Create))
            {
                using (var sw = new StreamWriter(fs, Encoding.Default))
                {
                    var note = @"Path=%SystemRoot%\resources\Themes\" + Environment.NewLine;
                    if (SfxRunAfterExtract == true)
                        note += string.Format("Setup=\"{0}.theme\"\r\n", ThemeFileName);
                    if (SfxTitle != "")
                        note += "Title=" + SfxTitle + Environment.NewLine;
                    note += "Text" + Environment.NewLine + "{" + Environment.NewLine + SfxReadme + Environment.NewLine + "}" + Environment.NewLine;
                    if (SfxIsUseLicence == true)
                        note += "License=" + Environment.NewLine + "{" + Environment.NewLine + SfxLicence + Environment.NewLine + "}" + Environment.NewLine;
                    sw.Write(note);
                }
            }
            SaveThemeFiles(tempThemePath);
            var process = new Process
            {
                StartInfo =
                {
                    Arguments = "a -sfxdefault.sfx -znote.txt -r theme",
                    FileName = Paths.WinrarExeFile,
                    WorkingDirectory = tempThemePath
                }
            };
            process.Start();
            process.WaitForExit();
            CopyFile(tempThemePath + @"\theme.exe", filename);
            MainWindow.ShowBubbleMessage(App.FindString("SfxCreatComplete"));
        }

        /// <summary>
        /// 复制文件夹
        /// </summary>
        /// <param name="direcSource">原路径</param>
        /// <param name="direcTarget">目标路径</param>
        /// <param name="ignoreError">是否忽略错误</param>
        public void CopyFolder(string direcSource, string direcTarget, bool ignoreError)
        {
            if (!Directory.Exists(direcTarget)) Directory.CreateDirectory(direcTarget);
            var direcInfo = new DirectoryInfo(direcSource);
            foreach (var file in direcInfo.GetFiles())
            {
                if (ignoreError)
                {
                    try
                    {
                        var comName = Path.Combine(direcTarget, file.Name);
                        file.CopyTo(comName, true);
                    }
                    catch (Exception ex)
                    {
                        AppDebug.Log(ex.Message);
                    }
                }
                else file.CopyTo(Path.Combine(direcTarget, file.Name), true);
            }
            foreach (var dir in direcInfo.GetDirectories())
                CopyFolder(Path.Combine(direcSource, dir.Name), Path.Combine(direcTarget, dir.Name), ignoreError);
        }

        private static void CopyFileWriteXml(string filename, string node)
        {
            if (!File.Exists(filename)) return;
            var xmlFile = Paths.AppTempDir + @"\SetupConfig.xml";
            var shortNode = node.Remove(0, node.LastIndexOf("/", StringComparison.Ordinal) + 1);
            File.Copy(filename, Paths.AppTempDir + @"\Contents\Files\" + shortNode + Path.GetExtension(filename), true);
            XmlHelper.Update(xmlFile, node, "", @"Contents\Files\" + shortNode + Path.GetExtension(filename));
        }

        public string GetMssFileNameFromTheme(string path)
        {
            var ini = new IniHelper(InstCostomThemeFilePath);
            var mssfile =
                ini.GetRealPath(ini.ReadInivalue("VisualStyles", "Path")).ToLower().Replace(
                    Paths.SysThemeDir.ToLower(), Path.GetDirectoryName(InstCostomThemeFilePath));
            return mssfile;
        }

        /// <summary>
        /// 创建现代化安装包
        /// </summary>
        /// <param name="filename"></param>
        public void InstallerCreatExe(string filename)
        {
            // 解压安装器
            ExtractFile(Paths.AppDir + @"\Res\File\ThemeInstaller.7z", Paths.AppTempDir);

            // 主题文件
            if (InstIsUseEditorTheme == true)
            {
                SaveThemeFiles(Paths.AppTempDir + @"\Contents\Theme");
            }
            else
            {
                if (InstCostomThemeFilePath != "")
                {
                    CopyFile(InstCostomThemeFilePath,
                        Paths.AppTempDir + @"\Contents\Theme\" + Path.GetFileName(InstCostomThemeFilePath));
                    var ini = new IniHelper(InstCostomThemeFilePath);
                    var mssfile =
                        ini.GetRealPath(ini.ReadInivalue("VisualStyles", "Path")).ToLower().Replace(
                            Paths.SysThemeDir.ToLower(), Path.GetDirectoryName(InstCostomThemeFilePath));
                    var mssDir = Path.GetDirectoryName(mssfile);
                    if (File.Exists(mssfile) && mssDir != null)
                    {
                        CopyFolder(mssDir, Paths.AppTempDir + @"\Contents\Theme" + mssDir.Remove(0, mssDir.LastIndexOf("\\", StringComparison.Ordinal)), true);
                    }
                }
                else
                {
                    MessageBox.Show(App.FindString("InstCreatErrorChooseStyle"));
                    return;
                }
            }

            var xmlFile = Paths.AppTempDir + @"\SetupConfig.xml";

            // 背景图片
            CopyFileWriteXml(InstBackgroundImage, "/Settings/BackgroundImagePath");

            // 闪屏图片
            CopyFileWriteXml(InstSplashLogo, "/Settings/SplashImagePath");

            // 欢迎图片
            CopyFileWriteXml(InstWelcomeLogo, "/Settings/WelcomeImagePath");

            // BGM
            CopyFileWriteXml(InstBgm, "/Settings/BgmPath");

            // 标题
            XmlHelper.Update(xmlFile, "/Settings/Title", "", InstTitle.Trim());

            // 作者
            XmlHelper.Update(xmlFile, "/Settings/Author", "", InstAuthor.Trim());

            // 网站
            XmlHelper.Update(xmlFile, "/Settings/Website", "", InstWebsite.Trim());

            // 说明信息
            if (InstIsUseReadme == true)
            {
                XmlHelper.Update(xmlFile, "/Settings/ReadmeInfo", "", InstReadme);
            }

            // 许可信息
            if (InstIsUseLicence == true)
            {
                XmlHelper.Update(xmlFile, "/Settings/LicenceInfo", "", InstLicence);
            }

            // 主题版本
            XmlHelper.Update(xmlFile, "/Settings/ThemeVersion", "", InstThemeVersion);

            // 字体文件
            if (InstFonts.Count > 0)
            {
                foreach (var font in InstFonts)
                {
                    File.Copy((string) font.Content, Paths.AppTempDir + @"\Contents\Fonts\" + Path.GetFileName((string) font.Content), true);
                }
            }

            // 开始按钮
            CopyFileWriteXml(InstStartButton, "/Settings/StartButtonPath");

            // 登录界面背景图片
            CopyFileWriteXml(InstLogonImage, "/Settings/LogonImagePath");

            // 输入法皮肤
            if (InstInputSkin.Count > 0)
            {
                foreach (var skin in InstInputSkin)
                {
                    File.Copy((string) skin.Content, Paths.AppTempDir + @"\Contents\Input\" + Path.GetFileName((string) skin.Content), true);
                }
            }

            // 其他文件
            if (InstOtherFile.Count > 0)
            {
                foreach (var file in InstOtherFile.Where(file => File.Exists((string)file.Content))) 
                {
                    File.Copy((string) file.Content, Paths.AppTempDir + @"\Contents\Other\" + Path.GetFileName((string) file.Content), true);
                }
            }

            // 密码
            if (InstPassword.Trim() != "") XmlHelper.Update(xmlFile, "/Settings/Password", "", InstPassword.Trim());

            // 安装包图标
            if (File.Exists(InstSetupIcon)) File.Copy(InstSetupIcon, Paths.AppTempDir + @"\icon.ico", true);
            else
                File.Copy(Paths.AppDir + @"\Res\Tools\ResHacker\win7themeinstaller.ico", Paths.AppTempDir + @"\icon.ico", true);
            
            // 7zip打包
            var exe7Zip =
                new Process
                {
                    StartInfo =
                    {
                        WorkingDirectory = Paths.AppTempDir,
                        FileName = Paths.AppDir + @"\Res\Tools\7zip\7zr.exe",
                        Arguments = "a -sfx7zS.sfx a.exe * -r"
                    }
                };
            exe7Zip.Start();
            exe7Zip.WaitForExit();

            // 换图标资源
            var reshackerScript = Paths.AppTempDir + @"\script.txt";
            using (var fs = new FileStream(reshackerScript, FileMode.Create))
            {
                using (var sw = new StreamWriter(fs, Encoding.Default))
                {
                    const string content = "[FILENAMES]\r\nExe=a.exe\r\nSaveAs=b.exe\r\n[COMMANDS]\r\n-addoverwrite icon.ico, ICONGROUP, 1, 2052";
                    sw.Write(content);
                }
            }
            var exeResHacker =
                new Process
                {
                    StartInfo =
                    {
                        FileName = Paths.AppDir + @"\Res\Tools\ResHacker\ResHacker.exe",
                        WorkingDirectory = Paths.AppTempDir,
                        Arguments = "-script script.txt",
                        WindowStyle = ProcessWindowStyle.Hidden
                    }
                };
            exeResHacker.Start();
            exeResHacker.WaitForExit();

            // 复制完成的安装包
            try
            {
                File.Copy(Paths.AppTempDir + @"\b.exe", filename, true);
            }
            catch (Exception ex)
            {
                File.Copy(Paths.AppTempDir + @"\a.exe", filename, true);
                MessageBox.Show(App.FindString("InstCreatErrorIconFail") + Environment.NewLine + ex.Message);
            }

            MainWindow.ShowBubbleMessage(App.FindString("InstCreatComplete"));
        }

        #endregion

        #region SaveTheme

        private static void CopyFile(string pathFrom, string pathTo)
        {
            if (!File.Exists(pathFrom))
            {
                AppDebug.Log(pathFrom + " :File doesn't exist");
                return;
            }
            //var fi = new FileInfo(pathFrom);
            //判断文件属性是否只读?是则修改为一般属性再操作
            //if (fi.Attributes.ToString().IndexOf("ReadOnly", StringComparison.Ordinal) != -1)
            //    fi.Attributes = FileAttributes.Normal;
            try
            {
                File.Copy(pathFrom, pathTo, true);
            }
            catch (Exception exception)
            {
                AppDebug.Log(exception);
            }
        }

        /// <summary>
        /// 保存Theme文件和相关文件到指定目录
        /// </summary>
        /// <param name="folderPath">目标目录</param>
        public void SaveThemeFiles(string folderPath)
        {
            // Prepare
            var themeFilePath = folderPath + "\\" + ThemeFileName + ".theme";
            var mssDirectoryPath = folderPath + "\\" + ThemeFileName;
            var virtualMssDirectoryPath = @"%SystemRoot%\Resources\Themes\" + ThemeFileName;
            var ini = new IniHelper(themeFilePath);
            ExtractFile(Paths.AppDir + @"\Res\File\Theme.7z", folderPath);
            Directory.Move(folderPath + @"\Aero", mssDirectoryPath);
            File.Move(folderPath + @"\aero.theme", themeFilePath);
            File.Move(mssDirectoryPath + @"\zh-CN\aero.msstyles.mui",
                mssDirectoryPath + @"\zh-CN\" + ThemeFileName + ".msstyles.mui");

            // TextReader
            string themetext;
            using (var fs = new FileStream(themeFilePath, FileMode.Open))
            {
                using (var sr = new StreamReader(fs, Encoding.Default))
                {
                    themetext = sr.ReadToEnd();
                }
            }
            themetext = themetext.Replace("{ThemeNote}", "此主题文件由樱茶Win7主题生成器生成\r\n; " + ThemeFileInfo.Replace("\r\n", "\r\n; "));
            using (var fs = new FileStream(themeFilePath, FileMode.Create))
            {
                using (var sw = new StreamWriter(fs, Encoding.Default))
                {
                    sw.Write(themetext);
                }
            }

            // Info
            ini.WriteInivalue("Theme", "DisplayName", ThemeDiaplayName);
            if (File.Exists(LogoFilePath))
            {
                ini.WriteInivalue("Theme", "BrandImage", virtualMssDirectoryPath + @"\Logo.png");
                CopyFile(LogoFilePath, mssDirectoryPath + @"\Logo.png");
            }

            // Msstyles
            ini.WriteInivalue("VisualStyles", "Path", virtualMssDirectoryPath + @"\" + ThemeFileName + ".msstyles");
            CopyFile(MsstylesFilePath, mssDirectoryPath + @"\" + ThemeFileName + ".msstyles");

            // Icon
            string[] iconSections =
            {
                @"CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\DefaultIcon",// Computer
                @"CLSID\{59031A47-3F72-44A7-89C5-5595FE6B30EE}\DefaultIcon",// User
                @"CLSID\{F02C1A0D-BE21-4350-88B0-7367FC96EF3C}\DefaultIcon",// network
                @"CLSID\{645FF040-5081-101B-9F08-00AA002F954E}\DefaultIcon",// Full
                @"CLSID\{645FF040-5081-101B-9F08-00AA002F954E}\DefaultIcon"// Empty
            };
            string[] iconKeys =
            {
                "DefaultValue", "DefaultValue", "DefaultValue", "Full", "Empty"
            };
            string[] iconNames =
            {
                @"\Icon\Computer.ico",
                @"\Icon\UsersFiles.ico",
                @"\Icon\Network.ico",
                @"\Icon\RecycleFull.ico",
                @"\Icon\RecycleEmpty.ico"
            };

            for (var i = 0; i < 5; i++)
            {
                if (IconItemSource[i].IsDefaultIcon != false) continue;
                ini.WriteInivalue(iconSections[i], iconKeys[i], virtualMssDirectoryPath + iconNames[i] +",0");
                CopyFile(IconItemSource[i].IconFilePath, mssDirectoryPath + iconNames[i]);
            }

            // Cursors
            var isExistCostomCursor = false;
            foreach (var item in CursorsItemSource.Where(item => !string.IsNullOrEmpty(item.CursorFilePath) && File.Exists(item.CursorFilePath)))
            {
                ini.WriteInivalue(@"Control Panel\Cursors", item.IniKey,
                    virtualMssDirectoryPath + @"\Cursors\" + item.IniKey +
                        Path.GetExtension(item.CursorFilePath));
                CopyFile(item.CursorFilePath,
                    mssDirectoryPath + @"\Cursors\" + item.IniKey + Path.GetExtension(item.CursorFilePath));
                isExistCostomCursor = true;
            }
            if (isExistCostomCursor)
            {
                ini.WriteInivalue(@"Control Panel\Cursors", "DefaultValue", ThemeDiaplayName + App.FindString("ThemeFileCursorsTheme"));
            }

            // Wallpapers
            if (WallpaperFileItemsSource.Count > 0)
            {
                ini.WriteInivalue(@"Control Panel\Desktop", "Wallpaper",
                    virtualMssDirectoryPath + @"\Wallpapers\0" +
                        Path.GetExtension(WallpaperFileItemsSource[0].ImageFilePath));
                CopyFile(WallpaperFileItemsSource[0].ImageFilePath,
                    mssDirectoryPath + @"\Wallpapers\0" +
                        Path.GetExtension(WallpaperFileItemsSource[0].ImageFilePath));
                if (WallpaperFileItemsSource.Count > 1)
                {
                    ini.WriteInivalue("Slideshow", "Interval", _wallpaperSliderTimes[SliderShowIntervalIndex].ToString(CultureInfo.InvariantCulture));
                    ini.WriteInivalue("Slideshow", "ImagesRootPath", virtualMssDirectoryPath + @"\Wallpapers");
                    var shuffle = SliderShowShuffle == true ? "1" : "0";
                    ini.WriteInivalue("Slideshow", "Shuffle", shuffle);
                    for (var i = 1; i < WallpaperFileItemsSource.Count; i++)
                    {
                        CopyFile(WallpaperFileItemsSource[i].ImageFilePath,
                            mssDirectoryPath + @"\Wallpapers\" + i.ToString(CultureInfo.InvariantCulture) +
                                Path.GetExtension(WallpaperFileItemsSource[i].ImageFilePath));
                    }
                }
            }

            // Color
            ini.WriteInivalue("VisualStyles", "ColorizationColor", "0x" + ThemeColor.ToString().Replace("#", ""));

            // SystemSound
            if (IsUseDefaultSystemSound == true)
            {
                ini.WriteInivalue("Sounds", "SchemeName",
                    "@%SystemRoot%\\System32\\mmres.dll,-" +
                        _systemWaveDefaultNum[DefaultSystemSoundIndex]);
            }
            else
            {
                ini.WriteInivalue("Sounds", "SchemeName", ThemeDiaplayName + App.FindString("ThemeFileSoundTheme"));
                foreach (var item in SystemSoundItemSource.Where(item => !string.IsNullOrEmpty(item.Path)))
                {
                    ini.WriteInivalue(item.IniSection, "DefaultValue",
                        virtualMssDirectoryPath + @"\Sound\" +
                            item.IniSection.Remove(0,
                                item.IniSection.LastIndexOf("\\", StringComparison.Ordinal) +
                                    1).Replace(".", "") + ".wav");
                    CopyFile(item.Path,
                        mssDirectoryPath + @"\Sound\" +
                            item.IniSection.Remove(0, item.IniSection.LastIndexOf("\\", StringComparison.Ordinal) + 1).
                                Replace(".", "") + ".wav");
                }
            }
        }

        #endregion
    }
}