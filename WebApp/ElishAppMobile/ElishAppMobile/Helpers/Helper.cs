using ElishAppMobile.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace ElishAppMobile
{
    public class Helper
    {
        //public static string Url { get; set; } = "http://192.168.1.2";
        public static string  Url { get; set; } = "https://waena-desa.id";

        internal static Tuple<bool, NetworkAccess> CheckInterNetConnection()
        {
            var current = Connectivity.NetworkAccess;

            if (current == NetworkAccess.Internet)
            {
                return Tuple.Create(true, current);
            }

            DependencyService.Get<IToas>().ShowLong("Tidak Ada Koneksi Internet !");

            return Tuple.Create(false, current);
        }
    }

    public class IMageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!string.IsNullOrEmpty(value.ToString()))
            {
                return Helper.Url + "/" + value.ToString();
            }
            return string.Empty;
        }
        
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class MyDatePicker : DatePicker
    {
        private string _format = null;
        #pragma warning disable CS0618 // Type or member is obsolete
                public static readonly BindableProperty NullableDateProperty = BindableProperty.Create<MyDatePicker, DateTime?>(p => p.NullableDate, null);
        #pragma warning restore CS0618 // Type or member is obsolete

        public DateTime? NullableDate
        {
            get { return (DateTime?)GetValue(NullableDateProperty); }
            set { SetValue(NullableDateProperty, value); UpdateDate(); }
        }

        private void UpdateDate()
        {
            if (NullableDate.HasValue) { if (null != _format) Format = _format; Date = NullableDate.Value; }
            else { _format = Format; Format = "pick ..."; }
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            UpdateDate();
        }

        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            if (propertyName == "Date") NullableDate = Date;
        }
    }
}
