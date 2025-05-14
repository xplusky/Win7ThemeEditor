using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace W7TEHelper
{
    class Program
    {
        public static string GetTempPath()
        {
            return Path.Combine(Path.GetTempPath(), "W7TETemp");
        }
        private static string[] Args { get; set; }

        static void Main(string[] args)
        {
            Args = args;
            Environment.CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            if (!args.Any()) return;
            switch (args[0])
            {
                case "/save":
                    try
                    {
                        SaveToSystem();
                        Console.WriteLine("Save Complete!!");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        Console.WriteLine("Fail to save theme to directory of system theme.");
                        Console.ReadKey();
                    }
                    break;
                case "/deltheme":
                    try
                    {
                        DeleteTheme();
                        Console.WriteLine("Delete Complete!");
                        Thread.Sleep(1000);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Delete Theme Failed!");
                        Console.ReadKey();
                    }
                    
                    break;
                case "/initfiles":
                    InitFiles();
                    break;
            }
            
        }

        private static void InitFiles()
        {
            if (Directory.Exists(GetTempPath()))
            {
                try
                {
                    CancelReadOnlyAttribute(GetTempPath());
                    DeleteDirctory(GetTempPath());
                    Directory.CreateDirectory(GetTempPath());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    Console.ReadKey();
                }
            }
            else
            {
                Directory.CreateDirectory(GetTempPath());
            }
        }

        private static void DeleteTheme()
        {
            File.Delete(Args[1]);
            if (Args.Count() == 3)
            {
                DeleteDirctory(Args[2]);
                try
                {
                    Directory.Delete(Args[2], true);
                    Console.WriteLine(Args[2] + "Delete Ok.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(Args[2] + ex.Message);
                }
                
            }
        }

        private static void SaveToSystem()
        {
            if (Args.Count() == 2) //覆盖安装，先删除原来的文件夹
            {
                CancelReadOnlyAttribute(Args[1]);
                DeleteDirctory(Args[1]);
                Console.WriteLine(Args[1] + "Delete Ok.");
            }
            var fromDir = Path.Combine(GetTempPath(), "Theme");
            var toDir = Path.Combine(Environment.GetEnvironmentVariable("windir"), @"Resources\Themes");
            CancelReadOnlyAttribute(GetTempPath());
            CopyFolder(fromDir, toDir);
            InitFiles();
        }

        public static void CopyFolder(string direcSource, string direcTarget)
        {
            if (!Directory.Exists(direcTarget)) Directory.CreateDirectory(direcTarget);
            var dirInfo = new DirectoryInfo(direcSource);
            foreach (var file in dirInfo.GetFiles())
            {
                var destPath = Path.Combine(direcTarget, file.Name);
                try
                {
                    if (File.Exists(destPath))
                        Console.WriteLine(destPath + ", Overwrote.");
                    else
                        Console.WriteLine(destPath + ", Fine.");
                    file.CopyTo(destPath, true);
                }
                catch (IOException ex)
                {
                    Console.WriteLine("File:" + destPath + ":" + ex.Message + ", Skiped.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("File:" + destPath + ":" + ex.Message + ", Skiped.");
                }

            }
            foreach (var dir in dirInfo.GetDirectories())
            {
                CopyFolder(Path.Combine(direcSource, dir.Name), Path.Combine(direcTarget, dir.Name));
            }
        }

        public static void DeleteDirctory(string dirPath)
        {
            if (!Directory.Exists(dirPath)) return;
            var direcInfo = new DirectoryInfo(dirPath);
            
            foreach (var file in direcInfo.GetFiles())
            {
                try
                {
                    file.Delete();
                    Console.WriteLine(file.FullName + " Deleted");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(file.FullName + " " + ex.Message);
                }
            }
            foreach (var dir in direcInfo.GetDirectories())
            {
                DeleteDirctory(dir.FullName);
                try
                {
                    dir.Delete(true);
                    Console.WriteLine(dir.FullName + " Deleted");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(dir.FullName + " " + ex.Message);
                }
            }
        }

        public static void CancelReadOnlyAttribute(string dirPath)
        {
            if (!Directory.Exists(dirPath)) return;
            var dir = new DirectoryInfo(dirPath)
            {
                Attributes = FileAttributes.Normal
            };
            foreach (var fileInfo in dir.GetFiles())
            {
                fileInfo.Attributes = FileAttributes.Normal;
            }
            foreach (var subDir in dir.GetDirectories())
            {
                CancelReadOnlyAttribute(subDir.FullName);
            }
        }
    }
}
