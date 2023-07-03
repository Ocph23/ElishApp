using ApsMobileApp.ViewModels;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using ShareModels;

namespace ApsMobileApp.Views.CustomerViews;


[QueryProperty(nameof(Customer),"Customer")]
public partial class CustomerProfileView : ContentPage
{

    private Customer customer;

    public Customer Customer
    {
        get { return customer; }
        set { customer = value;
            var vm = BindingContext as CustomerProfileViewModel;
            _=vm.SetCustomer(value);
            OnPropertyChanged();
        }
    }


    public CustomerProfileView(CustomerProfileViewModel vm)
    {
        InitializeComponent();
        vm.Load(maps); 
        BindingContext = vm;

    }



}


public class CustomerProfileViewModel:BaseViewModel
{
    private CancellationTokenSource cts;

    public Command SaveCommand { get; }
    public Command SetLocationCommand { get; }

    private Microsoft.Maui.Controls.Maps.Map _map;
    private readonly ICustomerService CustomerService;

    public Array MapTypes { get; }

    private Customer cust;
    private MapType _mapType;

    public Customer Model
    {
        get { return cust; }
        set { SetProperty(ref cust , value); }
    }

    public MapType MapSelected
    {
        get => _mapType;
        set { SetProperty(ref _mapType, value); _map.MapType = value; }
    }

    public CustomerProfileViewModel(ICustomerService customerService)
    {
        IsBusy = true;
        SaveCommand = new Command(SaveAction);
        SetLocationCommand = new Command(SetLocation);
        this.CustomerService = customerService;
        MapTypes = Enum.GetValues(typeof(MapType));

    }

    private async void SaveAction(object obj)
    {
        try
        {
            IsBusy = true;
            var updated = await CustomerService.Update(Model.Id,Model);
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
            var newMap = new MapSpan(new Location(location.Latitude, location.Longitude), 0.01, 0.01);
            SetPin(Model.Name, PinType.Place, new Location(location.Latitude, location.Longitude));

            var customer = new Customer() { Id = Model.Id, Location = $"{location.Latitude}; {location.Longitude}" };

          await  CustomerService.UpdateLocation(customer);
            Model.Location = customer.Location;
            _map.MoveToRegion(newMap);
        }
    }

    public async Task Load(Microsoft.Maui.Controls.Maps.Map map, Customer customer=null)
    {
        try
        {
            _map = map;
            _map.MapClicked += _map_MapClicked;
            var profile = await Account.GetProfile();
            Model = CustomerService.Get(profile.Id).Result;
            var request = new GeolocationRequest(GeolocationAccuracy.High, TimeSpan.FromSeconds(10));
            cts = new CancellationTokenSource();
            var location = await Geolocation.GetLocationAsync(request, cts.Token);
            if (location != null)
            {
                var newMap = new MapSpan(new Location(location.Latitude, location.Longitude), 0.01, 0.01);
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

    private void _map_MapClicked(object sender, MapClickedEventArgs e)
    {
        _map.Pins.Clear();
        SetPin(Model.Name, PinType.Place, e.Location);
        Model.Location = $"{e.Location.Latitude}; {e.Location.Longitude}";
    }

    private Pin SetPin(string label, PinType pinType, Location position)
    {
        Pin pin = new Pin
        {
            Label = label,
            Type = pinType,
             Location =position
        };

        _map.Pins.Add(pin);
        return pin;
    }

    internal async Task SetCustomer(Customer cust)
    {
        try
        {
            await Task.Delay(2000);
            IsBusy = true;
            Model = cust;
            if (!string.IsNullOrEmpty(Model.Location))
            {
                var LocationView = Helper.GetLocationView(Model.Location);
                var post = new Location(LocationView.Item1, LocationView.Item2);
                SetPin(Model.Name, PinType.Place, post);
                var newMap = new MapSpan(post,0,0);
                _map.MoveToRegion(newMap);
            }
        }
        catch (Exception ex)
        {
          await MessageShow.ErrorAsync(ex.Message);
        }
        finally { IsBusy = false; }
    }

}