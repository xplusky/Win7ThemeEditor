using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using System.Xml;

namespace Win7ThemeEditor
{
    //[Serializable]
    public class AppSettings : FrameworkElement//, ISerializable
    {

        #region DependencyProperty

        public bool? IsBackgroundAnimating
        {
            get { return (bool?)GetValue(IsBackgroundAnimatingProperty); }
            set { SetValue(IsBackgroundAnimatingProperty, value); }
        }

        public static readonly DependencyProperty IsBackgroundAnimatingProperty =
            DependencyProperty.Register("IsBackgroundAnimating", typeof(bool?), typeof(AppSettings), new PropertyMetadata(true));



        public string WsbExeFilePath
        {
            get { return (string)GetValue(WsbExeFilePathProperty); }
            set { SetValue(WsbExeFilePathProperty, value); }
        }

        public static readonly DependencyProperty WsbExeFilePathProperty =
            DependencyProperty.Register("WsbExeFilePath", typeof(string), typeof(AppSettings), new PropertyMetadata(null));

        #endregion

        //private AppSettings(SerializationInfo info, StreamingContext context)
        //{
        //    try { IsBackgroundAnimating = info.GetBoolean("IsBackgroundAnimating"); }
        //    catch (Exception ex) { AppDebug.Log(ex.Message); }
        //    try { WsbExeFilePath = info.GetString("WsbExeFilePath"); }
        //    catch (Exception ex) { AppDebug.Log(ex.Message); }
        //}

        public void Save()
        {
            //try
            //{
            //    using (var stream = File.OpenWrite(Paths.ThisAppSettingFile))
            //    {
            //        (new BinaryFormatter()).Serialize(stream, this);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    AppDebug.Log(ex.Message);
            //}

            //Settings.Default.IsBakgroundAnimate = IsBackgroundAnimating == true;
            //Settings.Default.WsbExeFilePath = WsbExeFilePath;
            //Settings.Default.Save();

            //var xml = new XmlDocument();
            //var xmlPath = Paths.ThisAppSettingFile;
            //if(!File.Exists(xmlPath))
            //using (var sw = new StreamWriter(Paths.ThisAppSettingFile, false, Encoding.Default))
            //{
            //    sw.Write(XamlWriter.Save(this));
            //}

            XmlConfigHelper.WriteConfigData(IsBackgroundAnimatingProperty.Name, IsBackgroundAnimating.ToString());
            XmlConfigHelper.WriteConfigData(WsbExeFilePathProperty.Name, WsbExeFilePath);


        }

        public void Load()
        {
            //AppSettings setting = null;
            //try
            //{
            //    using (var stream = File.OpenRead(Paths.ThisAppSettingFile))
            //    {
            //        var formatter = new BinaryFormatter();
            //        setting = (AppSettings) formatter.Deserialize(stream);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    AppDebug.Log(ex.Message);
            //}
            //if (setting == null) return;
            //IsBackgroundAnimating = setting.IsBackgroundAnimating;
            //WsbExeFilePath = setting.WsbExeFilePath;

            //IsBackgroundAnimating = Settings.Default.IsBakgroundAnimate;
            //WsbExeFilePath = Settings.Default.WsbExeFilePath;
            var isanimate = XmlConfigHelper.GetConfigData(IsBackgroundAnimatingProperty.Name, "True");
            switch (isanimate)
            {
                case "True":
                    IsBackgroundAnimating = true;
                    break;
                case "False":
                    IsBackgroundAnimating = false;
                    break;
            }

            WsbExeFilePath = XmlConfigHelper.GetConfigData(WsbExeFilePathProperty.Name, string.Empty);
        }

        //public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        //{
        //    info.AddValue("IsBackgroundAnimating", IsBackgroundAnimating);
        //    info.AddValue("WsbExeFilePath", WsbExeFilePath);
        //}

    }
}
