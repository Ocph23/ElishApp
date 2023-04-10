using ApsMobileApp.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ApsMobileApp;

public class Helper
{
  //  public static string Url { get; set; } = "https://rr6cngw0-7232.asse.devtunnels.ms";
    public static string  Url { get; set; } = "https://apspapua.com";
    public static JsonSerializerOptions JsonOption { get; set; } = new () { PropertyNameCaseInsensitive = true, ReferenceHandler = ReferenceHandler.Preserve };

    internal static Tuple<bool, NetworkAccess> CheckInterNetConnection()
    {
        var current = Connectivity.NetworkAccess;

        if (current == NetworkAccess.Internet)
        {
            return Tuple.Create(true, current);
        }

        _ = Toas.ShowLong("Tidak Ada Koneksi Internet !");
        return Tuple.Create(false, current);
    }

    public static Tuple<double, double> GetLocationView(string location)
    {
        if (string.IsNullOrEmpty(location))
            return null;
        else
        {
            var datas = location.Split(';');
            return Tuple.Create(Convert.ToDouble(datas[0]), Convert.ToDouble(datas[1]));
        }
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
    //private string _format = null;
    //#pragma warning disable CS0618 // Type or member is obsolete
    //        public static readonly BindableProperty NullableDateProperty = BindableProperty.Create<MyDatePicker, DateTime?>(p => p.NullableDate, null);
    //#pragma warning restore CS0618 // Type or member is obsolete

    //public DateTime? NullableDate
    //{
    //    get { return (DateTime?)GetValue(NullableDateProperty); }
    //    set { SetValue(NullableDateProperty, value); UpdateDate(); }
    //}

    //private void UpdateDate()
    //{
    //    if (NullableDate.HasValue) { if (null != _format) Format = _format; Date = NullableDate.Value; }
    //    else { _format = Format; Format = "pick ..."; }
    //}

    //protected override void OnBindingContextChanged()
    //{
    //    base.OnBindingContextChanged();
    //    UpdateDate();
    //}

    //protected override void OnPropertyChanged(string propertyName = null)
    //{
    //    base.OnPropertyChanged(propertyName);
    //    if (propertyName == "Date") NullableDate = Date;
    //}
}


