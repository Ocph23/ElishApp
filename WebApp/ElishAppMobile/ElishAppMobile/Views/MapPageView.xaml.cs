using ElishAppMobile.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace ElishAppMobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MapPageView : ContentPage
    {
        public MapPageView()
        {
            InitializeComponent();
            BindingContext = new MapPageViewModel(map);


        }



      
    }


    public class MapPageViewModel : BaseViewModel
    {
        private MapType _mapType;

        public MapPageViewModel(Map map)
        {
            this.Map = map;
            _ = Load();
            MapTypes = Enum.GetValues(typeof(MapType));
            MapSelected = MapType.Satellite;
        }

        private async Task Load()
        {
            var customers =  await Customers.Get();
            foreach (var item in customers)
            {
                var location = Helper.GetLocationView(item.Location);
                var position = new Position(location.Item1, location.Item2);
                SetPin(item.Name, PinType.Place, position);
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