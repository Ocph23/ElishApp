using ElishAppMobile.Models;
using ElishAppMobile.ViewModels;
using ShareModels;
using System;
using System.Threading;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace ElishAppMobile.Views.CustomerViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CustomerProfileView : ContentPage
    {
        public CustomerProfileView()
        {
            InitializeComponent();
            BindingContext = new CustomerProfileViewModel(map);
        }
    }


    public class CustomerProfileViewModel:BaseViewModel
    {
        private CancellationTokenSource cts;

        public Command SetLocationCommand { get; }

        private Xamarin.Forms.Maps.Map _map;

        public Profile MyProfile { get; set; }
        public CustomerProfileViewModel(Xamarin.Forms.Maps.Map map)
        {

            SetLocationCommand = new Command(SetLocation);
            _map = map;
            MyProfile = Account.GetProfile().Result;
            Load();
        }

        private async void SetLocation(object obj)
        {
            var request = new GeolocationRequest(GeolocationAccuracy.High, TimeSpan.FromSeconds(10));
            cts = new CancellationTokenSource();
            var location = await Geolocation.GetLocationAsync(request, cts.Token);
            if (location != null)
            {
                var newMap = new MapSpan(new Position(location.Latitude, location.Longitude), 0.01, 0.01);
                SetPin(MyProfile.Name, PinType.Place, new Position(location.Latitude, location.Longitude));

                var customer = new Customer() { Id = MyProfile.Id, Location = $"{location.Latitude}, {location.Longitude}" };

                Customers.UpdateLocation(customer);
                _map.MoveToRegion(newMap);
            }
        }

    

        private async void Load()
        {
            var request = new GeolocationRequest(GeolocationAccuracy.High, TimeSpan.FromSeconds(10));
            cts = new CancellationTokenSource();
            var location = await Geolocation.GetLocationAsync(request, cts.Token);
            if (location != null)
            {
                var newMap = new MapSpan(new Position(location.Latitude, location.Longitude), 0.01, 0.01);

                if (!string.IsNullOrEmpty(MyProfile.Location))
                {
                    var post = new Position(MyProfile.LocationView.Item1, MyProfile.LocationView.Item2);
                    SetPin(MyProfile.Name, PinType.Place, post);
                    newMap = new MapSpan(post, 0.01, 0.01);
                }
               _map.MoveToRegion(newMap);
            }
        }

        private Pin SetPin(string label, PinType pinType, Position position)
        {
            Pin pin = new Pin
            {
                Label = label,
                Type = pinType,
                Position =position
            };

            _map.Pins.Add(pin);
            return pin;
        }
    }
}