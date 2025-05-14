#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using Win7ThemeEditor.Helper;

#endregion

namespace Win7ThemeEditor.Controls
{
    /// <summary>
    /// 异步读取图片控件，界面在读取大图片时不会锁死
    /// </summary>
    public partial class AsyncImage
    {
        public static Queue<AsyncImage> ImageQueue = new Queue<AsyncImage>();
        private static bool _isQueueLocked;

        public AsyncImage()
        {
            InitializeComponent();
            Loaded += AsyncImage_Loaded;
        }

        
        public string ImagePath
        {
            get { return (string)GetValue(ImagePathProperty); }
            set { SetValue(ImagePathProperty, value); }
        }public static readonly DependencyProperty ImagePathProperty =
            DependencyProperty.Register("ImagePath", typeof(string), typeof(AsyncImage), new PropertyMetadata(null,
                (o,e)=>
                {
                    var asyncImage = o as AsyncImage;
                    if (asyncImage == null || !asyncImage.IsLoaded) return;
                    asyncImage.AddToQueue();
                }));

        public void AddToQueue()
        {
            ImageQueue.Enqueue(this);
            if (_isQueueLocked) return;
            _isQueueLocked = true;
            Console.WriteLine("Async Image Queue Lock");
            ImageQueue.Dequeue().LoadImage();
        }

        public void AsyncImage_Loaded(object sender, RoutedEventArgs e)
        {
            //LoadImage();
            AddToQueue();
        }

        public void LoadImage()
        {
            if (File.Exists(ImagePath))
            {
                new Thread(LoadImageAsync).Start(ImagePath);
                var sb = FindResource("LoadingSpinStoryboard") as Storyboard;
                if (sb != null) sb.Begin();
            }
            else
            {
                image.Source = null;
                var sb = FindResource("ShowImageStoryboard") as Storyboard;
                if (sb != null) sb.Begin();
                if (ImageQueue.Count > 0) ImageQueue.Dequeue().LoadImage();
                else
                {
                    _isQueueLocked = false;
                    Console.WriteLine("Async Image Queue Unlock");
                }
            }
        }

        private void LoadImageAsync(object para)
        {
            var bitmap = new BitmapImage();
            bool isLoadedOk;
            try
            {
                bitmap.BeginInit();
                bitmap.UriSource = new Uri((string) para);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                bitmap.Freeze();
                isLoadedOk = true;
            }
            catch (Exception)
            {
                AppDebug.Log("Load Error: " + (string) para);
                isLoadedOk = false;
            }
            

            Dispatcher.BeginInvoke((Action)(() =>
            {
                if(isLoadedOk)
                {
                    image.Source = bitmap;
                    AppDebug.Log("Image Loaded:" + bitmap.UriSource);
                }
                
                //image.Stretch = Stretch.Uniform;
                var sb = FindResource("LoadingSpinStoryboard") as Storyboard;
                if (sb != null) sb.Stop();
                var sb2 = FindResource("ShowImageStoryboard") as Storyboard;
                if (sb2 != null) sb2.Begin();
                if (ImageQueue.Count > 0) ImageQueue.Dequeue().LoadImage();
                else
                {
                    _isQueueLocked = false;
                    Console.WriteLine("Async Image Queue Unlock");
                }
            }));
        }

    }
}
