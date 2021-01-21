using ElishAppMobile.Helpers;
using ElishAppMobile.Models;
using ElishAppMobile.ViewModels;
using Plugin.Media;
using ShareModels;
using ShareModels.ModelViews;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
        public Command ShowImageCommand { get; }
        public Command RemoveImageCommand { get; }
        public ObservableCollection<ProductImageModel> Pictures { get; set; }

        public ProductDetailViewModel(ProductStock model)
        {
            Model = model;
            AddPictureCommand = new Command(CameraAction, CanAddPicture);
            ShowImageCommand = new Command(ShowImageAction);
            RemoveImageCommand = new Command(RemoveImageAction, CanAddPicture);
            Pictures = new ObservableCollection<ProductImageModel>();
            if(model.ProductImage!=null)
                foreach (var item in model.ProductImage)
                {
                    Pictures.Add(new ProductImageModel(item));
                }
        }

        private async void RemoveImageAction(object obj)
        {
            try
            {
                var model = (ProductImageModel)obj;
                var deleted= await Products.RemovePhoto(model.Id);
                if (deleted)
                {
                    Pictures.Remove(model);
                    var item = Model.ProductImage.SingleOrDefault(x => x.Id == model.Id);
                    if(item!=null)
                        Model.ProductImage.Remove(item);
                   await Toas.ShowLong("Delete Success !");
                }
            }
            catch (Exception ex)
            {
               await MessageHelper.ErrorAsync(ex.Message);
            }

        }

        private void ShowImageAction(object obj)
        {
            var model = (ProductImageModel)obj;
            if(model!=null)
            {
                var form = new ShowImagePage(model);
                Shell.Current.Navigation.PushAsync(form);
            }
        }

        private async void CameraAction(object obj)
        {
            try
            {
                IsBusy = true;

                if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                {
                    await MessageHelper.ErrorAsync(":( Permission not granted to photos.");
                    return;
                }

                var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
                {
                    PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium,
                    Directory = "Pictures",
                    Name = "image.jpg"
                });

                if (file == null)
                    return;
                var image = new ProductImage
                {
                    ProductId = Model.Id
                };
                var stream = file.GetStream();
                using (MemoryStream ms = new MemoryStream())
                {
                    stream.CopyTo(ms);
                    image.Buffer = ms.ToArray();
                }

                file.Dispose();

                image = await Products.AddPhoto(image);
                if (image != null)
                {
                    Model.ProductImage.Add(image);
                    Pictures.Add(new ProductImageModel(image));
                    await Toas.ShowLong("Success !");
                }
            }
            catch (Exception ex)
            {
               await MessageHelper.ErrorAsync(ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
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