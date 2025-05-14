using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Win7ThemeEditor.Helper
{
    public class AppDebug
    {
        public static void Log(params object[] messages)
        {
            //var output = string.Empty;
            //messages.SkipWhile(mes => mes == null).ToList().ForEach(
            //    mes =>
            //    {
            //        output += mes.ToString() + Environment.NewLine;
            //    });
            //output = string.Format("[{0}] {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), output);
            //File.AppendAllText(Paths.ThisAppLogFile, output, Encoding.Default);
            //Console.WriteLine(output);
        }
    }
}
