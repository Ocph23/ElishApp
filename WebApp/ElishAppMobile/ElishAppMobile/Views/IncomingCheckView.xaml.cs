using ElishAppMobile.ViewModels;
using ShareModels.ModelViews;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Linq;
using ShareModels;
using ElishAppMobile.Helpers;

namespace ElishAppMobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class IncomingCheckView : ContentPage
    {
        private readonly IncomingCheckViewModel vm;

        public IncomingCheckView()
        {
            InitializeComponent();
            BindingContext = vm= new IncomingCheckViewModel();
            search.OnSearchFound += Search_OnSearchFound;
        }

        private void Search_OnSearchFound(object data)
        {
            vm.Search(data.ToString());
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            picker.Focus();
        }
    }

    public class IncomingCheckViewModel : BaseViewModel
    {
        private PembelianModel _source;

        public ObservableCollection<Pembelian> Pembelians { get;  }
        public ObservableCollection<IncomingItem> Items { get; }

        public Command LoadItemsCommand { get; }
        public Command AddItemCommand { get; }
        public Command ScanCommand { get; }
        public Command<IncomingItem> ItemTapped { get; }

        public IncomingCheckViewModel()
        {
            Title = "IncomingItems";
            Items = new ObservableCollection<IncomingItem>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
            ItemTapped = new Command<IncomingItem>(OnItemSelected);
            AddItemCommand = new Command(OnAddItem);
            ScanCommand = new Command(ScanAction);

            if (Signalr.Connection.State != HubConnectionState.Connected)
                Task.Run(() => Signalr.Connect());

            Signalr.Connection.On<IncomingItem>("RecieveIncomingItem", model => {
                var item = Source.Datas.Where(x=>x.ProductId== model.ProductId && x.PembelianId==model.PembelianId).FirstOrDefault();
                if (item != null && item.ActualValue != model.ActualValue)
                    item.ActualValue = model.ActualValue;
            });

            Pembelians = new ObservableCollection<Pembelian>();
            LoadItemsCommand.Execute(null);
        }

        public async void CreateAction(Pembelian obj)
        {
            await Task.Delay(1);
            Items.Clear();
            Source = await IncomingService.CreateNew(obj.Id);
            Model = Source.Model;
            foreach (var item in Source.Datas)
            {
                item.UpdateEvent += Item_UpdateEvent;
                Items.Add(item);
            }
        }

        private async void ScanAction(object obj)
        {
            try
            {
                var barcodeScan = new InputBarcodeView();
                var vmScanBarcode = new InputBarcodeViewModel();
                barcodeScan.BindingContext = vmScanBarcode;
                vmScanBarcode.OnResultScanHandler += async (dynamic result) => {
                    if (result.Type == "Auto")
                    {
                        string article = result.Article.ToString();
                        var data = Items.Where(x => x.Product.CodeArticle == article).FirstOrDefault();
                        if (data != null)
                        {
                            data.ActualValue++;
                          await Toas.ShowLong($"{data.Product.Name} , Amount : {data.ActualValue}");
                        }
                        else
                            await Toas.ShowLong($"Error : {result.Article.ToString()} Not Found !");
                    }
                    else
                    {
                        string article = (string)result.Article;
                        var data = Items.Where(x => x.Product.CodeArticle == article).FirstOrDefault();
                        if (data != null)
                        {
                            data.ActualValue+= (double)result.Count;
                            await Toas.ShowLong($"{data.Product.Name} , Amount : {data.ActualValue}");
                        }
                        else
                            await Toas.ShowLong($"Error : {result.Article} Not Found !");
                    }
                };
                await Shell.Current.Navigation.PushModalAsync(barcodeScan);
            }
            catch (Exception ex)
            {
                await Toas.ShowLong($"Error : {ex.Message}");
            }
        }

        async Task ExecuteLoadItemsCommand()
        {
            try
            {
                Pembelians.Clear();
                   var pembelians = await PembelianService.GetPembelians();
                foreach (var item in pembelians)
                {
                    Pembelians.Add(item);
                }
                await Task.Delay(500);
                Items.Clear();
                Source = await IncomingService.Load();
                if (Source.Model != null)
                {

                    Model = Source.Model;
                    foreach (var item in Source.Datas)
                    {
                        item.UpdateEvent += Item_UpdateEvent;
                        Items.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task Item_UpdateEvent(IncomingItem arg)
        {
           await Signalr.UpdateIncomingItem(arg);
        }

        public void OnAppearing()
        {
            IsBusy = true;
            SelectedItem = null;
        }

        public IncomingItem SelectedItem
        {
            get => _selectedItem;
            set
            {
                SetProperty(ref _selectedItem, value);
                OnItemSelected(value);
            }
        }

        public PembelianModel Source { 
            get=>_source;  
            set =>SetProperty(ref _source,value);
        }
     

        private async void OnAddItem(object obj)
        {
            await Shell.Current.GoToAsync(nameof(NewItemPage));
        }

        async void OnItemSelected(IncomingItem item)
        {
            if (item == null)
                return;
            await Shell.Current.GoToAsync($"{nameof(ItemDetailPage)}?{nameof(ItemDetailViewModel)}");
        }

        internal Task Search(string textSearch)
        {
            if (Source != null &&Source.Datas!=null)
            {
                Items.Clear();
                if (string.IsNullOrEmpty(textSearch))
                {
                    foreach (var item in Source.Datas)
                    {
                        Items.Add(item);
                    }
                }
                else
                {
                    foreach (var item in Source.Datas.Where(x => x.Product.Name.ToLower() == textSearch.ToLower()))
                    {
                        Items.Add(item);
                    }
                }
            }
           
            return Task.CompletedTask;
        }


        private Pembelian pickerSelected;
        private IncomingItem _selectedItem;

        public Pembelian Model
        {
            get { return pickerSelected; }
            set { SetProperty(ref pickerSelected, value);
                if (value != null && value != this.Source.Model)
                {
                    CreateAction(value);
                }
            }
        }

    }
}