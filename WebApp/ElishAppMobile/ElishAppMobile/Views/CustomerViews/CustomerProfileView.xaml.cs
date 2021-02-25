using ElishAppMobile.Models;
using ElishAppMobile.ViewModels;
using Newtonsoft.Json;
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

        public CustomerProfileView(Customer cust)
        {
            InitializeComponent();
            BindingContext = new CustomerProfileViewModel(map, cust);
        }
    }


    public class CustomerProfileViewModel:BaseViewModel
    {
        private CancellationTokenSource cts;

        public Command SaveCommand { get; }
        public Command SetLocationCommand { get; }

        private Xamarin.Forms.Maps.Map _map;
        private Customer cust;

        public Customer Model
        {
            get { return cust; }
            set { SetProperty(ref cust , value); }
        }


        public CustomerProfileViewModel(Xamarin.Forms.Maps.Map map)
        {
            IsBusy = true;
            SaveCommand = new Command(SaveAction);
            SetLocationCommand = new Command(SetLocation);
            _map = map;
            var profile = Account.GetProfile().Result;
            Model = Customers.Get(profile.Id).Result;
            Load();
        }

        public CustomerProfileViewModel(Xamarin.Forms.Maps.Map map, Customer cust)
        {
            IsBusy = true;
            SaveCommand = new Command(SaveAction);
            SetLocationCommand = new Command(SetLocation);
            _map = map;
            Model = cust;
            Load();
        }

        private async void SaveAction(object obj)
        {
            try
            {
                IsBusy = true;
                var updated = await Customers.Update(Model.Id,Model);
                if (updated)
                {
                   await MessageHelper.InfoAsync("Update Berhasil  !");
                }
                else
                {
                    throw new SystemException("Update Tidak Berhasil !");
                }
                IsBusy = false;
            }
            catch (Exception ex)
            {
               await MessageHelper.ErrorAsync(ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

      

        private async void SetLocation(object obj)
        {
            var request = new GeolocationRequest(GeolocationAccuracy.High, TimeSpan.FromSeconds(10));
            cts = new CancellationTokenSource();
            var location = await Geolocation.GetLocationAsync(request, cts.Token);
            if (location != null)
            {
                var newMap = new MapSpan(new Position(location.Latitude, location.Longitude), 0.01, 0.01);
                SetPin(Model.Name, PinType.Place, new Position(location.Latitude, location.Longitude));

                var customer = new Customer() { Id = Model.Id, Location = $"{location.Latitude}, {location.Longitude}" };

              await  Customers.UpdateLocation(customer);
                _map.MoveToRegion(newMap);
            }
        }

    

        private async void Load()
        {
            try
            {
                
                var request = new GeolocationRequest(GeolocationAccuracy.High, TimeSpan.FromSeconds(10));
                cts = new CancellationTokenSource();
                var location = await Geolocation.GetLocationAsync(request, cts.Token);
                if (location != null)
                {
                    var newMap = new MapSpan(new Position(location.Latitude, location.Longitude), 0.01, 0.01);

                    if (!string.IsNullOrEmpty(Model.Location))
                    {
                        var LocationView = Helper.GetLocationView(Model.Location);
                        var post = new Position(LocationView.Item1, LocationView.Item2);
                        SetPin(Model.Name, PinType.Place, post);
                        newMap = new MapSpan(post, 0.01, 0.01);
                    }
                    _map.MoveToRegion(newMap);
                }
            }
            catch (Exception)
            {

                throw;
            }finally
            {
                IsBusy = false;
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