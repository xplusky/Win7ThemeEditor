using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Remoting;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Xml;
using ThemeInstaller.Pages;

namespace ThemeInstaller
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App
    {
        public static string Culture { get; set; }
        public App()
        {
            if (Debugger.IsAttached || Environment.CommandLine.Contains("/debug"))
            {
                External.AllocConsole();
            }
            else
            {
                DispatcherUnhandledException +=
                    (sender, args) =>
                    {
                        try
                        {
                            MessageBox.Show(args.Exception.ToString());
                        }
                        catch
                        {
                            MessageBox.Show("不可恢复的WPF窗体线程异常，应用程序将退出！");
                        }
                        Current.Shutdown();
                    };
            }
            Exit += OnExit;
        }

        private void OnExit(object sender, ExitEventArgs exitEventArgs)
        {
            //todo something
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Environment.CurrentDirectory = Paths.AppDir;

            // Init Language
            Culture = CultureInfo.CurrentUICulture.Name;
            if (File.Exists(Path.Combine(Paths.AppDir, string.Format(@"Tools\Lang\{0}.xaml", Culture))))
            {
                Current.Resources.MergedDictionaries[1] = new ResourceDictionary
                {
                    Source = new Uri(string.Format("pack://siteOfOrigin:,,,/Tools/Lang/{0}.xaml", Culture), UriKind.RelativeOrAbsolute)
                };
            }
            else
            {
                Culture = "en-US";
            }

            if (!Setup.LoadXml())
            {
                MessageBox.Show("Error when reading xml file");
                Current.Shutdown();
            }
            if (Environment.CommandLine.Contains("/applylogonimage"))
            {
                Setup.Mode = Setup.ModeEnum.ApplyLogonImage;
            }
            FlowPages.LoadPages();

            new MainWindow().Show();
        }

        public static string GetText(string key)
        {
            var message = Current.TryFindResource(key);
            return message == null ? "*Not translated* {0}{1}{2}{3}{4}" : message.ToString();
        }

        
    }

    public class Lang
    {
        public string Name { get; set; }
        public string Culture { get; set; }
        public string Path { get; set; }

        public override bool Equals(object other)
        {
            var lang = other as Lang;
            if (lang == null) return false;
            if (Name == lang.Name) return true;
            return false;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}
