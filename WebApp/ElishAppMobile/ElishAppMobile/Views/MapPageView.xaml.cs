using ElishAppMobile.ViewModels;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace ElishAppMobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MapPageView : ContentPage
    {
        private MapPageViewModel vm;

        public MapPageView()
        {
            InitializeComponent();
            BindingContext =vm= new MapPageViewModel(map);
        }

        private async void isMyMap_Toggled(object sender, ToggledEventArgs e)
        {
            await  vm.Load(e.Value);
        }
    }


    public class MapPageViewModel : BaseViewModel
    {
        private MapType _mapType;
        private CancellationTokenSource cts;

        public MapPageViewModel(Map map)
        {
            this.Map = map;
            _ = Load();
            MapTypes = Enum.GetValues(typeof(MapType));
            MapSelected = MapType.Satellite;
        }

        public async Task Load(bool isMyMap=false)
        {
            _=  MyCenterPosition();
            Map.Pins.Clear();
            if (!isMyMap)
            {
                var customers = await Customers.Get();
                foreach (var item in customers)
                {
                    var location = Helper.GetLocationView(item.Location);
                    var position = new Position(location.Item1, location.Item2);
                    SetPin(item.Name, PinType.Place, position);
                }
            }
            else
            {
                var profile = await Account.GetProfile();
                var customers = await Customers.Get();
                foreach (var item in customers.Where(x=>x.KaryawanId==profile.Id))
                {
                    var location = Helper.GetLocationView(item.Location);
                    var position = new Position(location.Item1, location.Item2);
                    SetPin(item.Name, PinType.Place, position);
                }
            }

        }

        private async Task MyCenterPosition()
        {
            var request = new Xamarin.Essentials.GeolocationRequest(Xamarin.Essentials.GeolocationAccuracy.High, TimeSpan.FromSeconds(10));
            cts = new CancellationTokenSource();
            var location = await Xamarin.Essentials.Geolocation.GetLocationAsync(request, cts.Token);
            if (location != null)
            {
                var newMap = new MapSpan(new Position(location.Latitude, location.Longitude), 0.01, 0.01);
                Map.MoveToRegion(newMap);
            }
        }

        public Map Map { get; }
        public Array MapTypes { get; }

        public MapType MapSelected
        {
            get => _mapType;
            set { SetProperty(ref _mapType, value); Map.MapType = value; }
        }

        private Pin SetPin(string label, PinType pinType, Position position)
        {
            Pin pin = new Pin
            {
                Label = label,
                Type = pinType,
                Position = position,
            };

            Map.MapType = MapType.Satellite;
            Map.Pins.Add(pin);
            return pin;
        }
    }
}