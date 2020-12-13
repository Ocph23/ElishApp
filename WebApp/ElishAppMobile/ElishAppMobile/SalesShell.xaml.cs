using ElishAppMobile.Views;
using System;
using Xamarin.Forms;

namespace ElishAppMobile
{
    public partial class SalesShell : Shell
    {
        public SalesShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(IncomingCheckView), typeof(IncomingCheckView));

            var result = Account.GetProfile().Result;
            if (result != null)
            {
                user.Text = result.Name;
            }


        }

        private async void OnMenuItemClicked(object sender, EventArgs e)
        {
            await Current.GoToAsync("//LoginPage");
        }
    }
}
