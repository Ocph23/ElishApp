using ApsMobileApp.ViewModels;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using ShareModels;
namespace ApsMobileApp.Views;

public partial class MapPageView : ContentPage
{

    public MapPageView(MapPageViewModel vm)
    {
        InitializeComponent();
        vm.Load(maps);
        BindingContext = vm;
    }

    private async void isMyMap_Toggled(object sender, ToggledEventArgs e)
    {
        var vm = BindingContext as MapPageViewModel;
        await vm.Load(maps, e.Value);
    }
}


public class MapPageViewModel : BaseViewModel
{
    private MapType _mapType=MapType.Street;
    private CancellationTokenSource cts;
    private readonly ICustomerService customerService;

    public MapPageViewModel(ICustomerService customerService)
    {
        this.customerService = customerService;
        MapTypes = Enum.GetValues(typeof(MapType));


    }

    public async Task Load(Microsoft.Maui.Controls.Maps.Map map, bool isMyMap = false)
    {
        if (Map == null)
            Map = map;

        _ = MyCenterPosition();
        Map.Pins.Clear();
        if (!isMyMap)
        {
            var customers = await customerService.Get();
            foreach (var item in customers)
            {
                var location = Helper.GetLocationView(item.Location);
                if (location != null)
                {
                    var position = new Location(location.Item1, location.Item2);
                    SetPin(item.Name, PinType.Place, position);
                }
            }
        }
        else
        {
            if (await Account.UserInRole("Customer"))
            {
                var profile = await Account.GetProfile();
                var customers = await customerService.Get();
                foreach (var item in customers.Where(x => x.Karyawan.Id == profile.Id))
                {
                    var location = Helper.GetLocationView(item.Location);
                    var position = new Location(location.Item1, location.Item2);
                    SetPin(item.Name, PinType.Place, position);
                }
            }
        }

    }

    private async Task MyCenterPosition()
    {
        var request = new GeolocationRequest(GeolocationAccuracy.High, TimeSpan.FromSeconds(10));
        cts = new CancellationTokenSource();
        var location = await Geolocation.GetLocationAsync(request, cts.Token);
        if (location != null)
        {
            var newMap = new MapSpan(new Location(location.Latitude, location.Longitude), 0.01, 0.01);
            Map.MoveToRegion(newMap);
        }
    }

    public Microsoft.Maui.Controls.Maps.Map Map { get; set; }
    public Array MapTypes { get; }

    public MapType MapSelected
    {
        get => _mapType;
        set { SetProperty(ref _mapType, value); Map.MapType = value; }
    }

    private Pin SetPin(string label, PinType pinType, Location position)
    {
        Pin pin = new Pin
        {
            Label = label,
            Type = pinType,
            Location = position,
        };
        Map.Pins.Add(pin);
        return pin;
    }
}