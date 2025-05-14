using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace Win7ThemeEditor
{
    public static class AeroHelper
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct Margins
        {
            public int cxLeftWidth;
            public int cxRightWidth;
            public int cyTopHeight;
            public int cyBottomHeight;
        };

        [DllImport("DwmApi.dll")]
        private static extern int DwmExtendFrameIntoClientArea(
            IntPtr hwnd,
            ref Margins pMarInset);

        public static bool ExtendAeroGlass(Window window)
        {
            try
            {
                // 为WPF程序获取窗口句柄
                var mainWindowPtr = new WindowInteropHelper(window).Handle;
                var mainWindowSrc = HwndSource.FromHwnd(mainWindowPtr);
                if (mainWindowSrc != null) if (mainWindowSrc.CompositionTarget != null) mainWindowSrc.CompositionTarget.BackgroundColor = Colors.Transparent;

                // 设置Margins
                var margins = new Margins {cxLeftWidth = -1, cxRightWidth = -1, cyTopHeight = -1, cyBottomHeight = -1};

                // 扩展Aero Glass

                if (mainWindowSrc != null)
                {
                    var hr = DwmExtendFrameIntoClientArea(mainWindowSrc.Handle, ref margins);
                    if (hr < 0)
                    {
                        MessageBox.Show("DwmExtendFrameIntoClientArea Failed");
                        return false;
                    }
                }
                return true;
            }
            catch (DllNotFoundException)
            {
                window.Background = Brushes.White;
                return false;
            }
        }
    }
}
