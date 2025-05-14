using System;

namespace Win7ThemeEditor.Helper
{
    public class SystemInfo
    {
        
        public static string OsBit()
        {
            return (Environment.GetEnvironmentVariable("ProgramFiles(x86)") != null) ? "64Bit" : "32Bit";
        }

        public static string SystemName()
        {
            var rootKey = Microsoft.Win32.Registry.LocalMachine;//本地计算机数据的配置 
            var runKey = rootKey.CreateSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");
            string value = null;
            try
            {
                if (runKey != null) value = runKey.GetValue("ProductName").ToString();
            }
            catch(Exception ex)
            {
                AppDebug.Log(ex.Message);
            }
            rootKey.Close();

            return value;
        }

        public static string SystemVersion()
        {
            return Environment.OSVersion.Version.ToString();
        }
    }
}
