using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace ElishAppDesktop.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        public ICommand LoginCommand { get; }

        public LoginViewModel()
        {
            LoginCommand = new CommandHandler(OnLoginClicked);
            this.PropertyChanged +=
              (_, __) => LoginCommand.CanExecute(null);
        }

        private async void OnLoginClicked(object obj)
        {
            // Prefixing with `//` switches to a different navigation stack instead of pushing to the active one
           // await Shell.Current.GoToAsync($"//{nameof(AboutPage)}");
        }
    }
}
