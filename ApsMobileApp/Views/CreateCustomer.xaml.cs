using ApsMobileApp.ViewModels;
using ShareModels;
using System;
using System.Threading.Tasks;



namespace ApsMobileApp.Views;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class CreateCustomer : ContentPage
{
    public CreateCustomer(CreateCustomerViewModel viewmodel)
    {
        InitializeComponent();
        BindingContext = viewmodel;
    }
}

public class CreateCustomerViewModel : BaseViewModel
{
    private Command _SaveCommand;

    public CreateCustomerViewModel(ICustomerService customerService)
    {
        Model = new Customer();
        SaveCommand = new Command(SaveAction, SaveValidate);
        this.Model.PropertyChanged += Model_PropertyChanged;
        CustomerService = customerService;
    }

    private void Model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        SaveCommand.ChangeCanExecute();
    }
    #region Fileds

    #endregion

    #region Properties
    public Customer Model { get; }
    public Command SaveCommand
    {
        get    => _SaveCommand;
        set    => SetProperty(ref _SaveCommand, value);
    }
    public ICustomerService CustomerService { get; }
    #endregion

    #region Methods
    private async void SaveAction(object obj)
    {
        try
        {
            var profile = await Account.GetProfile();
            Model.Karyawan.Id= profile.Id;
            var result = await CustomerService.Post(Model);
            if (result != null)
            {
                MessagingCenter.Send<MessageDataCenter>(new MessageDataCenter { Message = "Berhasil !", Title = "Success" }, "message");
                await Shell.Current.Navigation.PopModalAsync();
            }
        }
        catch (Exception ex)
        {
            MessagingCenter.Send<MessageDataCenter>(new MessageDataCenter { Message = ex.Message , Title="Error"}, "message");
        }
    }
    private bool SaveValidate(object obj)
    {
        if (Model == null)
            return false;
        if (string.IsNullOrEmpty(Model.Name) || string.IsNullOrEmpty(Model.ContactName) || string.IsNullOrEmpty(Model.Telepon) || string.IsNullOrEmpty(Model.Address))
            return false;
        return true;

    }

    #endregion
}