using ElishAppMobile.ViewModels;
using System;
using System.ComponentModel;
using System.Diagnostics;
using Xamarin.Forms;

namespace ElishAppMobile.Views
{
    public partial class ItemDetailPage :  ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }

        public void search_OnSearchFound(string text)
        {
            Debug.WriteLine(text);
        }
    }
}