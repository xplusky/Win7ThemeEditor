#region

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

#endregion

namespace Win7ThemeEditor.ColorPicker
{
    //[ValueConversion(typeof(double), typeof(Color))]
    public class HueToColorConverter : IValueConverter
    {
        #region IValueConverter Members

        public Object Convert(
            Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            var doubleValue = (double)value;

            return ColorUtils.ConvertHsvToRgb(doubleValue, 1, 1);
        }
        public Object ConvertBack(
            Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class HsvControl : Control
    {
        #region Public Methods

        static HsvControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(HsvControl), new FrameworkPropertyMetadata(typeof(HsvControl)));

            // Register Event Handler for the Thumb 
            EventManager.RegisterClassHandler(typeof(HsvControl), Thumb.DragDeltaEvent, new DragDeltaEventHandler(OnThumbDragDelta));
        }

        #endregion

        #region Dependency Properties

        public static readonly DependencyProperty HueProperty = 
            DependencyProperty.Register("Hue", typeof(double), typeof(HsvControl), 
                new UIPropertyMetadata((double)0, OnHueChanged));

        public static readonly DependencyProperty SaturationProperty = 
            DependencyProperty.Register("Saturation", typeof(double), typeof(HsvControl),
                new UIPropertyMetadata((double)0, OnSaturationChanged));

        public static readonly DependencyProperty ValueProperty = 
            DependencyProperty.Register("Value", typeof(double), typeof(HsvControl),
                new UIPropertyMetadata((double)0, OnValueChanged));

        public static readonly DependencyProperty SelectedColorProperty =
            DependencyProperty.Register("SelectedColor", typeof(Color), typeof(HsvControl), new UIPropertyMetadata(Colors.Transparent));

        public double Hue
        {
            get { return (double)GetValue(HueProperty); }
            set { SetValue(HueProperty, value); }
        }

        public double Saturation
        {
            get { return (double)GetValue(SaturationProperty); }
            set { SetValue(SaturationProperty, value); }
        }

        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public Color SelectedColor
        {
            get { return (Color)GetValue(SelectedColorProperty); }
            set { SetValue(SelectedColorProperty, value); }
        }

        #endregion

        #region Routed Events

        public static readonly RoutedEvent SelectedColorChangedEvent = EventManager.RegisterRoutedEvent(
            "SelectedColorChanged",
            RoutingStrategy.Bubble,
            typeof(RoutedPropertyChangedEventHandler<Color>),
            typeof(HsvControl)
            );

        public event RoutedPropertyChangedEventHandler<Color> SelectedColorChanged
        {
            add { AddHandler(SelectedColorChangedEvent, value); }
            remove { RemoveHandler(SelectedColorChangedEvent, value); }
        }  

        #endregion

        #region Event Handlers

        private void OnThumbDragDelta(DragDeltaEventArgs e)
        {
            var offsetX = _mThumbTransform.X + e.HorizontalChange;
            var offsetY = _mThumbTransform.Y + e.VerticalChange;

            UpdatePositionAndSaturationAndValue(offsetX, offsetY);
        }

        private static void OnThumbDragDelta(object sender, DragDeltaEventArgs e)
        {
            var hsvControl = sender as HsvControl;
            if (hsvControl != null) hsvControl.OnThumbDragDelta(e);
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (_mThumb != null)
            {
                var position = e.GetPosition(this);

                UpdatePositionAndSaturationAndValue(position.X, position.Y);

                // Initiate mouse event on thumb so it will start drag
                _mThumb.RaiseEvent(e);
            }

            base.OnMouseLeftButtonDown(e);
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            UpdateThumbPosition();

            base.OnRenderSizeChanged(sizeInfo);
        }

        private static void OnHueChanged(
            DependencyObject relatedObject, DependencyPropertyChangedEventArgs e)
        {
            var hsvControl = relatedObject as HsvControl;
            if (hsvControl != null)
                hsvControl.UpdateSelectedColor();
        }

        private static void OnSaturationChanged(
            DependencyObject relatedObject, DependencyPropertyChangedEventArgs e)
        {
            var hsvControl = relatedObject as HsvControl;
            if (hsvControl != null)
                hsvControl.UpdateThumbPosition();
        }

        private static void OnValueChanged(
            DependencyObject relatedObject, DependencyPropertyChangedEventArgs e)
        {
            var hsvControl = relatedObject as HsvControl;
            if (hsvControl != null)
                hsvControl.UpdateThumbPosition();
        }

        #endregion

        private const string ThumbName = "PART_Thumb";

        private readonly TranslateTransform  _mThumbTransform = new TranslateTransform();
        private Thumb               _mThumb;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _mThumb = GetTemplateChild(ThumbName) as Thumb;
            if (_mThumb != null)
            {
                UpdateThumbPosition();
                _mThumb.RenderTransform = _mThumbTransform;
            }
        }

        #region Private Methods

        // Limit value to range (0 , max] 
        private double LimitValue(double value, double max)
        {
            if (value < 0)
                value = 0;
            if (value > max)
                value = max;
            return value;
        }

        private void UpdateSelectedColor()
        {
            var oldColor = SelectedColor;
            var newColor = ColorUtils.ConvertHsvToRgb(Hue, Saturation, Value);

            SelectedColor = newColor;
            ColorUtils.FireSelectedColorChangedEvent(this, SelectedColorChangedEvent, oldColor, newColor);
        }

        private void UpdatePositionAndSaturationAndValue(double positionX, double positionY)
        {
            positionX = LimitValue(positionX, ActualWidth);
            positionY = LimitValue(positionY, ActualHeight);

            _mThumbTransform.X = positionX;
            _mThumbTransform.Y = positionY;

            Saturation = positionX / ActualWidth;
            Value      = 1 - positionY / ActualHeight;

            UpdateSelectedColor();
        }

        private void UpdateThumbPosition()
        {
            _mThumbTransform.X = Saturation * ActualWidth;
            _mThumbTransform.Y = (1 - Value) * ActualHeight;

            SelectedColor = ColorUtils.ConvertHsvToRgb(Hue, Saturation, Value);
        }

        #endregion
    }
}
