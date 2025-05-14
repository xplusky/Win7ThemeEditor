#region

using System;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

#endregion

namespace ThemeInstaller
{
    
　　public class IniHelper
　　{
　　　　public string FileName; //INI文件名

　　    public IniHelper(string aFileName)
　　    {
　　        // 判断文件是否存在
　　        var fileInfo = new FileInfo(aFileName);
　　        // 搞清枚举的用法
　　        if ((!fileInfo.Exists))
　　        { //|| (FileAttributes.Directory in fileInfo.Attributes))
　　            //文件不存在，建立文件
　　            var sw = new StreamWriter(aFileName, false, Encoding.Default);
　　            try
　　            {
　　                sw.Write("#表格配置档案");
　　                sw.Close();
　　            }

　　            catch
　　            {
　　                throw new ApplicationException("Ini文件不存在");
　　            }
　　        }
　　        //必须是完全路径，不能是相对路径
　　        FileName = fileInfo.FullName;
　　    }

　　    //声明读写INI文件的API函数
　　　　[DllImport("kernel32")]
　　　　private static extern bool WritePrivateProfileString(string section, string key, string val, string filePath);
　　　　[DllImport("kernel32")]
　　　　private static extern int GetPrivateProfileString(string section, string key, string def, byte[] retVal, int size, string filePath);

　　　　//类的构造函数，传递INI文件名
　　    //写INI文件
　　　　public void WriteString(string section, string ident, string value)
　　　　{
　　　　　　if (!WritePrivateProfileString(section, ident, value, FileName))
　　　　　　{
　
　　　　　　　　throw (new ApplicationException("写Ini文件出错"));
　　　　　　}
　　　　}
　　　　//读取INI文件指定
　　　　public string ReadString(string section, string ident, string Default)
　　　　{
　　　　　　var buffer = new Byte[65535];
　　　　　　var bufLen = GetPrivateProfileString(section, ident, Default, buffer, buffer.GetUpperBound(0), FileName);
　　　　　　//必须设定0（系统默认的代码页）的编码方式，否则无法支持中文
　　　　　　var s = Encoding.GetEncoding(0).GetString(buffer);
　　　　　　s = s.Substring(0, bufLen);
　　　　　　return s.Trim();
　　　　}

　　　　//读整数
　　　　public int ReadInteger(string section, string ident, int Default)
　　　　{
　　　　　　var intStr = ReadString(section, ident, Convert.ToString(Default));
　　　　　　try
　　　　　　{
　　　　　　　　return Convert.ToInt32(intStr);

　　　　　　}
　　　　　　catch (Exception ex)
　　　　　　{
　　　　　　　　Console.WriteLine(ex.Message);
　　　　　　　　return Default;
　　　　　　}
　　　　}

　　　　//写整数
　　　　public void WriteInteger(string section, string ident, int value)
　　　　{
　　　　　　WriteString(section, ident, value.ToString(CultureInfo.InvariantCulture));
　　　　}

　　　　//读布尔
　　　　public bool ReadBool(string section, string ident, bool Default)
　　　　{
　　　　    if (section == null) throw new ArgumentNullException("section");
　　　　    try
　　　　　　{
　　　　　　　　return Convert.ToBoolean(ReadString(section, ident, Convert.ToString(Default)));
　　　　　　}
　　　　　　catch (Exception ex)
　　　　　　{
　　　　　　　　Console.WriteLine(ex.Message);
　　　　　　　　return Default;
　　　　　　}
　　　　}

　　    //写Bool
　　　　public void WriteBool(string section, string ident, bool value)
　　　　{
　　　　　　WriteString(section, ident, Convert.ToString(value));
　　　　}

　　　　//从Ini文件中，将指定的Section名称中的所有Ident添加到列表中
　　　　public void ReadSection(string section, StringCollection idents)
　　　　{
　　　　　　var buffer = new Byte[16384];
　　　　　　//Idents.Clear();

　　　　　　var bufLen = GetPrivateProfileString(section, null, null, buffer, buffer.GetUpperBound(0),
　　　　　　　FileName);
　　　　　　//对Section进行解析
　　　　　　GetStringsFromBuffer(buffer, bufLen, idents);
　　　　}

　　　　private void GetStringsFromBuffer(Byte[] buffer, int bufLen, StringCollection strings)
　　　　{
　　　　　　strings.Clear();
　　　　    if (bufLen == 0) return;
　　　　    var start = 0;
　　　　    for (var i = 0; i < bufLen; i++)
　　　　    {
　　　　        if ((buffer[i] != 0) || ((i - start) <= 0)) continue;
　　　　        var s = Encoding.GetEncoding(0).GetString(buffer, start, i - start);
　　　　        strings.Add(s);
　　　　        start = i + 1;
　　　　    }
　　　　}
　　　　//从Ini文件中，读取所有的Sections的名称
　　　　public void ReadSections(StringCollection sectionList)
　　　　{
　　　　　　//Note:必须得用Bytes来实现，StringBuilder只能取到第一个Section
　　　　　　var buffer = new byte[65535];
　　　　    var bufLen = GetPrivateProfileString(null, null, null, buffer,
　　　　        buffer.GetUpperBound(0), FileName);
　　　　　　GetStringsFromBuffer(buffer, bufLen, sectionList);
　　　　}
　　　　//读取指定的Section的所有Value到列表中
　　　　public void ReadSectionValues(string section, NameValueCollection values)
　　　　{
　　　　　　var keyList = new StringCollection();
　　　　　　ReadSection(section, keyList);
　　　　　　values.Clear();
　　　　　　foreach (var key in keyList)
　　　　　　{
　　　　　　　　values.Add(key, ReadString(section, key, ""));
　　
　　　　　　}
　　　　}
　　　　
　　　　//清除某个Section
　　　　public void EraseSection(string section)
　　　　{
　　　　　　//
　　　　　　if (!WritePrivateProfileString(section, null, null, FileName))
　　　　　　{

　　　　　　　　throw (new ApplicationException("无法清除Ini文件中的Section"));
　　　　　　}
　　　　}
　　　　//删除某个Section下的键
　　　　public void DeleteKey(string section, string ident)
　　　　{
　　　　　　WritePrivateProfileString(section, ident, null, FileName);
　　　　}
　　　　//Note:对于Win9X，来说需要实现UpdateFile方法将缓冲中的数据写入文件
　　　　//在Win NT, 2000和XP上，都是直接写文件，没有缓冲，所以，无须实现UpdateFile
　　　　//执行完对Ini文件的修改之后，应该调用本方法更新缓冲区。
　　　　public void UpdateFile()
　　　　{
　　　　　　WritePrivateProfileString(null, null, null, FileName);
　　　　}

　　　　//检查某个Section下的某个键值是否存在
　　　　public bool ValueExists(string section, string ident)
　　　　{
　　　　　　//
　　　　　　var idents = new StringCollection();
　　　　　　ReadSection(section, idents);
　　　　　　return idents.IndexOf(ident) > -1;
　　　　}

　　　　//确保资源的释放
        ~IniHelper()
　　　　{
　　　　　　UpdateFile();
　　　　}
　　}

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
  public class IniHelper2
  {
      ///  <summary>
      ///  ini文件名称（带路径)
      ///  </summary>
      public string FilePath;

      ///  <summary>
      ///  类的构造函数
      ///  </summary>
      ///  <param  name="iniPath">INI文件名</param>  
      public IniHelper2(string iniPath)
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
