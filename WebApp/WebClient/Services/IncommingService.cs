using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Ocph.DAL;
using ShareModels;
using ShareModels.ModelViews;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace WebClient.Services
{
    public class IncommingService : ShareModels.BaseNotify, IIncommingService
    {
        private readonly IHubContext<ElishAppHub> _hub;
        public IncommingService(IHubContext<ElishAppHub> hubContext)
        {
            _hub = hubContext;

            using var scope = ServiceLocator.Instance.CreateScope();
            Pembelians = new ObservableCollection<Pembelian>();
            dbContext = scope.ServiceProvider.GetRequiredService<OcphDbContext>();
            var pembelianService = scope.ServiceProvider.GetRequiredService<IPembelianService>();
            Task.Run(async () =>
            {
                var pembelians = await pembelianService.GetPembelians();
                foreach (var item in pembelians)
                {
                    Pembelians.Add(item);
                }
            });
        }

        public ObservableCollection<Pembelian> Pembelians { get; set; }

        private readonly OcphDbContext dbContext;
        private Pembelian pembelianSelected;
        public Pembelian PembelianSelected
        {
            get => pembelianSelected;
            set  {
                if (value != null && value != pembelianSelected)
                    SetDataSource(value);
                else
                    ClearDataSource();
                SetProperty(ref pembelianSelected, value);
            }
        }

        private void ClearDataSource()
        {
            this.Datas.Clear();
            Task.Run(async () =>
            {
                using var scope = ServiceLocator.Instance.CreateScope();
                var pembelianService = scope.ServiceProvider.GetRequiredService<IPembelianService>();
                var pembelians = await pembelianService.GetPembelians();
                Pembelians.Clear();
                foreach (var item in pembelians)
                {
                    Pembelians.Add(item);
                }
            });
        }

        private void SetDataSource(Pembelian value)
        {
            var result = (from a in value.Items
                         join b in dbContext.IncomingItems.Where(x => x.PembelianId == value.Id)  
                         on new { a.PembelianId, a.ProductId } equals new { b.PembelianId, b.ProductId }
                         into cc
                         from b in cc.DefaultIfEmpty()
                         select new IncomingItem
                         {   PembelianId=a.PembelianId,
                             ActualValue = b==null?0: b.ActualValue, Amount= a.Amount, Product=a.Product,  
                             ProductId=a.ProductId, Id= b==null?0:b.Id, UnitId=a.UnitId, Unit = b==null?a.Unit: a.Product.Units.Where(x=>x.Id== b.UnitId).FirstOrDefault()
                         }).ToList();
                        
            if (result.Any())
            {
                Datas.Clear();
                foreach (var item in result)
                {
                    Datas.Add(item);
                }
            }
            else
            {
                CreateNew(value.Id);
            }
        }

        //  public Pembelian Model { get; private set; }
        public ObservableCollection<IncomingItem> Datas { get; private set; } = new ObservableCollection<IncomingItem>();

        public Task<PembelianModel> CreateNew(int pembelianid)
        {
            var results = from p in dbContext.Pembelians.Where(x => x.Id == pembelianid)
                          join o in dbContext.OrderPembelians.Select() on p.OrderPembelianId equals o.Id
                          join s in dbContext.Suppliers.Select() on o.SupplierId equals s.Id

                          select new Pembelian
                          {
                              CreatedDate = p.CreatedDate,
                              Discount = p.Discount,
                              Id = p.Id,
                              InvoiceNumber = p.InvoiceNumber,
                              OrderPembelian = o,
                              PayDeadLine = p.PayDeadLine,
                              OrderPembelianId = p.OrderPembelianId,
                              Status = p.Status,
                              Supplier = s
                          };

            var datas = from i in dbContext.PembelianItems.Where(x => x.PembelianId == PembelianSelected.Id)
                        join p in dbContext.Products.Select() on i.ProductId equals p.Id
                        join u in dbContext.Units.Select() on i.UnitId equals u.Id
                        select new PembelianItem
                        {              
                            Amount = i.Amount,  
                            Id = i.Id,
                            PembelianId = i.PembelianId,
                            Price = i.Price,
                            Product = p,
                            ProductId = i.ProductId,
                            Unit = u,
                            UnitId = i.UnitId
                        };
            foreach (var item in datas)
            {
                var data = new IncomingItem(item);
                data.UpdateEvent += Data_UpdateEvent;
                Datas.Add(data);
            }

            return  Task.FromResult(new PembelianModel { Model = PembelianSelected, Datas = Datas.ToList() });
        }

        private Task Data_UpdateEvent(IncomingItem arg)
        {
           return Invoike(arg);
        }

        public Task Invoike(IncomingItem data)
        {
            return _hub.Clients.All.SendAsync("RecieveIncomingItem", data);
        }

        public Task<PembelianModel> Load(bool force = false)
        {
            return Task.FromResult( new PembelianModel {Model=PembelianSelected, Datas=Datas.ToList()});
        }

        public Task Save()
        {

            var trans = dbContext.BeginTransaction();
            try
            {
                if (Datas.Count > 0)
                {
                    foreach (var item in Datas)
                    {
                        if (item.Id <= 0)
                        {
                            item.Id = dbContext.IncomingItems.InsertAndGetLastID(item);
                            if (item.Id <= 0)
                                throw new SystemException("Not Saved !");
                        }
                        else
                           if (!dbContext.IncomingItems.Update(x => new { x.ActualValue }, item, x => x.Id == item.Id && x.Id == item.Id))
                            {
                                throw new SystemException("Not Saved !");
                            }
                    }
                }


                trans.Commit();
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                trans.Rollback();
                throw new SystemException("Not Saved !");
            }
        }
    }
}
