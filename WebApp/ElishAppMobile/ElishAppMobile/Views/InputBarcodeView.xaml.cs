using ElishAppMobile.Helpers;
using ElishAppMobile.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZXing;

namespace ElishAppMobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InputBarcodeView : ContentPage
    {
        public InputBarcodeView()
        {
            InitializeComponent();
            BindingContext = new InputBarcodeViewModel();
        }
    }



    public class InputBarcodeViewModel : BaseViewModel
    {
        public event OnResultBarcode OnResultScanHandler;
        private bool showFlash;
        public bool ShowFlash
        {
            get => showFlash;
            set => SetProperty(ref showFlash, value);
        }
        private bool autoCount;
        public bool AutoCount
        {
            get => autoCount;
            set => SetProperty(ref autoCount, value);
        }

        private double amountValue=1;

        public double AmountValue
        {
            get => amountValue; 
            set => SetProperty(ref amountValue , value); 
        }


        public InputBarcodeViewModel()
        {
            ScanningCommand = new Command(ScanningAction, x => IsScanning);
            ScanAgainCommand = new Command(() => { IsScanning = true; ScanAgain = false; TextResult = string.Empty; });
            TakeCommand = new Command(() =>
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    dynamic data = new System.Dynamic.ExpandoObject();
                    data.Article = TextResult;
                    data.Count = AmountValue;
                    data.Type = "Manual";
                    OnResultScanHandler?.Invoke(data);
                    await Task.Delay(1000);
                    IsScanning = true;
                });
            });
            CancelCommand = new Command(() =>
            {
                TextResult = string.Empty;
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await Task.Delay(1000);
                    await Application.Current.MainPage.Navigation.PopModalAsync();
                });
            });
            Task.Run(() => StartScan());
        }



        private string textResult;

        public string TextResult
        {
            get { return textResult; }
            set { SetProperty(ref textResult, value); }
        }


        private bool scanAgain;

        public bool ScanAgain
        {
            get { return scanAgain; }
            set { SetProperty(ref scanAgain, value); }
        }

        public async Task StartScan()
        {
            await Task.Delay(1000);
            IsScanning = true;
        }

        private void ScanningAction(object obj)
        {
            IsScanning = false;
            ScanAgain = true;
            Device.BeginInvokeOnMainThread(async () =>
            {
              
                try
                {
                    IsBusy = true;
                    var data = obj as Result;
                    TextResult = data.Text;
                    if (AutoCount)
                    {
                        dynamic result = new System.Dynamic.ExpandoObject();
                        result.Type = "Auto";
                        result.Article = TextResult;
                        OnResultScanHandler?.Invoke(result);
                        await Task.Delay(1000);
                        IsScanning = true;
                    }
                    Console.WriteLine(obj.ToString());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    IsBusy = false;
                 
                }
            });

        }


        public Command ScanningCommand
        {
            get
            {
                return _scanningCommand;
            }
            set
            {
                SetProperty(ref _scanningCommand, value);
            }
        }

        public Command ScanAgainCommand { get; private set; }
        public Command TakeCommand { get; }
        public Command CancelCommand { get; }

        private bool isScaning;

        public bool IsScanning
        {
            get { return isScaning; }
            set { SetProperty(ref isScaning, value); }
        }

        public bool IsAnalyzing
        {
            get { return isAnalyzing; }
            set { SetProperty(ref isAnalyzing, value); }
        }

        private bool showAutoCount=true;

        public bool ShowAutoCount
        {
            get=> showAutoCount;
            set => SetProperty(ref showAutoCount, value);
        }


        private Command _scanningCommand;
        private bool isAnalyzing;

    }
}