using ElishAppMobile.ViewModels;
using ShareModels;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ElishAppMobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CreateCustomer : ContentPage
    {
        public CreateCustomer()
        {
            InitializeComponent();
            BindingContext = new CreateCustomerViewModel();
        }
    }

    public class CreateCustomerViewModel : BaseViewModel
    {
        private Command _SaveCommand;

        public CreateCustomerViewModel()
        {
            Model = new Customer();
            SaveCommand = new Command(SaveAction, SaveValidate);
            this.Model.PropertyChanged += Model_PropertyChanged;
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
        #endregion

        #region Methods
        private async void SaveAction(object obj)
        {
            try
            {
                var result = await Customers.Post(Model);
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
}