#region

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

#endregion

namespace Win7ThemeEditor.ColorPicker
{
    [ValueConversion(typeof(Color), typeof(String))]
    public class ColorToStringConverter : IValueConverter
    {
        #region IValueConverter Members

        public Object Convert(
            Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            var colorValue = (Color)value;
            return ColorNames.GetColorName(colorValue);
        }
        public Object ConvertBack(
            Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            //throw new NotImplementedException();
            return null;
        }

        #endregion
    }

    public /*internal*/ static class ColorNames
    {
        #region Public Methods

        static ColorNames()
        {
            MColorNames = new Dictionary<Color, string>();

            FillColorNames();
        }

        static public String GetColorName(Color colorToSeek)
        {
            return MColorNames.ContainsKey(colorToSeek) ? MColorNames[colorToSeek] : colorToSeek.ToString();
        }

        #endregion

        #region Private Methods

        public static void FillColorNames()
        {
            var colorsType = typeof(Colors);
            var colorsProperties = colorsType.GetProperties();

            foreach (var colorProperty in colorsProperties)
            {
                var colorName = colorProperty.Name;

                var color = (Color)colorProperty.GetValue(null, null);

                // Path - Aqua is the same as Magenta - so we add 1 to red to avoid collision
                if (colorName == "Aqua")
                    color.R++;

                if (colorName == "Fuchsia")
                    color.G++;
                    
                MColorNames.Add(color, colorName);
            }
        }

        #endregion

        static private readonly Dictionary<Color, String> MColorNames;
    }

    public /*internal*/ static class ColorUtils
    {
        public static String[] GetColorNames()
        {
            var colorsType = typeof(Colors);
            var colorsProperties = colorsType.GetProperties();


            var colorNames = new List<String>();
            foreach (var colorProperty in colorsProperties)
            {
                var colorName = colorProperty.Name;
                colorNames.Add(colorName);

            }

            //String[] colorNamesArray = new String[colorNames.Count];
            return colorNames.ToArray();
        }

        public static void FireSelectedColorChangedEvent(UIElement issuer, RoutedEvent routedEvent, Color oldColor, Color newColor)
        {
            var newEventArgs =
                new RoutedPropertyChangedEventArgs<Color>(oldColor, newColor) {RoutedEvent = routedEvent};
            issuer.RaiseEvent(newEventArgs);
        }

        private static Color BuildColor(double red, double green, double blue, double m)
        {
            return Color.FromArgb(
                255, 
                (byte)((red + m) * 255 + 0.5), 
                (byte)((green + m) * 255 + 0.5), 
                (byte)((blue + m) * 255 + 0.5));
        }

        public static void ConvertRgbToHsv(
            Color color, out double hue, out double saturation, out double value)
        {
            var red   = color.R / 255.0;
            var green = color.G / 255.0;
            var blue  = color.B / 255.0;
            var min = Math.Min(red, Math.Min(green, blue));
            var max = Math.Max(red, Math.Max(green, blue));

            value = max;
            var delta = max - min;

            if (Math.Abs(value - 0) < 0.01)
                saturation = 0;
            else
                saturation = delta / max;

            if (Math.Abs(saturation - 0) < 0.01)
                hue = 0;
            else
            {
                if (Math.Abs(red - max) < 0.01)
                    hue = (green - blue) / delta;
                else if (Math.Abs(green - max) < 0.01)
                    hue = 2 + (blue - red) / delta;
                else // blue == max
                    hue = 4 + (red - green) / delta;
            }
            hue *= 60;
            if (hue < 0)
                hue += 360;
        }

        // Converts an HSV color to an RGB color.
        // Algorithm taken from Wikipedia
        public static Color ConvertHsvToRgb(double hue, double saturation, double value)
        {
            var chroma = value * saturation;

            if (Math.Abs(hue - 360) < 0.01)
                hue = 0;

            var hueTag = hue / 60;
            var x = chroma * (1 - Math.Abs(hueTag % 2 - 1));
            var m = value - chroma;
            switch ((int)hueTag)
            {
                case 0:
                    return BuildColor(chroma, x, 0, m);
                case 1:
                    return BuildColor(x, chroma, 0, m);
                case 2:
                    return BuildColor(0, chroma, x, m);
                case 3:
                    return BuildColor(0, x, chroma, m);
                case 4:
                    return BuildColor(x, 0, chroma, m);
                default:
                    return BuildColor(chroma, 0, x, m);
            }
        }

        public static Color[] GetSpectrumColors(int colorCount)
        {
            var spectrumColors = new Color[colorCount];
            for (var i = 0; i < colorCount; ++i)
            {
                var hue = (i * 360.0) / colorCount;
                spectrumColors[i] = ConvertHsvToRgb(hue, /*saturation*/1.0, /*value*/1.0);
            }

            return spectrumColors;
        }

        public static bool TestColorConversion()
        {
            for (var i = 0; i <= 0xFFFFFF; ++i)
            {
                var red = (byte)(i & 0xFF);
                var green = (byte)((i & 0xFF00) >> 8);
                var blue = (byte)((i & 0xFF0000) >> 16);
                var originalColor = Color.FromRgb(red, green, blue);

                double hue, saturation, value;
                ConvertRgbToHsv(originalColor, out hue, out saturation, out value);

                var resultColor = ConvertHsvToRgb(hue, saturation, value);
                if (originalColor != resultColor)
                    return false;
            }
            return true;
        }
    }
}
