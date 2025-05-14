using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using Win7ThemeEditor.Helper;

namespace Win7ThemeEditor
{
    public class AdminHelper
    {
        /// <summary>
        /// 运行W7TEHelper.exe并传入相关字符
        /// </summary>
        /// <param name="arg"></param>
        public static void AdminExecute(string arg)
        {
            var process = new Process
            {
                StartInfo =
                {
                    FileName = "W7TEHelper.exe", 
                    Arguments = arg
                }
            };
            try
            {
                process.Start();
                process.WaitForExit();
            }
            catch (Exception ex)
            {
                AppDebug.Log(ex.Message);
            }
            
        }

        static internal bool AppIsAdmin()
        {
            var id = WindowsIdentity.GetCurrent();
            if (id != null) 
            {
                var p = new WindowsPrincipal(id);
                return p.IsInRole(WindowsBuiltInRole.Administrator);
            }
            return false;
        }
    }
}
