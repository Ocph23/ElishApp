using ApsMobileApp.Helpers;
using ApsMobileApp.Models;
using ApsMobileApp.ViewModels;
using ShareModels;
using ShareModels.ModelViews;
using System.Collections.ObjectModel;




namespace ApsMobileApp.Views;

[QueryProperty(nameof(Product), "Product")]
public partial class ProductDetailView : ContentPage
{
    ProductStock product;
    public ProductStock Product
    {
        get { return product; }
        set
        {
            product = value;
            if (value != null)
            {
                var vm = BindingContext as ProductDetailViewModel;
                vm.SetModel(value);
            }
            OnPropertyChanged();
        }
    }

    public ProductDetailView(ProductDetailViewModel viemodel)
    {
        InitializeComponent();
        BindingContext = viemodel;
    }
}
public class ProductDetailViewModel : BaseViewModel
{
    ProductStock model;
    public ProductStock Model
    {
        get { return model; }
        set
        {
           SetProperty(ref model, value);
        }
    }


    public Command AddPictureCommand { get; }
    public Command ShowImageCommand { get; }
    public Command RemoveImageCommand { get; }
    public ObservableCollection<ProductImageModel> Pictures { get; set; }

    public ProductDetailViewModel(IProductService _productService)
    {
        productService = _productService;
        AddPictureCommand = new Command(CameraAction, CanAddPicture);
        ShowImageCommand = new Command(ShowImageAction);
        RemoveImageCommand = new Command(RemoveImageAction, CanAddPicture);
        Pictures = new ObservableCollection<ProductImageModel>();

    }


    public Task SetModel(ProductStock model)
    {
        Model = model;
        if (Model!=null && Model.ProductImage != null)
            foreach (var item in Model.ProductImage)
            {
                Pictures.Add(new ProductImageModel(item));
            }
        return Task.CompletedTask;
    }

    private async void RemoveImageAction(object obj)
    {
        try
        {
            var model = (ProductImageModel)obj;
            var deleted = await productService.RemovePhoto(model.Id);
            if (deleted)
            {
                Pictures.Remove(model);
                var item = Model.ProductImage.SingleOrDefault(x => x.Id == model.Id);
                if (item != null)
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
        if (model != null)
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

            if (MediaPicker.Default.IsCaptureSupported)
            {
                FileResult photo = await MediaPicker.Default.CapturePhotoAsync();

                if (photo != null)
                {
                    // save the file into local storage
                    string localFilePath = Path.Combine(FileSystem.CacheDirectory, photo.FileName);

                    using Stream sourceStream = await photo.OpenReadAsync();
                    //using FileStream localFileStream = File.OpenWrite(localFilePath);
                    //await sourceStream.CopyToAsync(localFileStream);

                    var image = new ProductImage
                    {
                        ProductId = Model.Id
                    };
                    using (MemoryStream ms = new MemoryStream())
                    {
                       await sourceStream.CopyToAsync(ms);
                      //  localFileStream.CopyTo(ms);
                        image.Buffer = ms.ToArray();
                    }


                    image = await productService.AddPhoto(image);
                    if (image != null)
                    {
                        Model.ProductImage.Add(image);
                        Pictures.Add(new ProductImageModel(image));
                        await Toas.ShowLong("Success !");
                    }


                }
            }
            else
            {
                await MessageHelper.ErrorAsync(":( Permission not granted to photos.");
                return;
            }



            //var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
            //{
            //    PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium,
            //    Directory = "Pictures",
            //    Name = "image.jpg"
            //});

            //if (file == null)
            //    return;
            //var image = new ProductImage
            //{
            //    ProductId = Model.Id
            //};
            //var stream = file.GetStream();
            //using (MemoryStream ms = new MemoryStream())
            //{
            //    stream.CopyTo(ms);
            //    image.Buffer = ms.ToArray();
            //}

            //file.Dispose();

            //image = await Products.AddPhoto(image);
            //if (image != null)
            //{
            //    Model.ProductImage.Add(image);
            //    Pictures.Add(new ProductImageModel(image));
            //    await Toas.ShowLong("Success !");
            //}
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
    private readonly IProductService productService;

    public bool CanAddImage
    {
        get { return canAddImage; }
        set { SetProperty(ref canAddImage, value); }
    }

}