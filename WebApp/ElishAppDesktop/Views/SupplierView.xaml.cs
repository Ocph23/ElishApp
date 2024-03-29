﻿using ElishAppDesktop.ViewModels;
using ShareModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace ElishAppDesktop.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SupplierView : Page
    {
        public SupplierView()
        {
            this.InitializeComponent();
            DataContext = new SupplierViewModel();
        }
    }



    public class SupplierViewModel : BaseViewModel
    {

        public ObservableCollection<Product> ItemSource { get; set; }
        public SupplierViewModel()
        {
            ItemSource = new ObservableCollection<Product>();
            Load();
        }

        private async void Load()
        {
            var data = await Products.Get();
            ItemSource.Clear();
            foreach (var item in data)
            {
                ItemSource.Add(item);
            }


        }
    }
}
