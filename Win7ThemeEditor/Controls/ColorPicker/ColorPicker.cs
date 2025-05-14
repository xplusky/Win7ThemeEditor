#region

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;

#endregion

namespace Win7ThemeEditor.ColorPicker
{
    [ValueConversion(typeof(double), typeof(String))]
    public class DoubleToIntegerStringConverter : IValueConverter
    {
        #region IValueConverter Members

        public Object Convert(
            Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            var doubleValue = (double)value;
            var    intValue    = (int)doubleValue;

            return intValue.ToString(CultureInfo.InvariantCulture);
        }
        public Object ConvertBack(
            Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            var stringValue = (String)value;
            double doubleValue;
            if (!Double.TryParse(stringValue, out doubleValue))
                doubleValue = 0;

            return doubleValue;
        }

        #endregion
    }

    public class ColorPicker : Control
    {
        #region Public Methods

        static ColorPicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorPicker), new FrameworkPropertyMetadata(typeof(ColorPicker)));

            // Register Event Handler for slider
            EventManager.RegisterClassHandler(typeof(ColorPicker), RangeBase.ValueChangedEvent, new RoutedPropertyChangedEventHandler<double>(OnSliderValueChanged));

            // Register Event Handler for Hsv Control
            EventManager.RegisterClassHandler(typeof(ColorPicker), HsvControl.SelectedColorChangedEvent, new RoutedPropertyChangedEventHandler<Color>(OnHsvControlSelectedColorChanged));
        }

        #endregion

        #region Dependency Properties

        public static readonly DependencyProperty SelectedColorProperty =
            DependencyProperty.Register("SelectedColor", typeof(Color), typeof(ColorPicker), 
                new UIPropertyMetadata(Colors.Black, OnSelectedColorPropertyChanged));

        // Using a DependencyProperty as the backing store for FixedSliderColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FixedSliderColorProperty =
            DependencyProperty.Register("FixedSliderColor", typeof(bool), typeof(SpectrumSlider),
                new UIPropertyMetadata(false, OnFixedSliderColorPropertyChanged));

        public Color SelectedColor
        {
            get { return (Color)GetValue(SelectedColorProperty); }
            set { SetValue(SelectedColorProperty, value); }
        }

        public bool FixedSliderColor
        {
            get { return (bool)GetValue(FixedSliderColorProperty); }
            set { SetValue(FixedSliderColorProperty, value); }
        }

        #endregion

        #region Routed Events

        public static readonly RoutedEvent SelectedColorChangedEvent = EventManager.RegisterRoutedEvent(
            "SelectedColorChanged",
            RoutingStrategy.Bubble,
            typeof(RoutedPropertyChangedEventHandler<Color>),
            typeof(ColorPicker)
            );

        public event RoutedPropertyChangedEventHandler<Color> SelectedColorChanged
        {
            add { AddHandler(SelectedColorChangedEvent, value); }
            remove { RemoveHandler(SelectedColorChangedEvent, value); }
        }

        #endregion

        #region Event Handling

        private void OnSliderValueChanged(RoutedPropertyChangedEventArgs<double> e)
        {
            // Avoid endless loop
            if (_mWithinChange)
                return;

            _mWithinChange = true;
            if (Equals(e.OriginalSource, _mRedColorSlider) ||
                Equals(e.OriginalSource, _mGreenColorSlider) ||
                Equals(e.OriginalSource, _mBlueColorSlider) ||
                Equals(e.OriginalSource, _mAlphaColorSlider))
            {
                var newColor = GetRgbColor();
                UpdateHsvControlColor(newColor);
                UpdateSelectedColor(newColor);
            }
            else if (Equals(e.OriginalSource, _mSpectrumSlider))
            {
                var hue = _mSpectrumSlider.Hue;
                UpdateHsvControlHue(hue);
                var newColor = GetHsvColor();
                UpdateRgbColors(newColor);
                UpdateSelectedColor(newColor);
            }

            _mWithinChange = false;
        }

        private static void OnSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var colorPicker = sender as ColorPicker;
            if (colorPicker != null) colorPicker.OnSliderValueChanged(e);
        }

        private void OnHsvControlSelectedColorChanged()
        {
            // Avoid endless loop
            if (_mWithinChange)
                return;

            _mWithinChange = true;

            var newColor = GetHsvColor();
            UpdateRgbColors(newColor);
            UpdateSelectedColor(newColor);

            _mWithinChange = false;
        }

        private static void OnHsvControlSelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color> e)
        {
            var colorPicker = sender as ColorPicker;
            if (colorPicker != null) colorPicker.OnHsvControlSelectedColorChanged();
        }

        private void OnSelectedColorPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (!_mTemplateApplied)
                return;

            // Avoid endless loop
            if (_mWithinChange)
                return;

            _mWithinChange = true;

            var newColor = (Color)e.NewValue;
            UpdateControlColors(newColor);

            _mWithinChange = false;
        }

        private static void OnSelectedColorPropertyChanged(
            DependencyObject relatedObject, DependencyPropertyChangedEventArgs e)
        {
            var colorPicker = relatedObject as ColorPicker;
            if (colorPicker != null) colorPicker.OnSelectedColorPropertyChanged(e);
        }

        private static void OnFixedSliderColorPropertyChanged(
            DependencyObject relatedObject, DependencyPropertyChangedEventArgs e)
        {
            var colorPicker = relatedObject as ColorPicker;
            if (colorPicker != null) colorPicker.UpdateColorSlidersBackground();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _mRedColorSlider = GetTemplateChild(RedColorSliderName) as ColorSlider;
            _mGreenColorSlider = GetTemplateChild(GreenColorSliderName) as ColorSlider;
            _mBlueColorSlider = GetTemplateChild(BlueColorSliderName) as ColorSlider;
            _mAlphaColorSlider = GetTemplateChild(AlphaColorSliderName) as ColorSlider;
            _mSpectrumSlider = GetTemplateChild(SpectrumSliderName) as SpectrumSlider;
            _mHsvControl = GetTemplateChild(HsvControlName) as HsvControl;

            _mTemplateApplied = true;
            UpdateControlColors(SelectedColor);

        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.Property == IsVisibleProperty && (bool)e.NewValue)
                Focus();
            base.OnPropertyChanged(e);
        }

        #endregion

        #region Private Methods

        private void SetColorSliderBackground(ColorSlider colorSlider, Color leftColor, Color rightColor)
        {
            colorSlider.LeftColor  = leftColor;
            colorSlider.RightColor = rightColor;
        }

        private void UpdateColorSlidersBackground()
        {
            if (FixedSliderColor)
            {
                SetColorSliderBackground(_mRedColorSlider, Colors.Red, Colors.Red);
                SetColorSliderBackground(_mGreenColorSlider, Colors.Green, Colors.Green);
                SetColorSliderBackground(_mBlueColorSlider, Colors.Blue, Colors.Blue);
                SetColorSliderBackground(_mAlphaColorSlider, Colors.Transparent, Colors.White);
            }
            else
            {
                var red   = SelectedColor.R;
                var green = SelectedColor.G;
                var blue  = SelectedColor.B;
                SetColorSliderBackground(_mRedColorSlider,
                    Color.FromRgb(0, green, blue), Color.FromRgb(255, green, blue));
                SetColorSliderBackground(_mGreenColorSlider,
                    Color.FromRgb(red, 0, blue), Color.FromRgb(red, 255, blue));
                SetColorSliderBackground(_mBlueColorSlider,
                    Color.FromRgb(red, green, 0), Color.FromRgb(red, green, 255));
                SetColorSliderBackground(_mAlphaColorSlider,
                    Color.FromArgb(0, red, green, blue), Color.FromArgb(255, red, green, blue));
            }
        }

        private Color GetRgbColor()
        {
            return Color.FromArgb(
                (byte)_mAlphaColorSlider.Value,
                (byte)_mRedColorSlider.Value,
                (byte)_mGreenColorSlider.Value,
                (byte)_mBlueColorSlider.Value);
        }

        private void UpdateRgbColors(Color newColor)
        {
            _mAlphaColorSlider.Value = newColor.A;
            _mRedColorSlider.Value   = newColor.R;
            _mGreenColorSlider.Value = newColor.G;
            _mBlueColorSlider.Value  = newColor.B;
        }

        private Color GetHsvColor()
        {
            var hsvColor = _mHsvControl.SelectedColor;
            hsvColor.A = (byte)_mAlphaColorSlider.Value;
            return hsvColor;
        }

        private void UpdateSpectrumColor()
        {
        }

        private void UpdateHsvControlHue(double hue)
        {
            _mHsvControl.Hue = hue;
        }


        private void UpdateHsvControlColor(Color newColor)
        {
            double hue, saturation, value;

            ColorUtils.ConvertRgbToHsv(newColor, out hue, out saturation, out value);

            // if saturation == 0 or value == 1 hue don't count so we save the old hue
            if (saturation != 0 && value != 0)
                _mHsvControl.Hue        = hue;

            _mHsvControl.Saturation = saturation;
            _mHsvControl.Value      = value;

            _mSpectrumSlider.Hue = _mHsvControl.Hue;
        }

        private void UpdateSelectedColor(Color newColor)
        {
            var oldColor = SelectedColor;
            SelectedColor = newColor;

            if (!FixedSliderColor)
                UpdateColorSlidersBackground();

            ColorUtils.FireSelectedColorChangedEvent(this, SelectedColorChangedEvent, oldColor, newColor);
        }

        private void UpdateControlColors(Color newColor)
        {
            UpdateRgbColors(newColor);
            UpdateSpectrumColor();
            UpdateHsvControlColor(newColor);
            UpdateColorSlidersBackground();
        }

        #endregion

        private const string RedColorSliderName = "PART_RedColorSlider";
        private const string GreenColorSliderName = "PART_GreenColorSlider";
        private const string BlueColorSliderName  = "PART_BlueColorSlider";
        private const string AlphaColorSliderName = "PART_AlphaColorSlider";

        private const string SpectrumSliderName   = "PART_SpectrumSlider1";
        private const string HsvControlName       = "PART_HsvControl";

        private ColorSlider     _mAlphaColorSlider;
        private ColorSlider     _mBlueColorSlider;
        private ColorSlider     _mGreenColorSlider;

        private HsvControl      _mHsvControl;
        private ColorSlider     _mRedColorSlider;
        private SpectrumSlider  _mSpectrumSlider;

        private bool            _mTemplateApplied;
        private bool            _mWithinChange;
    }
}
