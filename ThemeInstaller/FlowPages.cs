using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace ThemeInstaller
{
    public class FlowPages
    {
        public static int Position = 0;
        public static List<Uri> PageList { get; set; }

        public static void Add(params string[] names)
        {
            if (PageList == null) PageList = new List<Uri>();
            foreach (var name in names)
            {
                PageList.Add(new Uri(string.Format("Pages/{0}.xaml", name), UriKind.RelativeOrAbsolute));
            }
        }

        public static Uri NextPage()
        {
            Position++;
            return PageList[Position];
        }

        public static void GoNextPage(Page page)
        {
            if (page.NavigationService != null) page.NavigationService.Navigate(NextPage());
        }

        public static Uri PrevPage()
        {
            Position--;
            return PageList[Position];
        }

        public static void GoPrevPage(Page page)
        {
            if (page.NavigationService != null) page.NavigationService.Navigate(PrevPage());
        }

        public static Uri CurrentPage()
        {
            return PageList[Position];
        }

        public static void LoadPages()
        {
            if (Setup.DetectErrors())
            {
                Add("NotSupportSence");
            }
            else
            {
                switch (Setup.Mode)
                {
                    case Setup.ModeEnum.Install:
                        if (!string.IsNullOrEmpty(Setup.Settings["Password"].Content.Trim()))
                            Add("PasswordSence");
                        Add("WelcomeSence");
                        if (!string.IsNullOrEmpty(Setup.Settings["LicenceInfo"].Content))
                            Add("LicenceSence");
                        Add("EnvironmentSence");
                        if (!string.IsNullOrEmpty(Setup.Settings["ReadmeInfo"].Content))
                            Add("ReadmeSence");
                        Add("ComponentSence", "InstallingSence", "CompleteSence");
                        break;

                    case Setup.ModeEnum.Uninstall:
                        Add("WelcomeSence", "UninstallSence", "InstallingSence", "CompleteSence");
                        break;

                    case Setup.ModeEnum.Crack:
                        Add("WelcomeSence", "EnvironmentSence", "CompleteSence");
                        break;

                    case Setup.ModeEnum.ApplyLogonImage:
                        Add("InstallingSence", "CompleteSence");
                        break;
                }
            }
        }
    }
}