using ApsMobileApp.Models.Messages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using ShareModels;
using System.ComponentModel.DataAnnotations;

namespace ApsMobileApp.Views.PembelianViews;


[QueryProperty("Sisa","Sisa")]
public partial class PembelianPembayaranDialogView : ContentPage
{

	private double sisa;

	public double Sisa
	{
		get { return sisa; }
		set { sisa = value;
			var vm = BindingContext as PembelianPembayaranDialogViewModel;
			vm.SetSisa(value);
			OnPropertyChanged();
		}
	}


	public PembelianPembayaranDialogView(PembelianPembayaranDialogViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}

public partial class PembelianPembayaranDialogViewModel:ObservableObject
{


    [ObservableProperty] private int id;
    [ObservableProperty] private DateTime payDate =DateTime.Now;
    [ObservableProperty] string payTo;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ShowBank))]
    private PayType payType;
    
    [ObservableProperty] private string bankName;
    [ObservableProperty] private string rekNumber;
    [ObservableProperty] private string description;
    [ObservableProperty] private int status;
    [ObservableProperty] private double payValue;


    public PembelianPembayaranDialogViewModel()
    {

		BayarCommand = new Command(async (x) => await BayarCommandAction(), BayarCommandValidation);
		CancelCommand = new Command(() => { Shell.Current.Navigation.PopModalAsync(); });
        PaymentTypes = Enum.GetValues<PayType>().Cast<PayType>().ToList();

        this.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName != "BayarCommand")
                BayarCommand = new Command(async (x) => await BayarCommandAction(), BayarCommandValidation);
        };

    }

    private bool BayarCommandValidation(object arg)
    {
        if(Sisa-PayValue <0 ) return false;
        if ((PayType != PayType.Tunai))
        {
            if (string.IsNullOrEmpty(RekNumber) || string.IsNullOrEmpty(PayTo) || string.IsNullOrEmpty(BankName))
            {
                return false;
            }
        }

        if (string.IsNullOrEmpty(PayTo) || string.IsNullOrEmpty(Description))
            return false;

        return true;
    }

    private Command bayarCommand;

    public Command BayarCommand
    {
        get { return bayarCommand; }
        set { SetProperty(ref bayarCommand , value); }
    }

    public Command CancelCommand { get; }
    public List<PayType> PaymentTypes { get; }

    public bool ShowBank => PayType!= PayType.Tunai ? true:false;

    public double Sisa { get; private set; }

    internal void SetSisa(double value)
    {
        Sisa =value;
        PayValue = value;
    }

    private async Task BayarCommandAction()
    {
		try
		{
            if ((PayType != PayType.Tunai))
            {
                if (string.IsNullOrEmpty(RekNumber) || string.IsNullOrEmpty(PayTo) || string.IsNullOrEmpty(BankName))
                {
                    throw new SystemException("Lengkapi Data Bank Pembayaran !");
                }
            }

            if (string.IsNullOrEmpty(PayTo))
                throw new SystemException("Account Name/Penerima Pembayaran tidak Boleh Kosong !");


            WeakReferenceMessenger.Default.Send(new PembayaranPembelianChangeMessage(new PembayaranPembelian {
             BankName=this.BankName, Description=Description, Id=Id, PayDate=PayDate, PayTo=PayTo, PayType=PayType, 
              PayValue=PayValue,  RekNumber = RekNumber, Status = Status 
            }));
              await Shell.Current.Navigation.PopModalAsync();
        }
		catch (Exception ex)
		{
		    await	MessageShow.ErrorAsync(ex.Message);

		}
    }
}