using Microsoft.EntityFrameworkCore;
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
        private readonly ApplicationDbContext dbContext;
     //   private readonly IHttpContextAccessor auth;
        private readonly ILogger<PenjualanService> _logger;

        public PenjualanService(ApplicationDbContext db, ILogger<PenjualanService> log)
        {
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
                var lastOrder = await GetOrder(orderid);

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
                lastOrder.Status = OrderStatus.Proccess;
                await dbContext.SaveChangesAsync();
                trans.Commit();
                return penjualan;
            }
            catch (Exception ex)
            {

                 trans.Rollback();
                throw new SystemException(ex.Message);
            }
        }
        public async Task<Penjualan> UpdatePenjualan(int penjualanId, Penjualan order)
        {
            var trans = dbContext.Database.BeginTransaction();
            try
            {
                var lastPenjualan = dbContext.Penjualan.Where(x => x.Id == penjualanId)  .Include(x=>x.Items)
                                 .FirstOrDefault();


                if (lastPenjualan == null)
                    throw new SystemException("Penjualan Not Found  !");

                order.Activity = order.Activity == ActivityStatus.None ? ActivityStatus.Created :order.Activity;
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

                await dbContext.SaveChangesAsync();
                trans.Commit();
                return order;
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


        public async Task<bool> DeletePenjualan(int id)
        {
            try
            {
                var penjualan = dbContext.Penjualan.SingleOrDefault(x => x.Id == id);
                if (penjualan==null)
                    throw new SystemException("Penjualan Tidak Ditemukan !");
                dbContext.Penjualan.Remove(penjualan);
                await dbContext.SaveChangesAsync();
                return true;
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
        public async Task<Orderpenjualan> CreateOrder(Orderpenjualan order)
        {
            try
            {
                if (order.Customer != null)
                    dbContext.Entry(order.Customer).State = EntityState.Unchanged;
                foreach (var item in order.Items)
                {
                    dbContext.Entry(item.Product).State = EntityState.Unchanged;
                    dbContext.Entry(item.Unit).State = EntityState.Unchanged;
                }   
                dbContext.Orderpenjualan.Add(order);
                await dbContext.SaveChangesAsync();
                return order;
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public async Task<bool> DeleteOrder(int id)
        {
            try
            {
                var oldData = dbContext.Orderpenjualan.Where(x => x.Id == id)
                    .Include(x=>x.Penjualan)
                    .ThenInclude(x=>x.Pembayaranpenjualan)
                    .FirstOrDefault();
                if (oldData==null)
                    throw new SystemException("Order Not Found !");

                if(oldData!=null && oldData.Penjualan.Pembayaranpenjualan.Count>0)
                    throw new SystemException("Data Tidak Dihapus Karena Telah Ada Pembayaran !");

                dbContext.Orderpenjualan.Remove(oldData);
                await dbContext.SaveChangesAsync();
                return true;
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
            .Include(x => x.Items)
            .ThenInclude(x => x.Product).ThenInclude(x => x.Units);

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

        public async Task<Orderpenjualan> UpdateOrder(int id, Orderpenjualan order)
        {
            Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction trans=dbContext.Database.BeginTransaction();

            try
            {
                var lastOrder = await GetOrder(id);
                if (lastOrder == null)
                    throw new SystemException("Order Not Found  !");

                dbContext.Entry(lastOrder).CurrentValues.SetValues(order);

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
                        var olditem = lastOrder.Items.SingleOrDefault(x => x.Id == item.Id);
                        dbContext.Entry(olditem).CurrentValues.SetValues(item);
                    }
                }

                //remove

                foreach (var item in lastOrder.Items)
                {
                    var existsDb = order.Items.SingleOrDefault(x => x.Id == item.Id);
                    if (existsDb == null)
                    {
                        dbContext.OrderPenjualanItem.Remove(existsDb);
                    }
                }

                await dbContext.SaveChangesAsync();
                trans.Commit();
                return order;
            }
            catch (Exception ex)
            {
                trans.Rollback();
                throw new SystemException(ex.Message);
            }
        }
        #endregion

        #region Pembayaran
        public async Task<Pembayaranpenjualan> CreatePembayaran(int penjualanId, Pembayaranpenjualan pembayaran, bool forced)
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


                var status = sisa > 0 ? PaymentStatus.DownPayment : PaymentStatus.PaidOff;
                penjualan.Status = status;
                dbContext.Pembayaranpenjualan.Add(pembayaran);
                penjualan.OrderPenjualan.Status = OrderStatus.Complete;
                await dbContext.SaveChangesAsync();
                trans.Commit();
                return pembayaran;
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

      
        #endregion

    }
}
