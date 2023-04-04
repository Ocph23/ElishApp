using ApsMobileApp.Models.Messages;
using ApsMobileApp.ViewModels;
using CommunityToolkit.Mvvm.Messaging;
using ShareModels;
using ShareModels.ModelViews;
using System.Collections.ObjectModel;

namespace ApsMobileApp.Views.PenjualanViews;

[QueryProperty("Penjualan", "Penjualan")]
public partial class PenjualanPembayaranView : ContentPage
{

    private PenjualanAndOrderModel model;

    public PenjualanAndOrderModel Penjualan
    {
        get { return model; }
        set
        {
            model = value;
            var vm = BindingContext as PenjualanPembayaranViewModel;
            vm.SetPenjualan(value);
            OnParentChanged();
        }

    }


    public PenjualanPembayaranView(PenjualanPembayaranViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}


public class PenjualanPembayaranViewModel : BaseViewModel
{
    private readonly IPenjualanService penjualanService;

    public ObservableCollection<PembayaranPenjualan> Items { get; }

    public PenjualanPembayaranViewModel(IPenjualanService _penjualanService)
    {
        Items = new ObservableCollection<PembayaranPenjualan>();
        LoadItemCommand = new Command(async (x) => await LoadItemAction(x));
        AddCommand = new Command(async (x) => await AddAction(x), AddCommandValidation);
        penjualanService = _penjualanService;
        WeakReferenceMessenger.Default.Register<Models.Messages.PembayaranPenjualanChangeMessage>(this, (r, m) =>
        {
            _ = SavePembayaran(m.Value);
        });


        this.PropertyChanged += (_, __) =>
        {
            if (__.PropertyName == "Model")
            {
                AddCommand = new Command(async (x) => await AddAction(x), AddCommandValidation);
            }
        };
    }

    private bool AddCommandValidation(object arg)
    {
       if(Model==null)
            return false;

       if(Model.PaymentStatus== PaymentStatus.Lunas)
            return false;

        return true;

    }

    private async Task SavePembayaran(PembayaranPenjualan value)
    {
        try
        {
            await Task.Delay(1500);
            if(IsBusy) return;
            IsBusy = true;
            var result = await penjualanService.CreatePembayaran(Model.Id, value, true);
            if(result != null)
            {
                Items.Add(result);
                var sisa = Model.Total- Items.Sum(x => x.PayValue);
                if(sisa<=0)
                    Model.PaymentStatus = PaymentStatus.Lunas;
                else
                    Model.PaymentStatus = PaymentStatus.Panjar;

                await MessageShow.InfoAsync("Berhasil !");
            }
        }
        catch (Exception ex)
        {
           await MessageShow.ErrorAsync(ex.Message);
        }
        finally { IsBusy = false; }
    }

    private async Task AddAction(object x)
    {
        try
        {
            if (!await Account.UserInRole("Accounting"))
                throw new SystemException("Maaf Anda Tidak Memiliki Akses !");

            var sisa = Items.Sum(x => x.PayValue);
            var dataParemeter = new Dictionary<string, object>() {
            {"Sisa", Model.Total-sisa}
        };
            await Shell.Current.GoToAsync($"//{nameof(PenjualanView)}/{nameof(PenjualanPembayaranView)}/{nameof(PembayaranDialogView)}", true, dataParemeter);
        }
        catch (Exception ex)
        {
            await MessageShow.ErrorAsync(ex.Message);
        }
    }

    private async Task LoadItemAction(object x)
    {
        try
        {
            if (x == null)
                return;

            IsBusy = true;
            var parameterX = x as PenjualanAndOrderModel;
            var data = await penjualanService.GetPembayaran(parameterX.PenjualanId);
            if (data != null)
            {
                Items.Clear();
                foreach (var item in data)
                {
                    Items.Add(item);
                }
            }
        }
        catch (Exception ex)
        {
            await MessageShow.ErrorAsync(ex.Message);
        }
        finally { IsBusy = false; }
    }


    private PenjualanAndOrderModel model;

    public PenjualanAndOrderModel Model
    {
        get { return model; }
        set { SetProperty(ref model, value);
           
        }
    }


    public Command LoadItemCommand { get; }
    private Command addCommand;

    public Command AddCommand
    {
        get { return addCommand; }
        set { SetProperty(ref addCommand , value); }
    }


    public Task SetPenjualan(PenjualanAndOrderModel value)
    {
        this.Model = value;
        LoadItemCommand.Execute(value);
        return Task.CompletedTask;
    }


}