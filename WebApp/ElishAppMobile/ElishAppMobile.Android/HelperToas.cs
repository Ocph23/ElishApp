using System.Threading.Tasks;
using Android.App;
using Android.Widget;
using ElishAppMobile.Droid;
using ElishAppMobile.Helpers;

[assembly: Xamarin.Forms.Dependency(typeof(HelperToas))]
namespace ElishAppMobile.Droid
{
    public class HelperToas : IToas
    {
        public Task ShowLong(string message)
        {
            Toast.MakeText(Application.Context, message, ToastLength.Long).Show();
             return Task.CompletedTask;
        }

        public Task ShowShort(string message)
        {
            Toast.MakeText(Application.Context, message, ToastLength.Short).Show();
            return Task.CompletedTask;
        }
    }
}