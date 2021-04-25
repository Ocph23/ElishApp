using ElishAppMobile.ViewModels;
using ShareModels;
using System;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ElishAppMobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
            BindingContext = new LoginViewModel();
            cr.Text = $"Copyright @Ocph23 2020 - {DateTime.Now.Year}";
            version.Text = $"Version : {VersionTracking.CurrentVersion} - Build:{VersionTracking.CurrentBuild}";
        }
    }
    public class LoginViewModel:BaseViewModel
    {
        #region Constructor
        public LoginViewModel()
        {
            LoginCommand = new Command(LoginAction, CanLogin);
            ShowPasswordCommand = new Command(ShowPasswordAction);
            this.PropertyChanged +=
             (_, __) => LoginCommand.ChangeCanExecute();
            EyeIcon = ImageSource.FromFile("openeye.png");
            ShowPassword = true;
        }

        private void ShowPasswordAction(object obj)
        {
            ShowPassword = !ShowPassword;
            if (!ShowPassword)
                EyeIcon = ImageSource.FromFile("openeye.png");
            else
                EyeIcon = ImageSource.FromFile("closeeye.png");

        }
        #endregion

        #region Fields
        private string url;
        private string userName;
        private string _password;
        private Command _loginCommand;
        #endregion

        #region Properties


        private ImageSource eyeIcon;

        public ImageSource EyeIcon
        {
            get { return eyeIcon; }
            set { SetProperty(ref eyeIcon , value); }
        }

        public string Url
        {
            get {
                if (string.IsNullOrEmpty(url))
                    url = Helper.Url;
                return url; 
            }
            set { SetProperty(ref url , value);
              
            }
        }


        private bool showPassword;

        public bool ShowPassword
        {
            get { return showPassword; }
            set {SetProperty(ref showPassword , value); }
        }


        public string UserName
        {
            get { return userName; }
            set { SetProperty(ref userName , value);
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                SetProperty(ref _password, value); 
            }

        }

        public Command LoginCommand {
            get => _loginCommand;
            set => SetProperty(ref _loginCommand, value);
        }
        public Command ShowPasswordCommand { get; }
        #endregion

        #region Methods
        private bool CanLogin(object arg)
        {
            if (IsBusy || string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(Password))
                return false;
            return true;
        }

        private async void LoginAction(object obj)
        {
            try
            {
                if (!Helper.CheckInterNetConnection().Item1)
                {
                    return;
                }

                if (IsBusy)
                    return;

                IsBusy = true;
                var user = new UserLogin() { UserName=UserName, Password=Password };
                var result=  await UserService.Login(user);
                if (Account.UserIsLogin)
                {
                    if (await Account.UserInRole("Administrator"))
                    {
                       Application.Current.MainPage = new AppShell();
                    }
                    else if (await Account.UserInRole("Sales"))
                    {
                        Application.Current.MainPage = new SalesShell();
                    }
                    else
                    {
                        Application.Current.MainPage = new CustomerShell();
                    }
                }
                else
                {
                    throw new SystemException("You Not Have Access !");
                }
            }
            catch (Exception ex)
            {
               await MessageHelper.ErrorAsync(ex.Message);
            }
            finally
            {
                Helper.Url = Url;
                IsBusy = false;
            }
        }
        #endregion
    }
}