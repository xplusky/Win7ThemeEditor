#region

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

#endregion

namespace Win7ThemeEditor
{
    ///  <summary>
    ///  读写ini文件的类
    ///  调用kernel32.dll中的两个api：WritePrivateProfileString，GetPrivateProfileString来实现对ini  文件的读写。
    ///
    ///  INI文件是文本文件,
    ///  由若干节(section)组成,
    ///  在每个带括号的标题下面,
    ///  是若干个关键词(key)及其对应的值(value)
    ///  
    ///[Section]
    ///Key=value
    ///
    ///  </summary>
    public class IniHelper
    {
        ///  <summary>
        ///  ini文件名称（带路径)
        ///  </summary>
        public string FilePath;

        ///  <summary>
        ///  类的构造函数
        ///  </summary>
        ///  <param  name="iniPath">INI文件名</param>  
        public IniHelper(string iniPath)
        {
            FilePath = iniPath;
        }

        //声明读写INI文件的API函数
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        ///  <summary>
        ///   写INI文件
        ///  </summary>
        ///  <param  name="section">Section</param>
        ///  <param  name="key">Key</param>
        ///  <param  name="value">value</param>
        public void WriteInivalue(string section, string key, string value)
        {
            WritePrivateProfileString(section, key, value, FilePath);
        }

        ///  <summary>
        ///    读取INI文件指定部分
        ///  </summary>
        ///  <param  name="section">Section</param>
        ///  <param  name="key">Key</param>
        ///  <returns>String</returns>  
        public string ReadInivalue(string section, string key)
        {
            var temp = new StringBuilder(255);
            try
            {
                var i = GetPrivateProfileString(section, key, "", temp, 255, FilePath);
                return temp.ToString();
            }
            catch { return ""; }

        }

        /// <summary>
        /// 将%systemroot%转换为实际路径
        /// </summary>
        /// <param name="origin">原代码</param>
        /// <returns></returns>
        public string ReplaceEnviromentVariable(string origin)//
        {
            return origin.ToLower().Replace("%systemroot%", Environment.GetEnvironmentVariable("windir")).Replace("%resourcedir%", Environment.GetEnvironmentVariable("windir") + "\\resources");

        }

        public string GetFilePathInIni(string section, string key)
        {
            var text = ReadInivalue(section, key);
            return File.Exists(GetRealPath(text)) ? GetRealPath(text) : null;
        }

        public string GetFilePathInIni(string section, string key, string org)
        {
            var text = ReadInivalue(section, key);
            return File.Exists(GetRealPath(text)) ? GetRealPath(text) : org;
        }

        public string GetRealPath(string text)
        {
            text = text.ToLower().Replace("%resourcedir%", Environment.GetEnvironmentVariable("windir") + @"\Resources").Replace("%systemroot%", Environment.GetEnvironmentVariable("windir"));
            //if (text.LastIndexOf(".ico") < text.Length - 4) text = text.Remove(text.IndexOf(".ico") + 4);
            return text;
        }
    }

}
