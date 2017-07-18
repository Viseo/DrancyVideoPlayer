using System;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Data;

namespace MediaPlayer.Tools
{
    public class MinutesFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var unit = new ResourceLoader().GetString("minuteUnit");
            var stringValue = value as string;

            if (value == null)
                return null;

            if (stringValue != null && stringValue.Contains(unit))
            {
                var cleanedValue = stringValue.Replace(unit, "");
                return cleanedValue + " " + unit;
            }
            return value + " " + unit;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            var unit = new ResourceLoader().GetString("minuteUnit");
            var stringValue = value as string;

            if (stringValue != null && stringValue.Contains(unit))
                value = stringValue.Replace(unit, "");

            int n;
            bool isNumeric = int.TryParse(value.ToString(), out n);

            if (isNumeric)
                return n.ToString();
            return "";
        }
    }

    public class SecondsFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var unit = new ResourceLoader().GetString("secondUnit");
            var stringValue = value as string;

            if (value == null)
                return null;

            if (stringValue != null && stringValue.Contains(unit))
            {
                var cleanedValue = stringValue.Replace(unit, "");
                return cleanedValue + " " +  unit;
            }

            return value + " " + unit;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            var unit = new ResourceLoader().GetString("secondUnit");
            var stringValue = value as string;

            if (stringValue != null && stringValue.Contains(unit))
                value = stringValue.Replace(unit, "");

            int n;
            bool isNumeric = int.TryParse(value.ToString(), out n);

            if (isNumeric)
                return n.ToString();
            return "";
        }
    }
}
