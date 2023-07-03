using ApsMobileApp.Helpers;
using ApsMobileApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;




namespace ApsMobileApp.Views;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class ShowImagePage : ContentPage
{
    public ShowImagePage(ProductImageModel model)
    {
      InitializeComponent();
      //pinchImage.Source= model.PhotoView;
    }

    public ImageSource Photo { get; }
}