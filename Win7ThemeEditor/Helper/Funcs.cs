using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using Win7ThemeEditor.Properties;

namespace Win7ThemeEditor
{
    public static class Funcs
    {
        public enum StoryboardActionEnum
        {
            Begin, Pause, Resume, Stop
        }

        public static void StoryboardAct(StoryboardActionEnum act, params Storyboard[] sbs)
        {
            foreach (var sb in sbs)
            {
                switch (act)
                {
                    case StoryboardActionEnum.Begin:
                        sb.Begin();
                        break;
                    case StoryboardActionEnum.Pause:
                        sb.Pause();
                        break;
                    case StoryboardActionEnum.Resume:
                        sb.Resume();
                        break;
                    case StoryboardActionEnum.Stop:
                        sb.Stop();
                        break;
                }
            }
        }

        public static BitmapImage GetBitmapImage(string path)
        {
            return new BitmapImage(new Uri(path, UriKind.Absolute));
        }

        /// <summary>
        /// 显示文件对话框，打开文件
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="isMultislect"></param>
        /// <returns></returns>
        public static string[] OpenFile(string filter, bool isMultislect)
        {
            var op = new OpenFileDialog
            {
                RestoreDirectory = true,
                Multiselect = isMultislect,
                Filter = filter
            };
            op.ShowDialog();
            return op.FileName == string.Empty ? null : op.FileNames;
        }

        public static void OpenFilesToInstCollection(ICollection<ListBoxItem> oc, string filter)
        {
            if (oc == null) throw new ArgumentNullException("oc");
            var files = OpenFile(filter, true);
            if (files == null) return;
            foreach (var file in files)
                oc.Add(new ListBoxItem { Content = file });
        }

    }
}
