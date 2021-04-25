using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ShareModels;
using ShareModels.ModelViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebClient.Services
{
    public class PenjualanService : IPenjualanService
    {
        private readonly IServiceProvider provider;
        private readonly ApplicationDbContext dbContext;
     //   private readonly IHttpContextAccessor auth;
        private readonly ILogger<PenjualanService> _logger;

        public PenjualanService(ApplicationDbContext db, IServiceProvider _provider,  ILogger<PenjualanService> log)
        {
            provider = _provider;
            dbContext = db;
       //     auth = httpContextAccessor;
            _logger = log;
        }


        #region penjualan
        public async Task<Penjualan> CreatePenjualan(int orderid)
        {
            var trans = dbContext.Database.BeginTransaction();
            try
            {
                var lastOrder = dbContext.Orderpenjualan.Where(x => x.Id == orderid)
                    .Include(x => x.Items)
                    .FirstOrDefault();

                if (lastOrder == null)
                    throw new SystemException("Order Tidak Ditemukan !");

                var penjualan =new Penjualan
                {
                    OrderPenjualanId = orderid,
                    Discount = lastOrder.Discount,
                    CreateDate = DateTime.Now,      
                    PayDeadLine =lastOrder.DeadLine,
                    Items = new List<Penjualanitem>()
                };
                foreach (var item in lastOrder.Items)
                {
                    var data = new Penjualanitem
                    {
                        Amount = item.Amount,
                        Price = item.Price,
                        ProductId = item.ProductId,
                        UnitId = item.UnitId,
                    };

                    penjualan.Items.Add(data);
                }

                dbContext.Penjualan.Add(penjualan);
                lastOrder.Status = OrderStatus.Diproses;
                dbContext.SaveChanges();
                trans.Commit();
                return penjualan;
            }
            catch (Exception ex)
            {

                 trans.Rollback();
                throw new SystemException(ex.Message);
            }
        }
        public Task<Penjualan> UpdatePenjualan(int penjualanId, Penjualan order)
        {
            var trans = dbContext.Database.BeginTransaction();
            try
            {
                var lastPenjualan = dbContext.Penjualan.Where(x => x.Id == penjualanId)  .Include(x=>x.Items)
                                 .FirstOrDefault();


                if (lastPenjualan == null)
                    throw new SystemException("Penjualan Not Found  !");

                order.Activity = order.Activity == ActivityStatus.None ? ActivityStatus.Created :order.Activity;
                order.OrderPenjualan.Status = OrderStatus.Diproses;
                dbContext.Entry(lastPenjualan).CurrentValues.SetValues(order);

                foreach (var item in order.Items)
                {
                    if (item.Id <= 0)
                    {
                        item.PenjualanId = order.Id;
                        if (item.Product != null)
                            dbContext.Entry(item.Product).State = EntityState.Unchanged;
                        dbContext.Penjualanitem.Add(item);
                    }
                    else
                    {
                        var oldItem = lastPenjualan.Items.SingleOrDefault(x => x.Id == item.Id);
                        if (item.Product != null)
                            dbContext.Entry(item.Product).State = EntityState.Unchanged;
                        dbContext.Entry(oldItem).CurrentValues.SetValues(item);
                    }
                }


                //remove

                foreach (var item in lastPenjualan.Items)
                {
                    var existsDb = order.Items.Where(x => x.Id == item.Id).FirstOrDefault();
                    if (existsDb == null)
                    {
                        dbContext.Penjualanitem.Remove(item);
                    }
                }

                dbContext.SaveChanges();
                trans.Commit();
                return Task.FromResult(order);
            }
            catch (Exception ex)
            {
                trans.Rollback();
                throw new SystemException(ex.Message);
            }
        }

        public Task<IEnumerable<PenjualanAndOrderModel>> GetPenjualans()
        {
            var datas = dbContext.Penjualan
                    .Include(x => x.Items)
                    .Include(x => x.OrderPenjualan).ThenInclude(x => x.Customer)
                    .Include(x => x.OrderPenjualan).ThenInclude(x => x.Sales);

            var result = datas.Select(x => new PenjualanAndOrderModel
            {
                PenjualanId = x.Id,
                OrderId = x.OrderPenjualanId,
                Invoice = x.Nomor,
                Customer = x.OrderPenjualan.Customer.Name,
                Sales = x.OrderPenjualan.Sales.Name,
                Total = x.Total,
                DeadLine = x.PayDeadLine,
                Created = x.CreateDate,
                Discount = x.Discount,
                NomorSO = x.OrderPenjualan.Nomor,
                PaymentStatus = x.Status,
                OrderStatus = x.OrderPenjualan.Status,
            });

            return Task.FromResult(result.AsEnumerable());
        }
        public Task<Penjualan> GetPenjualan(int id)
        {
            var orders = dbContext.Penjualan.Where(x=>x.Id==id)
                          .Include(x => x.Items).ThenInclude(x => x.Unit)
                          .Include(x => x.Items).ThenInclude(x => x.Product).ThenInclude(x=>x.Units)
                          .Include(x => x.OrderPenjualan).ThenInclude(x => x.Customer)
                          .Include(x => x.OrderPenjualan).ThenInclude(x => x.Sales).AsNoTracking();
            return Task.FromResult(orders.FirstOrDefault());
        }

        public Task<IEnumerable<PenjualanAndOrderModel>> GetPenjualansBySalesId(int id)
        {
            var datas = dbContext.Penjualan.Where(x => x.OrderPenjualan.SalesId == id )
                .Include(x=>x.Items)
                .Include(x=>x.OrderPenjualan).ThenInclude(x=>x.Sales)
                .Include(x=>x.OrderPenjualan).ThenInclude(x=>x.Customer)
                .Select(x => new PenjualanAndOrderModel {
                PenjualanId = x.Id,
                OrderId = x.OrderPenjualanId,
                Invoice = x.Nomor,
                Customer = x.OrderPenjualan.Customer.Name,
                Sales = x.OrderPenjualan.Sales.Name,
                Total = x.Total,
                DeadLine = x.PayDeadLine,
                Created = x.CreateDate, 
                PaymentStatus = x.Status, Discount = x.Discount, NomorSO=x.OrderPenjualan.Nomor, OrderStatus=x.OrderPenjualan.Status
            });
            return Task.FromResult(datas.AsEnumerable());
        }

        public Task<IEnumerable<PenjualanAndOrderModel>> GetPenjualansByCustomerId(int id)
        {
            var datas = dbContext.Penjualan.Where(x => x.OrderPenjualan.CustomerId == id)
              .Include(x => x.Items)
              .Include(x => x.OrderPenjualan).ThenInclude(x => x.Sales)
              .Include(x => x.OrderPenjualan).ThenInclude(x => x.Customer)
              .Select(x => new PenjualanAndOrderModel
              {
                  PenjualanId = x.Id,
                  OrderId = x.OrderPenjualanId,
                  Invoice = x.Nomor,
                  Customer = x.OrderPenjualan.Customer.Name,
                  Sales = x.OrderPenjualan.Sales.Name,
                  Total = x.Total,
                  DeadLine = x.PayDeadLine,
                  Created = x.CreateDate,
                  PaymentStatus = x.Status,
                  Discount = x.Discount,
                  NomorSO = x.OrderPenjualan.Nomor,
                  OrderStatus = x.OrderPenjualan.Status
              });
            return Task.FromResult(datas.AsEnumerable());
        }


        public  Task<bool> DeletePenjualan(int id)
        {
            try
            {
                var penjualan = dbContext.Penjualan.SingleOrDefault(x => x.Id == id);
                if (penjualan==null)
                    throw new SystemException("Penjualan Tidak Ditemukan !");
                dbContext.Penjualan.Remove(penjualan);
                dbContext.SaveChanges();
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public Task<IEnumerable<PenjualanAndOrderModel>> GetPenjualans(DateTime startDate, DateTime endDate)
        {
            try
            {
                var datas = dbContext.Penjualan.Where(x => x.CreateDate >= startDate && x.CreateDate <= endDate)
                    .Include(x => x.Items)
                    .Include(x => x.OrderPenjualan).ThenInclude(x => x.Customer)
                    .Include(x => x.OrderPenjualan).ThenInclude(x => x.Sales);

                var result = datas.Select(x => new PenjualanAndOrderModel
                {
                    PenjualanId = x.Id,
                    OrderId = x.OrderPenjualanId,
                    Invoice = x.Nomor,
                    Customer = x.OrderPenjualan.Customer.Name,
                    Sales = x.OrderPenjualan.Sales.Name,
                    Total = x.Total,
                    DeadLine = x.PayDeadLine,
                    Created = x.CreateDate,
                    Discount = x.Discount,
                    NomorSO = x.OrderPenjualan.Nomor,
                    PaymentStatus = x.Status,
                    OrderStatus = x.OrderPenjualan.Status,
                });

                return Task.FromResult(result.AsEnumerable());


            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new SystemException(ex.Message);
            }
        }

        #endregion


        #region Order
        public Task<Orderpenjualan> CreateOrder(Orderpenjualan order)
        {
            try
            {
                using var scope = provider.CreateScope();
                var _dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                if (order.Customer != null)
                    _dbContext.Entry(order.Customer).State = EntityState.Unchanged;

                if (order.Sales!= null)
                    _dbContext.Entry(order.Sales).State = EntityState.Unchanged;

                foreach (var item in order.Items)
                {
                    _dbContext.Entry(item.Product).State = EntityState.Unchanged;
                    _dbContext.Entry(item.Unit).State = EntityState.Unchanged;
                }
                _dbContext.Orderpenjualan.Add(order);
                _dbContext.SaveChanges();
                return Task.FromResult(order);
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public Task<bool> DeleteOrder(int id)
        {
            try
            {
                var oldData = dbContext.Orderpenjualan.Where(x => x.Id == id)
                    .Include(x=>x.Penjualan)
                    .ThenInclude(x=>x.Pembayaranpenjualan)
                    .FirstOrDefault();
                if (oldData==null)
                    throw new SystemException("Order Not Found !");

                if(oldData!=null &&  oldData.Penjualan !=null && oldData.Penjualan.Pembayaranpenjualan.Count>0)
                    throw new SystemException("Data Tidak Dihapus Karena Telah Ada Pembayaran !");

                dbContext.Orderpenjualan.Remove(oldData);
                dbContext.SaveChanges();
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public Task<Orderpenjualan> GetOrder(int id)
        {

            var orders = dbContext.Orderpenjualan.Where(x => x.Id == id)
            .Include(x => x.Customer)
            .Include(x => x.Sales)
            .Include(x => x.Items).ThenInclude(x => x.Unit).AsNoTracking()
            .Include(x => x.Items).ThenInclude(x => x.Product)
            .ThenInclude(x => x.Units);

            return Task.FromResult(orders.FirstOrDefault());
        }

        public Task<IEnumerable<PenjualanAndOrderModel>> GetOrders()
        {
            var orders = dbContext.Orderpenjualan
          .Include(x => x.Customer)
               .Include(x => x.Sales)
               .Include(x => x.Items);

            var results = orders.Select(x => new PenjualanAndOrderModel
            {
                OrderId = x.Id,
                NomorSO = x.Nomor,
                Customer = x.Customer.Name,
                Sales = x.Sales.Name,
                Total = x.Total,
                DeadLine = x.DeadLine,
                Created = x.OrderDate,
                Discount = x.Discount,
                OrderStatus = x.Status, 
            });

            return Task.FromResult(results.AsEnumerable());
        }

        public Task<IEnumerable<PenjualanAndOrderModel>> GetOrdersByCustomerId(int customerId)
        {
            var orders = dbContext.Orderpenjualan.Where(x => x.CustomerId == customerId)
          .Include(x => x.Customer)
               .Include(x => x.Sales)
               .Include(x => x.Items);

            var results = orders.Select(x => new PenjualanAndOrderModel
            {
                OrderId = x.Id,
                NomorSO = x.Nomor,
                Customer = x.Customer.Name,
                Sales = x.Sales.Name,
                Total = x.Total,
                DeadLine = x.DeadLine,
                Created = x.OrderDate,
                Discount = x.Discount,
                OrderStatus = x.Status
            });

            return Task.FromResult(results.AsEnumerable());
        }
        public Task<IEnumerable<PenjualanAndOrderModel>> GetOrdersBySalesId(int salesId)
        {
            var orders = dbContext.Orderpenjualan.Where(x => x.SalesId == salesId)
               .Include(x => x.Customer)
               .Include(x => x.Sales)
               .Include(x => x.Items).Select(x => new PenjualanAndOrderModel
            {
                OrderId = x.Id,
                NomorSO = x.Nomor,
                Customer = x.Customer.Name,
                Sales = x.Sales.Name,
                Total = x.Total,
                DeadLine = x.DeadLine,
                Created = x.OrderDate,
                Discount = x.Discount,
                OrderStatus = x.Status
            });

            return Task.FromResult(orders.AsEnumerable());
        }

        public Task<Orderpenjualan> UpdateOrder(int id, Orderpenjualan order)
        {
           // Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction trans=dbContext.Database.BeginTransaction();

            try
            {
                var lastOrder = dbContext.Orderpenjualan.Where(x => x.Id == id)
                                .Include(x => x.Customer)
                                .Include(x => x.Sales)
                                .Include(x => x.Items).FirstOrDefault();
                if (lastOrder == null)
                    throw new SystemException("Order Not Found  !");


                foreach (var item in order.Items)
                {
                    if (item.Id <= 0)
                    {
                        dbContext.Entry(item.Product).State = EntityState.Unchanged;
                        item.OrderPenjualanId = order.Id;
                        dbContext.OrderPenjualanItem.Add(item);
                    }
                    else
                    {
                        var olditem = lastOrder.Items.Where(x => x.Id == item.Id).FirstOrDefault();
                        if (olditem != null)
                        {
                            dbContext.Entry<OrderPenjualanItem>(olditem).CurrentValues.SetValues(item);
                        }
                    }
                }

                dbContext.Entry<Orderpenjualan>(lastOrder).CurrentValues.SetValues(order);
                foreach (var item in lastOrder.Items)
                {
                    var existsDb = order.Items.Where(x => x.Id == item.Id).FirstOrDefault();
                    if (existsDb == null)
                    {
                        lastOrder.Items.Remove(item);
                    }
                }

                var result=  dbContext.SaveChanges();
            //    trans.Commit();
                return Task.FromResult(order);
            }
            catch (Exception ex)
            {
              //  trans.Rollback();
                throw new SystemException(ex.Message);
            }
        }
        #endregion

        #region Pembayaran
        public  Task<Pembayaranpenjualan> CreatePembayaran(int penjualanId, Pembayaranpenjualan pembayaran, bool forced)
        {
            var trans = dbContext.Database.BeginTransaction();
            try
            {
                var penjualan = dbContext
                    .Penjualan.Where(x => x.Id == penjualanId)
                    .Include(x=>x.OrderPenjualan)
                    .Include(x=>x.Pembayaranpenjualan)
                    .Include(x => x.Items).FirstOrDefault();

                if (penjualan == null)
                    throw new SystemException("Pembelian Tidak Ditemukan");


                var totalInvoice = penjualan.Items.Sum(x => x.Total) - (penjualan.Items.Sum(x => x.Total) * (penjualan.Discount / 100));
                double totalBayar = 0;
                if (pembayaran != null)
                {
                    totalBayar = penjualan.Pembayaranpenjualan.Sum(x => x.PayValue);

                }

                var sisa = totalInvoice - totalBayar - pembayaran.PayValue;

                if (sisa < 0 && !forced)
                    throw new SystemException($"Pembayaran Melebihi Tagihan Invoice ! sisa {sisa}");


                var status = sisa > 0 ? PaymentStatus.Panjar : PaymentStatus.Lunas;
                penjualan.Status = status;
                dbContext.Pembayaranpenjualan.Add(pembayaran);
                penjualan.OrderPenjualan.Status = OrderStatus.Selesai;
                dbContext.SaveChanges();
                trans.Commit();
                return Task.FromResult(pembayaran);
            }
            catch (Exception ex)
            {
                trans.Rollback();
                _logger.LogError(ex.Message);
                throw new SystemException(ex.Message);
            }
        }

        public Task<IEnumerable<Pembayaranpenjualan>> GetPembayaran(int penjualanId)
        {
            var pembayarans = dbContext.Pembayaranpenjualan.Where(x => x.PenjualanId == penjualanId);
            return Task.FromResult(pembayarans.AsEnumerable());
        }


        public Task<bool> UpdatePembayaran(Pembayaranpenjualan model)
        {
            try
            {
                var penjualan = dbContext.Penjualan.Where(x => x.Id == model.PenjualanId)
                    .Include(x=>x.Items)
                    .Include(x=>x.OrderPenjualan)
                    .Include(x=>x.Pembayaranpenjualan).FirstOrDefault();

                if (penjualan == null || !penjualan.Pembayaranpenjualan.Any())
                    throw new SystemException("Data Penjualan atau Pembayaran Tidak Ditemukan");


                var totalInvoice = penjualan.Items.Sum(x => x.Total) - (penjualan.Items.Sum(x => x.Total) * (penjualan.Discount / 100));
                var totalWithoutCurrentPayment = penjualan.Pembayaranpenjualan.Where(x => x.Id != model.Id).Sum(x => x.PayValue);
              
                if (totalWithoutCurrentPayment + model.PayValue > totalInvoice)
                    throw new SystemException("Maaf, Nilai Bayar Terlalu Besar !");

                var oldPembayaran = penjualan.Pembayaranpenjualan.Where(x => x.Id == model.Id).FirstOrDefault();
                if (oldPembayaran == null)
                    throw new SystemException("Pembayaran Tidak Ditemukan");

                if (totalWithoutCurrentPayment + model.PayValue < totalInvoice)
                {
                    penjualan.OrderPenjualan.Status = OrderStatus.Diproses;
                    penjualan.Status =  PaymentStatus.Panjar;
                }
                else
                {
                    penjualan.OrderPenjualan.Status = OrderStatus.Selesai;
                    penjualan.Status = PaymentStatus.Lunas;
                }

                dbContext.Entry(oldPembayaran).CurrentValues.SetValues(model);
                dbContext.SaveChanges();
                return Task.FromResult(true);

            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        #endregion

    }
}
