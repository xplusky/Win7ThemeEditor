#region

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Threading;

#endregion

namespace Win7ThemeEditor.ColorPicker
{
    public class ColorComboBox : Control
    {
        #region Public Methods

        static ColorComboBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorComboBox), new FrameworkPropertyMetadata(typeof(ColorComboBox)));

            EventManager.RegisterClassHandler(typeof(ColorComboBox), ColorPicker.SelectedColorChangedEvent, new RoutedPropertyChangedEventHandler<Color>(OnColorPickerSelectedColorChanged));
        }

        #endregion

        #region Dependency Properties

        public static readonly DependencyProperty IsDropDownOpenProperty =
            DependencyProperty.Register("IsDropDownOpen", typeof(bool), typeof(ColorComboBox),
                new UIPropertyMetadata(false, OnIsDropDownOpenChanged));

        public static readonly DependencyProperty SelectedColorProperty =
            DependencyProperty.Register("SelectedColor", typeof(Color), typeof(ColorComboBox),
                new UIPropertyMetadata(Colors.Transparent, OnSelectedColorPropertyChanged));

        public bool IsDropDownOpen
        {
            get { return (bool)GetValue(IsDropDownOpenProperty); }
            set { SetValue(IsDropDownOpenProperty, value); }
        }

        public Color SelectedColor
        {
            get { return (Color)GetValue(SelectedColorProperty); }
            set { SetValue(SelectedColorProperty, value); }
        }

        #endregion

        #region Handling Events

        private static void OnIsDropDownOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var colorComboBox = d as ColorComboBox;
            var newValue = (bool)e.NewValue;

            // Mask HistTest visibility of toggle button otherwise when pressing it
            // and popup is open the popup is closed (since StaysOpen is false)
            // and reopens immediately
            if (colorComboBox != null && colorComboBox._mToggleButton != null)
            {
                colorComboBox.Dispatcher.BeginInvoke(
                    DispatcherPriority.Normal,
                    new Action(
                        delegate {
                                     colorComboBox._mToggleButton.IsHitTestVisible = !newValue;
                        }
                        ));
            }
            //AppDebug.Log("OnIsDropDownOpenChanged - Popup Focused {0} {1}",
            //    colorComboBox.m_popup.IsFocused, colorComboBox.m_popup.IsKeyboardFocused);
        }

        private static void OnSelectedColorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var colorComboBox = d as ColorComboBox;

            if (colorComboBox != null && colorComboBox._mWithinChange)
                return;

            if (colorComboBox != null)
            {
                colorComboBox._mWithinChange = true;
                if (colorComboBox._mColorPicker != null)
                    colorComboBox._mColorPicker.SelectedColor = colorComboBox.SelectedColor;
                colorComboBox._mWithinChange = false;
            }
        }

        private static void OnColorPickerSelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color> e)
        {
            var colorComboBox = sender as ColorComboBox;

            if (colorComboBox != null && colorComboBox._mWithinChange)
                return;

            if (colorComboBox != null) 
            {
                colorComboBox._mWithinChange = true;
                if (colorComboBox._mColorPicker != null)
                    colorComboBox.SelectedColor = colorComboBox._mColorPicker.SelectedColor;
                colorComboBox._mWithinChange = false;
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _mPopup        = GetTemplateChild("PART_Popup") as UIElement;
            _mColorPicker  = GetTemplateChild("PART_ColorPicker") as ColorPicker;
            _mToggleButton = GetTemplateChild("PART_ToggleButton") as ToggleButton;

            if (_mColorPicker != null)
                _mColorPicker.SelectedColor = SelectedColor;
        }


        #endregion

        private ColorPicker  _mColorPicker;
        private UIElement    _mPopup;
        private ToggleButton _mToggleButton;
        private bool         _mWithinChange;
    }
}
