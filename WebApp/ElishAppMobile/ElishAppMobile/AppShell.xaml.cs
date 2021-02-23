﻿using ElishAppMobile.Helpers;
using ElishAppMobile.Views;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ElishAppMobile
{
    public partial class AppShell : Shell
    {
        public AppShell()
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
           await Account.SetProfile(null);
            await  Account.SetUser(null);
            await Current.GoToAsync("//LoginPage");
        }

        private int countPress;
        protected override bool OnBackButtonPressed()
        {
            countPress++;
            if (countPress >= 2)
            {
                return base.OnBackButtonPressed();
            }

            Toas.ShowLong("Tekan Sekali Lagi Untuk Keluar !");
            Task.Run(() => ClearCount());
            return true;
        }

        private Task ClearCount()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                await Task.Delay(2000);
                countPress = 0;
            });

            return Task.CompletedTask;
        }
    }
}
