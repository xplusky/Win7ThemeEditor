#region

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

#endregion

namespace Win7ThemeEditor.ColorPicker
{
    public class SpectrumSlider : Slider
    {
        #region Public Methods

        static SpectrumSlider()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SpectrumSlider), new FrameworkPropertyMetadata(typeof(SpectrumSlider)));

        }

        public SpectrumSlider()
        {
            SetBackground();

            //Binding binding = new Binding();
            //binding.Path = new PropertyPath("Value");
            //binding.RelativeSource = new System.Windows.Data.RelativeSource(RelativeSourceMode.Self);
            //binding.Mode = BindingMode.TwoWay;
            //binding.Converter = new ValueToSelectedColorConverter();

            //BindingOperations.SetBinding(this, SelectedColorProperty, binding);
        }

        #endregion

        #region Protected Methods

        protected override void OnValueChanged(double oldValue, double newValue)
        {
            base.OnValueChanged(oldValue, newValue);

            if (_mWithinChanging || BindingOperations.IsDataBound(this, HueProperty)) return;
            _mWithinChanging = true;
            Hue = 360 - newValue;
            _mWithinChanging = false;
        }

        #endregion

        #region Private Methods

        private void SetBackground()
        {
            var backgroundBrush = new LinearGradientBrush
            {
                StartPoint = new Point(0.5, 0), 
                EndPoint = new Point(0.5, 1)
            };

            const int spectrumColorCount = 30;
     
            var spectrumColors = ColorUtils.GetSpectrumColors(spectrumColorCount);
            for (var i = 0; i < spectrumColorCount; ++i)
            {
                var offset = i * 1.0 / spectrumColorCount;
                var gradientStop = new GradientStop(spectrumColors[i], offset);
                backgroundBrush.GradientStops.Add(gradientStop);
            }
            Background = backgroundBrush;
        }

        private static void OnHuePropertyChanged(
            DependencyObject relatedObject, DependencyPropertyChangedEventArgs e)
        {
            var spectrumSlider = relatedObject as SpectrumSlider;
            if (spectrumSlider == null || spectrumSlider._mWithinChanging) return;
            spectrumSlider._mWithinChanging = true;

            var hue = (double)e.NewValue;
            spectrumSlider.Value = 360 - hue;

            spectrumSlider._mWithinChanging = false;
        }

        #endregion

        #region Dependency Properties

        public static readonly DependencyProperty HueProperty =
            DependencyProperty.Register("Hue", typeof(double), typeof(SpectrumSlider),
                new UIPropertyMetadata((double)0, OnHuePropertyChanged));

        public double Hue
        {
            get { return (double)GetValue(HueProperty); }
            set { SetValue(HueProperty, value); }
        }

        #endregion

        private bool _mWithinChanging;
    }
}
