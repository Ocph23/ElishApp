using ApsMobileApp.Models.Messages;
using ApsMobileApp.ViewModels;
using CommunityToolkit.Mvvm.Messaging;
using ShareModels;
using ShareModels.ModelViews;
using System.Collections.ObjectModel;

namespace ApsMobileApp.Views.PembelianViews;

[QueryProperty("Pembelian", "Pembelian")]
public partial class PembelianPembayaranView : ContentPage
{

    private PembelianDataModel model;

    public PembelianDataModel Pembelian
    {
        get { return model; }
        set
        {
            model = value;
            var vm = BindingContext as PembelianPembayaranViewModel;
            vm.SetPembelian(value);
            OnParentChanged();
        }

    }


    public PembelianPembayaranView(PembelianPembayaranViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}


public class PembelianPembayaranViewModel : BaseViewModel
{
    private readonly IPembelianService pembelianService;

    public ObservableCollection<PembayaranPembelian> Items { get; }

    public PembelianPembayaranViewModel(IPembelianService _pembelianService)
    {
        Items = new ObservableCollection<PembayaranPembelian>();
        LoadItemCommand = new Command(async (x) => await LoadItemAction(x));
        AddCommand = new Command(async (x) => await AddAction(x), AddCommandValidation);
        pembelianService = _pembelianService;
        WeakReferenceMessenger.Default.Register<Models.Messages.PembayaranPembelianChangeMessage>(this, (r, m) =>
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
        if (Model == null)
            return false;

        if (Model.Status == PaymentStatus.Lunas)
            return false;

        return true;

    }

    private async Task SavePembayaran(PembayaranPembelian value)
    {
        try
        {
            await Task.Delay(1500);
            if (IsBusy) return;
            IsBusy = true;
            var result = await pembelianService.CreatePembayaran(Model.Id, value);
            if (result != null)
            {
                Items.Add(result);
                var sisa = Model.Total - Items.Sum(x => x.PayValue);
                if (sisa <= 0)
                    Model.Status = PaymentStatus.Lunas;
                else
                    Model.Status = PaymentStatus.Panjar;

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
            await Shell.Current.GoToAsync($"//{nameof(PembelianView)}/{nameof(PembelianPembayaranView)}/{nameof(PembelianPembayaranDialogView)}", true, dataParemeter);
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
            var parameterX = x as PembelianDataModel;
            var data = await pembelianService.GetPembayaran(parameterX.Id);
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


    private PembelianDataModel model;

    public PembelianDataModel Model
    {
        get { return model; }
        set
        {
            SetProperty(ref model, value);

        }
    }


    public Command LoadItemCommand { get; }
    private Command addCommand;

    public Command AddCommand
    {
        get { return addCommand; }
        set { SetProperty(ref addCommand, value); }
    }


    public Task SetPembelian(PembelianDataModel value)
    {
        this.Model = value;
        LoadItemCommand.Execute(value);
        return Task.CompletedTask;
    }


}