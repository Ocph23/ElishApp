using Dapper;
using Microsoft.Extensions.Configuration;
using ShareModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RLDC
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private readonly OcphDbContext _db;

        public MainWindow( OcphDbContext db)
        {
            InitializeComponent();
            _db = db;
            Loaded += MainWindow_Loaded;


           

        }


        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var results = from a in _db.Supplier
                          join b in _db.SupplierProduct on a.Id equals b.SupplierId
                          join c in _db.Product on b.ProductId equals c.Id
                          select a;

            foreach (var item in results)
            {
                Debug.WriteLine(item.Nama);
            }

        }
    }
}
