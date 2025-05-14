using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace ThemeInstaller
{
    public class External
    {
        #region Console

        /// <summary>
        /// 启动控制台
        /// </summary>
        /// <returns></returns>
        [DllImport("kernel32.dll")]
        public static extern bool AllocConsole();

        /// <summary>
        /// 释放控制台
        /// </summary>
        /// <returns></returns>
        [DllImport("kernel32.dll")]
        public static extern bool FreeConsole();

        #endregion

        #region WowSys64 to System32

        /// <summary>
        /// 实现32位程序访问64位系统的System32文件夹
        /// </summary>
        /// <param name="ptr"></param>
        /// <returns></returns>
        [DllImport("Kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool Wow64DisableWow64FsRedirection(ref IntPtr ptr);

        /// <summary>
        /// 还原32位程序访问64位系统的Wow64文件夹
        /// </summary>
        /// <param name="ptr"></param>
        /// <returns></returns>
        [DllImport("Kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool Wow64RevertWow64FsRedirection(IntPtr ptr);

        #endregion

        #region Fonts

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int WriteProfileString(string lpszSection, string lpszKeyName, string lpszString);

        [DllImport("user32.dll")]
        public static extern int SendMessage(
            int hWnd, // handle to destination window 
            uint msg, // message 
            int wParam, // first message parameter 
            int lParam // second message parameter 
            );

        [DllImport("gdi32")]
        public static extern int AddFontResource(string lpFileName);

        #endregion
    }
}
