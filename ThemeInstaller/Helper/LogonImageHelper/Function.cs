using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;

namespace ThemeInstaller.LogonImage
{
    public class Function
    {
        public static void Apply(string picPath)
        {
            var sh = Screen.PrimaryScreen.Bounds.Height;
            var sw = Screen.PrimaryScreen.Bounds.Width;
            var pic = Image.FromFile(picPath);
            try
            {
                Registry.SetValue(@"HKEY_LOCAL_MACHINE\Software\Microsoft\Windows\CurrentVersion\Authentication\LogonUI\Background", "OEMBackground", 1, RegistryValueKind.DWord);
                //MessageBox.Show("登陆画面修改成功!", "完成", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            catch (Exception)
            {
                MessageBox.Show("更改不成功,请检查当前账户是否是管理员账户!");
            }
            var size = new Size(sw, sh);
            var image = ImageProcessor.CompressToJpeg(pic, size, 0x3e800L);
            try
            {
                image.WriteStreamToFile(Environment.ExpandEnvironmentVariables(@"%windir%\system32\oobe\info\backgrounds\backgroundDefault.jpg"), true);
            }
            catch (Exception exception)
            {
                MessageBox.Show("图片保存错误:" + Environment.NewLine + exception.Message);
            }
            finally
            {
                if (image != null)
                {
                    image.Dispose();
                }
            }
            using (var reg = Registry.LocalMachine)
            {
                var key = reg.OpenSubKey(@"SOFTWARE\MicroSoft\Windows\CurrentVersion\Authentication\LogonUI", true);
                if (key != null) key.SetValue("Buttonset", 1);
            }
            //MessageBox.Show("字体阴影修改成功!", "完成", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);

        }
    }
}
