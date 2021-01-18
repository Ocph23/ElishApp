﻿using Microsoft.AspNetCore.SignalR;
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
        private readonly IServiceProvider _serviceProvider;
        private readonly ApplicationDbContext dbContext;
        private readonly IPembelianService pembelianService;
        private Pembelian pembelianSelected;

        public IncommingService(IHubContext<ElishAppHub> hubContext, IServiceProvider provider)
        {
            _hub = hubContext;
            _serviceProvider = provider;

            using var scope = provider.CreateScope();
            Pembelians = new ObservableCollection<Pembelian>();
            dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            pembelianService = scope.ServiceProvider.GetRequiredService<IPembelianService>();
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
                using var scope = _serviceProvider.CreateScope();
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
                         join b in dbContext.IncomingItem.Where(x => x.PembelianId == value.Id)  
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
               _= CreateNew(value.Id).Result;
            }
        }

        //  public Pembelian Model { get; private set; }
        public ObservableCollection<IncomingItem> Datas { get; private set; } = new ObservableCollection<IncomingItem>();

        public async Task<PembelianModel> CreateNew(int pembelianid)
        {

            var pembelian = await pembelianService.GetPembelian(pembelianid);

            var datas = pembelian.Items.Select(x => x);
            foreach (var item in datas)
            {
                var data = new IncomingItem(item);
                data.UpdateEvent += Data_UpdateEvent;
                Datas.Add(data);
            }

            return new PembelianModel { Model = PembelianSelected, Datas = Datas.ToList() };
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

        public async Task Save()
        {

            var trans = dbContext.Database.BeginTransaction();
            try
            {
                if (Datas.Count > 0)
                {
                    foreach (var item in Datas)
                    {
                        if (item.Id <= 0)
                        {
                            dbContext.IncomingItem.Add(item);
                            if (item.Id <= 0)
                                throw new SystemException("Not Saved !");
                        }
                        else
                        {
                            var oldItem = dbContext.IncomingItem.SingleOrDefault(x => x.Id == item.Id);
                            if (oldItem != null)
                            {
                                dbContext.Entry(oldItem).CurrentValues.SetValues(item);
                            }
                        }
                    }
                }

                await dbContext.SaveChangesAsync();
                trans.Commit();
            }
            catch (Exception)
            {
                trans.Rollback();
                throw new SystemException("Not Saved !");
            }
        }
    }
}
