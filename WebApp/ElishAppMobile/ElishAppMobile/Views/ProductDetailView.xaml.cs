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
        public Command AddPictureCommand { get; }

        public ProductDetailViewModel(ProductStock model)
        {
            Model = model;

            AddPictureCommand = new Command(AddPictureAction, CanAddPicture);
        }

        private void AddPictureAction(object obj)
        {
           
        }

        private bool CanAddPicture(object arg)
        {
            if (Account.UserInRole("Administrator").Result)
                return true;
            return false;
        }

        private bool canAddImage;

        public bool CanAddImage
        {
            get { return canAddImage; }
            set { SetProperty(ref canAddImage , value); }
        }

    }
}