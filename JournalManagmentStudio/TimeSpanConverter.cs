using System;
using System.Windows.Data;

namespace JournalManagmentStudio.Journal
{
    [ValueConversion(typeof(bool), typeof(bool))]
    public class InverseBooleanConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(bool))
                throw new InvalidOperationException("The target must be a boolean");

            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion
    }

    [ValueConversion(typeof(object), typeof(string))]
    public class TimeSpanConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            TimeSpan time = TimeSpan.FromSeconds(Math.Round(System.Convert.ToDouble(value) / 0.00001157407, 2));
            return System.Convert.ToDouble(value) >= 1 ? time.ToString(@"d\д\ hh\:mm\:ss") : time.ToString(@"hh\:mm\:ss");
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            return System.Convert.ToDecimal(TimeSpan.Parse(((string)value).Replace("д ", ".")).TotalSeconds * 0.00001157407);
        }

        #endregion
    }

    [ValueConversion(typeof(object), typeof(string))]
    public class StringTrimConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            return value.ToString().Trim();
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            return value;
        }

        #endregion
    }
}
