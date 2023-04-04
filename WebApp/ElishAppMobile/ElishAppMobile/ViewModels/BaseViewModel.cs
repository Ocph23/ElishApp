﻿using ElishAppMobile.Services;
using ShareModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace ElishAppMobile.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public SignalrClient Signalr=> DependencyService.Get<SignalrClient>();
        public ICategoryService Categories => DependencyService.Get<ICategoryService>();
       // public ICustomerService Customers => DependencyService.Get<ICustomerService>();
        public ISupplierService Suppliers => DependencyService.Get<ISupplierService>();
        public IProductService Products=> DependencyService.Get<IProductService>();
        public IIncommingService IncomingService => DependencyService.Get<IIncommingService>();
        public IPembelianService PembelianService=> DependencyService.Get<IPembelianService>();
        public IPenjualanService PenjualanService=> DependencyService.Get<IPenjualanService>();
        public IUserStateService UserService  => DependencyService.Get<IUserStateService>();
        public ElishDbStore ElishDbStore => DependencyService.Get<ElishDbStore>();

        bool isBusy = false;
        public bool IsBusy
        {
            get { return isBusy; }
            set { SetProperty(ref isBusy, value); }
        }

        string title = string.Empty;
        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        protected bool SetProperty<T>(ref T backingStore, T value,
            [CallerMemberName] string propertyName = "",
            Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
