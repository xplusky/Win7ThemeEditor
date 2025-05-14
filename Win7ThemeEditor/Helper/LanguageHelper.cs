using System;
using System.IO;
using System.Windows;

namespace Win7ThemeEditor
{
    public class LanguageHelper
    {
        public static void LoadLanguageFile(string languagefileName)
        {
            
            Application.Current.Resources.MergedDictionaries[1] = new ResourceDictionary
            {
                Source = new Uri(languagefileName, UriKind.RelativeOrAbsolute)
            };
            
        }
    }
}
