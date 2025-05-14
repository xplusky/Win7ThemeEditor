using System;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Threading;
using Win7ThemeEditor.Helper;
using Win7ThemeEditor.Properties;

namespace Win7ThemeEditor
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App
    {
        
        public App()
        {
            DispatcherUnhandledException += Application_DispatcherUnhandledException;
            if (Debugger.IsAttached)
            {
                Imports.AllocConsole();
                AppDebug.Log("App Startting...");
                new TestWindow().Show();
            }
        }

        /// <summary>
        /// 界面语言
        /// </summary>
        public static string Culture { get; set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            var process = RuningInstance();
            if (process == null)
            {
                //LanguageHelper.LoadLanguageFile("pack://siteOfOrigin:,,,/Res/Lang/zh-CN.xaml");
                Culture = XmlConfigHelper.GetConfigData("Culture", string.Empty).Equals(string.Empty) ? CultureInfo.CurrentCulture.Name : XmlConfigHelper.GetConfigData("Culture", string.Empty);
                if (File.Exists(Path.Combine(Paths.AppDir , @"Res\Lang\" + Culture + ".xaml")))
                    LanguageHelper.LoadLanguageFile("pack://siteOfOrigin:,,,/Res/Lang/" + Culture + ".xaml");
                else
                {
                    Culture = "en-US";
                }
                new MainWindow().Show();
            }
            else
            {
                AppDebug.Log("应用程序已经在运行中。。。");
                HandleRunningInstance(process);
                Current.Shutdown();
            }
        }

        #region Debug
        
        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            if (Debugger.IsAttached) return;
            try
            {
                var ex = e.Exception;
                ShowErrorForm(ex);
                AppDebug.Log("\r\n" + ex + "\r\n");
            }
            catch
            {
                MessageBox.Show("不可恢复的WPF窗体线程异常，应用程序将退出！");
            }
        }

        private static void HandleRunningInstance(Process instance)
        {
            Imports.ShowWindowAsync(instance.MainWindowHandle, 1/*SW_SHOWNOMAL*/);//显示
            Imports.SetForegroundWindow(instance.MainWindowHandle);//当到最前端
        }
        private static Process RuningInstance()
        {
            var currentProcess = Process.GetCurrentProcess();
            var processes = Process.GetProcessesByName(currentProcess.ProcessName);
            foreach (var process in processes)
            {
                if (process.Id == currentProcess.Id) continue;
                if (Assembly.GetExecutingAssembly().Location.Replace("/", "\\") == currentProcess.MainModule.FileName)
                {
                    return process;
                }
            }
            return null;
        }

        /// <summary>
        /// 显示异常信息并发送到我的邮箱
        /// </summary>
        /// <param name="ex"></param>
        public static void ShowErrorForm(Exception ex)
        {
            var errorMsg = "[Win7 theme editor" + AppVersion + " error message]" + Environment.NewLine + ex.Message + Environment.NewLine + ex;
            new ErrorWindow(errorMsg).ShowDialog();
        }
        public static void ShowErrorForm(string message)
        {
            new ErrorWindow(message).ShowDialog();
        }

        
        #endregion

        #region Shared Infomation

        public static string AppVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString(3);
        
        #endregion

        public static string FindString(string key)
        {
            var message = Current.TryFindResource(key);
            return message == null ? "*Not translated* {0}{1}{2}{3}{4}" : message.ToString();
        }
    }
}
