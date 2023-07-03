using ApsWebApp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShareModels;
using ShareModels.ModelViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApsWebApp.Services
{
    public class PenjualanService : IPenjualanService
    {
        //private readonly IServiceProvider provider;
        private readonly ApplicationDbContext dbContext;
        //   private readonly IHttpContextAccessor auth;
        private readonly ILogger<PenjualanService> _logger;

        public PenjualanService(ApplicationDbContext db, IServiceProvider _provider, ILogger<PenjualanService> log)
        {
            dbContext = db;
            _logger = log;
        }

        #region penjualan
        public Task<Penjualan> CreatePenjualan(int orderid, Penjualan model)
        {
            var trans = dbContext.Database.BeginTransaction();
            try
            {
                dbContext.ChangeTracker.Clear();

                dbContext.Entry(model.Customer).State = EntityState.Unchanged;
                dbContext.Entry(model.Gudang).State = EntityState.Unchanged;
                dbContext.Entry(model.Salesman).State = EntityState.Unchanged;

                var tracker = dbContext.ChangeTracker.Entries();
                foreach (var item in model.Items)
                {
                    dbContext.Entry(item.Product).State = EntityState.Unchanged;
                    dbContext.Entry(item.Unit).State = EntityState.Unchanged;

                }
                if (model.OrderPenjualan != null)
                {
                    dbContext.Entry(model.OrderPenjualan).State = EntityState.Modified;
                    model.OrderPenjualan.Status = OrderStatus.Diproses;
                }


                dbContext.ChangeTracker.DisplayTrackedEntities();
                dbContext.Penjualan.Add(model);
                dbContext.SaveChanges();
                trans.Commit();
                return Task.FromResult(model);
            }
            catch (Exception ex)
            {

                trans.Rollback();
                throw new SystemException(ex.Message);
            }
        }
        public Task<Penjualan> UpdatePenjualan(int penjualanId, Penjualan order)
        {

            //dbContext.ChangeTracker.Clear();

            var trans = dbContext.Database.BeginTransaction();
            try
            {
                var lastPenjualan = dbContext.Penjualan.AsNoTracking().Where(x => x.Id == penjualanId).Include(x => x.Items)
                                 .FirstOrDefault();
                if (lastPenjualan == null)
                    throw new SystemException("Penjualan Not Found  !");
                order.Activity = order.Activity == ActivityStatus.None ? ActivityStatus.Created : order.Activity;
                //order.OrderPenjualan.Status = OrderStatus.Diproses;
                dbContext.Entry(lastPenjualan).CurrentValues.SetValues(order);

                foreach (var item in order.Items)
                {
                    if (item.Id <= 0)
                    {
                        if (item.Product != null)
                        {
                            dbContext.Entry(item.Product).State = EntityState.Unchanged;
                            dbContext.Entry(item.Product.UnitSelected).State = EntityState.Unchanged;
                        }
                        dbContext.Penjualanitem.Add(item);
                    }
                    else
                    {
                        var oldItem = lastPenjualan.Items.SingleOrDefault(x => x.Id == item.Id);
                        if (item.Unit != oldItem.Unit ||
                            item.Discount != oldItem.Discount ||
                            item.Price != oldItem.Price ||
                            item.Quantity != oldItem.Quantity
                            )
                        {
                            oldItem.Unit = item.Unit;
                            oldItem.Discount = item.Discount;
                            oldItem.Price = item.Price;
                            oldItem.Quantity = item.Quantity;
                        }


                    }
                }


                //remove

                foreach (var item in lastPenjualan.Items)
                {
                    var existsDb = order.Items.Where(x => x.Id == item.Id).FirstOrDefault();
                    if (existsDb == null)
                    {
                        lastPenjualan.Items.Remove(item);
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
                 .Include(x => x.Gudang)
                    .Include(x => x.Items).ThenInclude(x => x.Product)
                    .Include(x => x.Customer)
                    .Include(x => x.Salesman).ToList();

            var result = datas.Select(x => new PenjualanAndOrderModel
            {
                Gudang = x.Gudang,
                PenjualanId = x.Id,
                Invoice = x.Nomor,
                Customer = x.Customer.Name,
                Sales = x.Salesman.Name,
                Total = x.Total,
                DeadLine = x.DeadLine,
                Created = x.CreateDate,
                NomorSO = x.Nomor,
                PaymentStatus = x.Status,
            });

            return Task.FromResult(result.AsEnumerable());
        }
        public Task<Penjualan> GetPenjualan(int id)
        {
            var orders = dbContext.Penjualan.Where(x => x.Id == id)
                .Include(x => x.Gudang)
                          .Include(x => x.Items).ThenInclude(x => x.Product)
                          .ThenInclude(x => x.Units)
                          .Include(x => x.Items).ThenInclude(x => x.Unit)
                          .Include(x => x.Customer)
                          .Include(x => x.Salesman);
            var datas = orders.FirstOrDefault();
            return Task.FromResult(datas);
        }

        public Task<IEnumerable<PenjualanAndOrderModel>> GetPenjualansBySalesId(int id)
        {
            var datas = dbContext.Penjualan
                .Include(x => x.Gudang)
                   .Include(x => x.Items).ThenInclude(x => x.Product)
                   .Include(x => x.Customer)
                   .Include(x => x.Salesman).Where(x => x.Salesman.Id == id).ToList();

            var result = datas.Select(x => new PenjualanAndOrderModel
            {
                Gudang = x.Gudang,
                PenjualanId = x.Id,
                Invoice = x.Nomor,
                Customer = x.Customer.Name,
                Sales = x.Salesman.Name,
                Total = x.Total,
                DeadLine = x.DeadLine,
                Created = x.CreateDate,
                NomorSO = x.Nomor,
                PaymentStatus = x.Status,    SalesId=x.Salesman.Id, 
            });

            return Task.FromResult(result.AsEnumerable());
        }

        public Task<IEnumerable<PenjualanAndOrderModel>> GetPenjualansByCustomerId(int id)
        {
            var datas = dbContext.Penjualan
                .Include(x => x.Gudang)
                   .Include(x => x.Items).ThenInclude(x => x.Product)
                   .Include(x => x.Customer)
                   .Include(x => x.Salesman).Where(x => x.Customer.Id == id).ToList();

            var result = datas.Select(x => new PenjualanAndOrderModel
            {
                Gudang = x.Gudang,
                PenjualanId = x.Id,
                Invoice = x.Nomor,
                Customer = x.Customer.Name,
                Sales = x.Salesman.Name,
                Total = x.Total,
                DeadLine = x.DeadLine,
                Created = x.CreateDate,
                NomorSO = x.Nomor,
                PaymentStatus = x.Status,   SalesId = x.Salesman.Id,
            });

            return Task.FromResult(result.AsEnumerable());
        }


        public Task<bool> DeletePenjualan(int id)
        {
            try
            {
                var penjualan = dbContext.Penjualan.SingleOrDefault(x => x.Id == id);
                if (penjualan == null)
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
                      .Include(x => x.Gudang)
                    .Include(x => x.Items)
                    .Include(x => x.Customer)
                    .Include(x => x.Salesman);

                var result = datas.Select(x => new PenjualanAndOrderModel
                {
                    Gudang = x.Gudang,
                    PenjualanId = x.Id,
                    Invoice = x.Nomor,
                    Customer = x.Customer.Name,
                    Sales = x.Salesman.Name,
                    Total = x.Total,
                    DeadLine = x.DeadLine,
                    Created = x.CreateDate,
                    NomorSO = x.Nomor,
                    PaymentStatus = x.Status,       SalesId=x.Salesman.Id
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
        public Task<OrderPenjualan> CreateOrder(OrderPenjualan order)
        {
            try
            {


                this.ValidateCreateOrder(order);
                dbContext.ChangeTracker.Clear();

                if (order.Customer != null)
                    dbContext.Entry(order.Customer).State = EntityState.Unchanged;
                if (order.Gudang != null)
                    dbContext.Entry(order.Gudang).State = EntityState.Unchanged;

                if (order.Sales != null)
                    dbContext.Entry(order.Sales).State = EntityState.Unchanged;

                foreach (var item in order.Items)
                {
                    dbContext.Entry(item.Product).State = EntityState.Unchanged;
                    dbContext.Entry(item.Unit).State = EntityState.Unchanged;
                }
                dbContext.OrderPenjualan.Add(order);
                dbContext.SaveChanges();
                return Task.FromResult(order);
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        private bool ValidateCreateOrder(OrderPenjualan order)
        {
            if (order.Customer == null)
                throw new SystemException("Customer Tidak Boleh Kosong !");

            if (order.Sales == null)
                throw new SystemException("Salesman Tidak Boleh Kosong !");

            if (order.Gudang == null)
                throw new SystemException("Pilih Gudang Sumber !");
            return true;
        }

        public Task<bool> DeleteOrder(int id)
        {
            try
            {
                var oldData = dbContext.OrderPenjualan.Where(x => x.Id == id)
                    .FirstOrDefault();
                if (oldData == null)
                    throw new SystemException("Order Not Found !");

                dbContext.OrderPenjualan.Remove(oldData);
                dbContext.SaveChanges();
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public async Task<OrderPenjualan?> GetOrder(int id)
        {

            var order = await dbContext.OrderPenjualan.Where(x => x.Id == id)
            .Include(x => x.Customer)
            .Include(x => x.Sales)
            .Include(x => x.Gudang)
            .Include(x => x.Items).ThenInclude(x => x.Unit).AsNoTracking()
            .Include(x => x.Items)
            .ThenInclude(x => x.Product).ThenInclude(x => x.Supplier)
            .Include(x => x.Items).ThenInclude(x => x.Product).ThenInclude(x => x.Units).FirstOrDefaultAsync();
            return order;
        }

        public Task<IEnumerable<PenjualanAndOrderModel>> GetOrders()
        {
            var orders = dbContext.OrderPenjualan
          .Include(x => x.Customer)
               .Include(x => x.Sales)
               .Include(x => x.Gudang)
               .Include(x => x.Items);

            var results = orders.Select(x => new PenjualanAndOrderModel
            {
                Gudang = x.Gudang,
                OrderId = x.Id,
                NomorSO = x.Nomor,
                Customer = x.Customer.Name,
                Sales = x.Sales.Name,
                Total = x.Total,
                DeadLine = x.DeadLine,
                Created = x.OrderDate,
                OrderStatus = x.Status,   SalesId= x.Sales.Id
            });

            return Task.FromResult(results.AsEnumerable());
        }

        public Task<IEnumerable<PenjualanAndOrderModel>> GetOrdersByCustomerId(int customerId)
        {
            var orders = dbContext.OrderPenjualan.Where(x => x.Customer.Id == customerId)
          .Include(x => x.Customer)
               .Include(x => x.Sales)
                .Include(x => x.Gudang)
               .Include(x => x.Items);

            var results = orders.Select(x => new PenjualanAndOrderModel
            {
                Gudang = x.Gudang,
                OrderId = x.Id,
                NomorSO = x.Nomor,
                Customer = x.Customer.Name,
                Sales = x.Sales.Name,
                Total = x.Total,
                DeadLine = x.DeadLine,
                Created = x.OrderDate,
                OrderStatus = x.Status,
                SalesId = x.Sales.Id
            });

            return Task.FromResult(results.AsEnumerable());
        }
        public Task<IEnumerable<PenjualanAndOrderModel>> GetOrdersBySalesId(int salesId)
        {
            var orders = dbContext.OrderPenjualan.Where(x => x.Sales.Id == salesId)
               .Include(x => x.Gudang)
               .Include(x => x.Customer)
               .Include(x => x.Sales)
               .Include(x => x.Items).Select(x => new PenjualanAndOrderModel
               {
                   Gudang = x.Gudang,
                   OrderId = x.Id,
                   NomorSO = x.Nomor,
                   Customer = x.Customer.Name,
                   Sales = x.Sales.Name,
                   Total = x.Total,
                   DeadLine = x.DeadLine,
                   Created = x.OrderDate,
                   OrderStatus = x.Status,
                   SalesId = x.Sales.Id
               });

            return Task.FromResult(orders.AsEnumerable());
        }

        public Task<OrderPenjualan> UpdateOrder(int id, OrderPenjualan order)
        {
            Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction trans = dbContext.Database.BeginTransaction();
            try
            {

                //    dbContext.ChangeTracker.Clear();
                ValidateCreateOrder(order);
                var lastOrder = dbContext.OrderPenjualan.Where(x => x.Id == id)
                                .Include(x => x.Gudang)
                                .Include(x => x.Customer)
                                .Include(x => x.Sales)
                                .Include(x => x.Items).FirstOrDefault();

                if (lastOrder == null)
                    throw new SystemException("Order Not Found  !");


                if (lastOrder.Customer.Id != order.Customer.Id)
                {
                    lastOrder.Customer = order.Customer;
                }

                if (lastOrder.Sales.Id != order.Sales.Id)
                {
                    lastOrder.Sales = order.Sales;
                }

                if (lastOrder.Gudang.Id != order.Gudang.Id)
                {
                    lastOrder.Gudang = order.Gudang;
                }
                lastOrder.DeadLine = order.DeadLine;

                foreach (var item in order.Items)
                {
                    if (item.Id <= 0)
                    {
                        dbContext.Entry(item.Product).State = EntityState.Detached;
                        dbContext.Entry(item.Unit).State = EntityState.Detached;
                        dbContext.OrderPenjualanItem.Add(item);
                    }
                    else
                    {
                        var olditem = lastOrder.Items.Where(x => x.Id == item.Id).FirstOrDefault();
                        if (olditem != null)
                        {
                            if (olditem.Product != null)
                                dbContext.Entry(olditem.Product).State = EntityState.Unchanged;
                            if (olditem.Unit != null)
                                dbContext.Entry(olditem.Unit).State = EntityState.Unchanged;
                            dbContext.Entry<OrderPenjualanItem>(olditem).CurrentValues.SetValues(item);
                        }
                    }
                }

                dbContext.Entry<OrderPenjualan>(lastOrder).CurrentValues.SetValues(order);
                foreach (var item in lastOrder.Items)
                {
                    var existsDb = order.Items.Where(x => x.Id == item.Id).FirstOrDefault();
                    if (existsDb == null)
                    {
                        lastOrder.Items.Remove(item);
                    }
                }
                var result = dbContext.SaveChanges();
                trans.Commit();
                return Task.FromResult(order);
            }
            catch (Exception ex)
            {
                trans.Rollback();
                throw new SystemException(ex.Message);
            }
        }
        #endregion

        #region Pembayaran
        public Task<PembayaranPenjualan> CreatePembayaran(int penjualanId, PembayaranPenjualan pembayaran, bool forced)
        {
            if (pembayaran.Id > 0)
                throw new SystemException("Data Sudah Disimpan !");

            var trans = dbContext.Database.BeginTransaction();
            try
            {
                var penjualan = dbContext
                    .Penjualan.Where(x => x.Id == penjualanId)
                    .Include(x => x.PembayaranPenjualan)
                    .Include(x => x.Items).SingleOrDefault();

                if (penjualan == null)
                    throw new SystemException("Penjualan Tidak Ditemukan !");


                var totalInvoice = penjualan.Total - penjualan.TotalDiscount;
                double totalBayar = 0;
                if (pembayaran != null)
                {
                    totalBayar = penjualan.PembayaranPenjualan.Sum(x => x.PayValue);

                }

                var sisa = totalInvoice - totalBayar - pembayaran.PayValue;

                if (sisa < 0 && !forced)
                    throw new SystemException($"Pembayaran Melebihi Tagihan Invoice ! sisa {sisa}");

                //if (pembayaran.Penjualan != null)
                //    dbContext.Entry(pembayaran.Penjualan).State = EntityState.Unchanged;

                penjualan.PembayaranPenjualan.Add(pembayaran);

                var status = sisa > 0 ? PaymentStatus.Panjar : PaymentStatus.Lunas;
                penjualan.Status = status;
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

        public Task<IEnumerable<PembayaranPenjualan>> GetPembayaran(int penjualanId)
        {
            var penjualan = dbContext.Penjualan.Where(x=>x.Id==penjualanId).Include(x => x.PembayaranPenjualan);
            return Task.FromResult(penjualan!=null?penjualan.FirstOrDefault().PembayaranPenjualan.AsEnumerable():null);
        }


        public Task<bool> UpdatePembayaran(PembayaranPenjualan model)
        {
            try
            {
                var penjualan = dbContext.Penjualan.Where(x => x.Id == model.Penjualan.Id)
                    .Include(x => x.Items)
                    .Include(x => x.PembayaranPenjualan).FirstOrDefault();

                if (penjualan == null || !penjualan.PembayaranPenjualan.Any())
                    throw new SystemException("Data Penjualan atau Pembayaran Tidak Ditemukan");


                var totalInvoice = penjualan.Total - penjualan.TotalDiscount;
                var totalWithoutCurrentPayment = penjualan.PembayaranPenjualan.Where(x => x.Id != model.Id).Sum(x => x.PayValue);

                if (totalWithoutCurrentPayment + model.PayValue > totalInvoice)
                    throw new SystemException("Maaf, Nilai Bayar Terlalu Besar !");

                var oldPembayaran = penjualan.PembayaranPenjualan.Where(x => x.Id == model.Id).FirstOrDefault();
                if (oldPembayaran == null)
                    throw new SystemException("Pembayaran Tidak Ditemukan");
                dbContext.Entry(oldPembayaran).CurrentValues.SetValues(model);
                if (totalWithoutCurrentPayment + model.PayValue < totalInvoice)
                {
                    if (penjualan.OrderPenjualan != null)
                        penjualan.OrderPenjualan.Status = OrderStatus.Diproses;
                    model.Penjualan.Status = PaymentStatus.Panjar;
                    penjualan.Status = PaymentStatus.Panjar;
                }
                else
                {
                    if (penjualan.OrderPenjualan != null)
                        penjualan.OrderPenjualan.Status = OrderStatus.Selesai;
                    model.Penjualan.Status = PaymentStatus.Lunas;
                    penjualan.Status = PaymentStatus.Lunas;
                }

                var result = dbContext.SaveChangesAsync().Result;
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
