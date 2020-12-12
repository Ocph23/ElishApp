using ElishAppMobile.ViewModels;
using ShareModels;
using ShareModels.ModelViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ElishAppMobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProductDetailView : ContentPage
    {
        public ProductDetailView()
        {
            InitializeComponent();
        }
    }
    public class ProductDetailViewModel : BaseViewModel
    {
        public ProductStock Model { get; set; }
        public ProductDetailViewModel(ProductStock model)
        {
            Model = model;
        }
    }
}