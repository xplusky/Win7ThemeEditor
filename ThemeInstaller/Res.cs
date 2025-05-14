using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace ThemeInstaller
{
    public class Res
    {
        public string GetString(string key)
        {
            var message = Application.Current.TryFindResource(key);
            return message == null ? "*Not translated* {0}{1}{2}{3}{4}" : message.ToString();
        }
    }
}
