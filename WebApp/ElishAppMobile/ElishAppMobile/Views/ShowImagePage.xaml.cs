using ElishAppMobile.Helpers;
using ElishAppMobile.Models;
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
    public partial class ShowImagePage : ContentPage
    {
        public ShowImagePage(ProductImageModel model)
        {
          InitializeComponent();
          pinchImage.Source= model.PhotoView;
        }

        public ImageSource Photo { get; }
    }
}