using ApsMobileApp.Views;
using System;
using System.Collections.Generic;
using System.Text;


namespace ApsMobileApp.ViewModels;

public class LoginViewModel : BaseViewModel
{
    public Command LoginCommand { get; }

    public LoginViewModel()
    {
        LoginCommand = new Command(OnLoginClicked);
        this.PropertyChanged +=
          (_, __) => LoginCommand.ChangeCanExecute();
    }

    private async void OnLoginClicked(object obj)
    {
        // Prefixing with `//` switches to a different navigation stack instead of pushing to the active one
        await Shell.Current.GoToAsync($"//{nameof(AboutPage)}");
    }
}
