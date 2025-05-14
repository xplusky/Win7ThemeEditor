#region

using System;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Threading;
using ThemeInstaller.Pages;

#endregion

namespace ThemeInstaller
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {
        private bool _allowDirectNavigation;
        private bool _isMusicOn = true;
        private NavigatingCancelEventArgs _navArgs;

        public MainWindow()
        {
            InitializeComponent();
            Instance = this;
            KeyDown += MainWindow_KeyDown;
            StateChanged += MainWindowStateChanged;
            MouseLeftButtonDown += (sender, args) => DragMove();
        }

        void MainWindowStateChanged(object sender, EventArgs e)
        {
            if (WindowState != WindowState.Normal) return;
            var sb = TryFindResource("NormalStoryboard") as Storyboard;
            if (sb == null) return; 
            sb.Completed+= (o, ev) => sb.Stop();
            sb.Begin();
        }

        public static MainWindow Instance { get; private set; }

        void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            
        }

        private void MusicFadeVolume(double volume)
        {
            BGM.BeginAnimation(MediaElement.VolumeProperty,
                new DoubleAnimation(volume, new Duration(TimeSpan.FromMilliseconds(1000))));
        }

        /// <summary>
        /// 结束程序，显示动画
        /// </summary>
        public void EndApp()
        {
            MusicFadeVolume(0);
            var exitStoryboard = TryFindResource("ExitStoryboard") as Storyboard;
            if (exitStoryboard == null)
            {
                Close();
                return;
            }
            exitStoryboard.Completed += (o, e) => Close();
            exitStoryboard.Begin();
        }

        /// <summary>
        /// 导航动画
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainFrameNavigating(object sender, NavigatingCancelEventArgs e)
        {
            if (Content != null && !_allowDirectNavigation)
            {
                e.Cancel = true;
                _navArgs = e;
                IsHitTestVisible = false;
                var da = new DoubleAnimation(0, new Duration(TimeSpan.FromMilliseconds(100)));
                da.Completed += FadeOutCompleted;
                MainFrame.BeginAnimation(OpacityProperty, da);
                TitleText.BeginAnimation(OpacityProperty, da);
            }
            _allowDirectNavigation = false;
        }

        private void FadeOutCompleted(object sender, EventArgs e)
        {
            var animationClock = sender as AnimationClock;
            if (animationClock != null) animationClock.Completed -= FadeOutCompleted;
            IsHitTestVisible = true;
            _allowDirectNavigation = true;
            switch (_navArgs.NavigationMode)
            {
                case NavigationMode.New:
                    if (_navArgs.Uri == null) MainFrame.Navigate(_navArgs.Content);
                    else MainFrame.Navigate(_navArgs.Uri);
                    break;
                case NavigationMode.Back:
                    MainFrame.GoBack();
                    break;
                case NavigationMode.Forward:
                    MainFrame.GoForward();
                    break;
                case NavigationMode.Refresh:
                    MainFrame.Refresh();
                    break;
            }
            Dispatcher.BeginInvoke(DispatcherPriority.Loaded, (ThreadStart)
                delegate
                {
                    var da = new DoubleAnimation(1, new Duration(TimeSpan.FromMilliseconds(100)));
                    MainFrame.BeginAnimation(OpacityProperty, da);
                    TitleText.BeginAnimation(OpacityProperty, da);
                });
        }

        //事件响应
        private void ButtonCloseClick(object sender, RoutedEventArgs e)
        {
            EndApp();
        }

        private void ButtonMinisizeClick(object sender, RoutedEventArgs e)
        {
            var sb = TryFindResource("MinisizeStoryboard") as Storyboard;
            if(sb == null)return;
            sb.Completed +=
                (o, ev) =>
                {
                    WindowState = WindowState.Minimized;
                    sb.Stop();
                };
            sb.Begin();
            
        }

        private void ButtonVolumeClick(object sender, RoutedEventArgs e)
        {
            if (_isMusicOn)
            {
                MusicFadeVolume(0);
                _isMusicOn = false;
                ImageVolume.Source = new BitmapImage(new Uri("/ThemeInstaller;component/Res/sound_mute.png", UriKind.Relative));
            }
            else
            {
                MusicFadeVolume(0.5);
                _isMusicOn = true;
                ImageVolume.Source = new BitmapImage(new Uri("/ThemeInstaller;component/Res/sound_high.png", UriKind.Relative));
            }
        }

        private void ButtonInfoClick(object sender, RoutedEventArgs e)
        {
            MainFrame.NavigationService.Navigate(new Uri("Pages/AboutSence.xaml", UriKind.Relative));
        }

        private void MainWindowElementLoaded(object sender, RoutedEventArgs e)
        {
            // Splash
            var logoImage = Setup.Settings["SplashImagePath"].Content;
            if (File.Exists(logoImage))
            {
                ImageLogo.Source = new BitmapImage(new Uri(Paths.AppDir + "\\" + logoImage, UriKind.RelativeOrAbsolute));
            }

            // Background image
            var bgImage = Setup.Settings["BackgroundImagePath"].Content;
            if (File.Exists(bgImage))
            {
                var bitimg = new BitmapImage(new Uri(bgImage, UriKind.RelativeOrAbsolute));
                var imageBrush = new ImageBrush(bitimg) {Stretch = Stretch.UniformToFill};
                MainBg.Background = imageBrush;
            }

            // Music
            var bgmpath = Setup.Settings["BgmPath"].Content;
            if (File.Exists(bgmpath))
            {
                BGM.Source = new Uri(bgmpath, UriKind.RelativeOrAbsolute);
                BGM.MediaEnded +=
                    (o, ev) =>
                    {
                        BGM.Stop();
                        BGM.Play();
                    };
                BGM.Play();
            }
            else
            {
                ButtonVolume.Visibility = Visibility.Collapsed;
            }

            // Play storyboard
            var startSb = TryFindResource("StartStoryboard") as Storyboard;
            if (startSb != null)
            {
                startSb.Completed +=
                    (o, ev) =>
                    {
                        startSb.Stop();
                        MainFrame.Navigate(FlowPages.CurrentPage());
                    };
                startSb.Begin();
            }
        }

        public static void ChangeTitle(string resKey)
        {
            Instance.TitleText.Text = App.GetText(resKey);
        }
    }
}