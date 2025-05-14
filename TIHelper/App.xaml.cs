using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;

namespace TIHelper
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            const string fileName = "ThemeInstaller.exe";
            if(!File.Exists(fileName))
            {
                Current.Shutdown();
                return;
            }
            try
            {
                var process = new Process {StartInfo = {FileName = fileName}};
                process.Start();
                process.WaitForExit();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            Current.Shutdown();
        }
    }
}
