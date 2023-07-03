using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Text;
using Windows.UI.Xaml.Data;

namespace ElishAppDesktop.Helpers
{
    public class IsNotConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class EnumToStringConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }



}
