using ApsWebApp.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Ocph.DAL;
using ShareModels;
using ShareModels.ModelViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApsWebApp.Services
{

    public class PembelianService : IPembelianService
    {
        private readonly ApplicationDbContext dbContext;
        //    private readonly IHttpContextAccessor auth;
        private readonly ILogger _logger;

        public PembelianService(ILogger<PembelianService> logger, ApplicationDbContext db)
        {
            dbContext = db;
            //     auth = httpContextAccessor;
            _logger = logger;
        }

        #region Pembelian
        public Task<Pembelian> CreatePembelian(int orderid, int gudangid)
        {
            var trans = dbContext.Database.BeginTransaction();
            try
            {

                var gudang = dbContext.Gudang.Where(x => x.Id == gudangid).FirstOrDefault();
                if (gudang == null)
                    throw new SystemException("Gudang tidak ditemukan !");

                var lastOrder = dbContext.OrderPembelian.Where(x => x.Id == orderid)
                    .Include(x => x.Items).ThenInclude(x => x.Product)
                    .Include(x => x.Items).ThenInclude(x => x.Unit)
                    .Include(x => x.Supplier)
                    .FirstOrDefault();

                if (lastOrder == null)
                    throw new SystemException("Order Tidak Ditemukan !");


                dbContext.Set<Pembelian>().AsNoTracking();
                var pembelian = new Pembelian
                {
                    Gudang = gudang,
                    OrderPembelianId = orderid,
                    OrderPembelian = lastOrder,
                    CreatedDate = DateTime.Now,
                    Items = new List<PembelianItem>()
                };

                foreach (var item in lastOrder.Items)
                {
                    var data = new PembelianItem
                    {
                        Amount = item.Quntity,
                        Price = item.Price,
                        Product = item.Product,
                        Unit = item.Unit
                    };
                    pembelian.Items.Add(data);
                }


                dbContext.Pembelian.Add(pembelian);
                lastOrder.Status = OrderStatus.Diproses;
                dbContext.SaveChanges();
                trans.Commit();
                return Task.FromResult(pembelian);
            }
            catch (Exception ex)
            {
                try
                {
                    _logger.LogError(ex.Message);
                    trans.Rollback();
                    throw new SystemException(ex.Message);
                }
                catch (System.Exception exx)
                {
                    throw new SystemException(exx.Message);
                }
            }
        }


        public Task<Pembelian> UpdatePembelian(int pembelianId, Pembelian order)
        {
            var trans = dbContext.Database.BeginTransaction();
            try
            {

                var lastOrder = dbContext.Pembelian.Where(x => x.Id == pembelianId).Include(x => x.OrderPembelian).FirstOrDefault();

                if (lastOrder == null)
                    throw new SystemException("Pembelian Not Found  !");

                lastOrder.OrderPembelian.Status = OrderStatus.Diproses;
                dbContext.Entry(lastOrder).CurrentValues.SetValues(order);

                foreach (var item in order.Items)
                {
                    if (item.Id <= 0)
                    {
                        dbContext.PembelianItem.Add(item);
                    }
                    else
                    {
                        var oldItem = dbContext.PembelianItem.SingleOrDefault(x => x.Id == item.Id);
                        if (oldItem == null)
                            throw new SystemException("Item Pembelian Tidak Ditemukan !");
                        dbContext.Entry(oldItem).CurrentValues.SetValues(item);
                    }
                }


                //remove

                foreach (var item in lastOrder.Items)
                {
                    var existsDb = order.Items.Where(x => x.Id == item.Id).FirstOrDefault();
                    if (existsDb == null)
                    {
                        dbContext.PembelianItem.Remove(existsDb);
                    }
                }


                dbContext.SaveChanges();
                trans.Commit();
                return Task.FromResult(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                trans.Rollback();
                throw new SystemException(ex.Message);
            }
        }

        public Task<Pembelian> GetPembelian(int id)
        {
            try
            {
                var pembelians = dbContext.Pembelian.Where(x => x.Id == id)
                                   .Include(x => x.Gudang)
                                   .Include(x => x.Items)
                                   .ThenInclude(x => x.Product)
                                   .ThenInclude(x => x.Units)
                                   .Include(x => x.OrderPembelian).ThenInclude(x => x.Supplier);
                return Task.FromResult(pembelians.FirstOrDefault());
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new SystemException(ex.Message);
            }
        }

        public Task<IEnumerable<PembelianDataModel>> GetPembelians()
        {
            try
            {
                var pembelians = dbContext.Pembelian
                    .Include(x=>x.Gudang)
                                   .Include(x => x.Items).ThenInclude(x => x.Product).ThenInclude(x => x.Units)
                                   .Include(x => x.OrderPembelian).ThenInclude(x => x.Supplier).ToList();

                var datas = from a in pembelians
                            select new PembelianDataModel
                            {
                                CreatedDate = a.CreatedDate,
                                DeadLine = a.DeadLine,
                                Gudang = a.Gudang.Name,
                                Id = a.Id,
                                InvoiceNumber = a.InvoiceNumber,
                                OrderNumber = a.OrderPembelian.Nomor,
                                Items = a.Items,
                                OrderPembelianId = a.OrderPembelianId,
                                Status = a.Status,
                                SupplierId = a.OrderPembelian.Supplier.Id,
                                SupplierName = a.OrderPembelian.Supplier.Nama
                            };

                return Task.FromResult(datas.AsEnumerable());
            }
            catch (System.Exception ex)
            {

                _logger.LogError(ex.Message);
                throw new SystemException(ex.Message);
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public Task<IEnumerable<Pembelian>> GetPembeliansBySupplierId(int id)
        {
            var pembelians = dbContext.Pembelian
                                  .Include(x => x.Items).ThenInclude(x => x.Product).ThenInclude(x => x.Units)
                                  .Include(x => x.OrderPembelian).ThenInclude(x => x.Supplier).AsNoTracking();
            return Task.FromResult(pembelians.Where(x => x.OrderPembelian.Supplier.Id == id).AsEnumerable());
        }


        public Task<bool> DeletePembelian(int id)
        {
            try
            {
                var oldData = dbContext.Pembelian.SingleOrDefault(x => x.Id == id);
                if (oldData == null)
                    throw new SystemException("Pembelian Not Found !");

                dbContext.Pembelian.Remove(oldData);

                dbContext.SaveChanges();

                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new SystemException(ex.Message);
            }
        }
        #endregion

        #region Orders
        public Task<OrderPembelian> CreateOrder(OrderPembelian order)
        {
            try
            {

                dbContext.ChangeTracker.Clear();

                dbContext.Set<OrderPembelian>().AsNoTracking();

                var entries = dbContext.ChangeTracker.Entries();

                if (order.Supplier != null)
                    dbContext.Entry(order.Supplier).State = EntityState.Unchanged;
                foreach (var item in order.Items)
                {
                    dbContext.Entry(item.Product).State = EntityState.Unchanged;
                    dbContext.Entry(item.Unit).State = EntityState.Unchanged;
                }
                dbContext.OrderPembelian.Add(order);
                dbContext.SaveChanges();
                return Task.FromResult(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new SystemException(ex.Message);
            }
        }

        public Task<bool> DeleteOrder(int id)
        {
            try
            {
                var deleted = dbContext.OrderPembelian.SingleOrDefault(x => x.Id == id);
                if (deleted == null)
                    throw new SystemException("Order Not Found !");
                dbContext.OrderPembelian.Remove(deleted);
                dbContext.SaveChanges();
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new SystemException(ex.Message);
            }
        }

        public Task<OrderPembelian> GetOrder(int id)
        {
            var lastOrder = dbContext.OrderPembelian.Where(x => x.Id == id)
                            .Include(x => x.Supplier)
                            .Include(x => x.Items).ThenInclude(x => x.Product).ThenInclude(x => x.Units);
            return Task.FromResult(lastOrder.FirstOrDefault());
        }

        public Task<IEnumerable<OrderPembelian>> GetOrders()
        {
            var lastOrder = dbContext.OrderPembelian
                           .Include(x => x.Supplier)
                           .Include(x => x.Items).ThenInclude(x => x.Product).ThenInclude(x => x.Units).AsNoTracking();
            return Task.FromResult(lastOrder.AsEnumerable());
        }

        public Task<IEnumerable<OrderPembelian>> GetOrdersBySupplierId(int supplierId)
        {
            var lastOrder = dbContext.OrderPembelian.Where(x => x.Supplier.Id == supplierId)
                            .Include(x => x.Supplier)
                            .Include(x => x.Items).ThenInclude(x => x.Product).ThenInclude(x => x.Units).AsNoTracking();
            return Task.FromResult(lastOrder.AsEnumerable());
        }



        public Task<OrderPembelian> UpdateOrder(int id, OrderPembelian order)
        {
            try
            {


                var lastOrder = dbContext.OrderPembelian.Where(x => x.Id == id).Include(x => x.Supplier).Include(x => x.Items).AsNoTracking().FirstOrDefault();


                if (lastOrder == null)
                    throw new SystemException("Order Not Found  !");

                lastOrder.OrderDate = order.OrderDate;
                lastOrder.Status = order.Status;
                foreach (var item in order.Items)
                {
                    if (item.Id <= 0)
                    {
                        dbContext.Entry(item.Unit).State = EntityState.Unchanged;
                        dbContext.Entry(item.Product).State = EntityState.Unchanged;
                        dbContext.OrderPembelianItem.Add(item);
                    }
                    else
                    {
                        var updatedItem = dbContext.OrderPembelianItem.AsNoTracking().SingleOrDefault(x => x.Id == item.Id);
                        if (updatedItem == null)
                            throw new SystemException("Order Item Not Found !");
                        dbContext.Entry(updatedItem).CurrentValues.SetValues(item);
                    }
                }

                //remove

                if (order.Items.Count != lastOrder.Items.Count)
                {
                    foreach (var item in lastOrder.Items)
                    {
                        var existsDb = order.Items.SingleOrDefault(x => x.Id == item.Id);
                        if (existsDb == null)
                        {
                            dbContext.OrderPembelianItem.Remove(existsDb);
                        }
                    }
                }



                dbContext.SaveChanges();
                return Task.FromResult(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new SystemException(ex.Message);
            }
        }


        #endregion


        #region Pembayaran
        public Task<PembayaranPembelian> CreatePembayaran(int pembelianId, PembayaranPembelian pembayaran)
        {
            var trans = dbContext.Database.BeginTransaction();
            try
            {
                var pembelian = dbContext.Pembelian.Where(x => x.Id == pembelianId)
                    .Include(x => x.Items)
                    .Include(x => x.OrderPembelian)
                    .Include(x => x.PembayaranPembelian).FirstOrDefault();

                if (pembelian == null)
                    throw new SystemException("Pembelian Tidak Ditemukan");

                var totalInvoice = pembelian.Total - pembelian.TotalDiscount;
                double totalBayar = 0;

                totalBayar = pembelian.PembayaranPembelian.Sum(x => x.PayValue);
                var sisa = totalInvoice - totalBayar - pembayaran.PayValue;

                if (sisa < 0 )
                    throw new SystemException("Pembayaran Anda Melebihi Tagihan Invoice !");

                var status = sisa > 0 ? PaymentStatus.Panjar : PaymentStatus.Lunas;
                pembelian.Status = status;
                pembelian.OrderPembelian.Status = OrderStatus.Selesai;
                pembelian.PembayaranPembelian.Add(pembayaran);
                var result = dbContext.SaveChanges();

                if (result <= 0)
                    throw new SystemException("Pembayaran Gagal !");

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

        public Task<IEnumerable<PembayaranPembelian>> GetPembayaran(int pembelianId)
        {
            var pembayaran = dbContext.Pembelian.Include(x => x.PembayaranPembelian).FirstOrDefault(x => x.Id == pembelianId);
            return Task.FromResult(pembayaran.PembayaranPembelian.AsEnumerable());
        }


        public Task<bool> UpdatePembayaran(PembayaranPembelian model)
        {
            try
            {
                var pembelian = dbContext.Pembelian.Where(x => x.Id == model.Pembelian.Id)
                    .Include(x => x.Items)
                    .Include(x => x.OrderPembelian)
                    .Include(x => x.PembayaranPembelian).FirstOrDefault();

                if (pembelian == null || !pembelian.PembayaranPembelian.Any())
                    throw new SystemException("Data Pembelian atau Pembayaran Tidak Ditemukan");


                var totalInvoice = pembelian.Total - pembelian.TotalDiscount;
                var totalWithoutCurrentPayment = pembelian.PembayaranPembelian.Where(x => x.Id != model.Id).Sum(x => x.PayValue);

                if (totalWithoutCurrentPayment + model.PayValue > totalInvoice)
                    throw new SystemException("Maaf, Nilai Bayar Terlalu Besar !");

                var oldPembayaran = pembelian.PembayaranPembelian.Where(x => x.Id == model.Id).FirstOrDefault();
                if (oldPembayaran == null)
                    throw new SystemException("Pembayaran Tidak Ditemukan");

                if (totalWithoutCurrentPayment + model.PayValue < totalInvoice)
                {
                    pembelian.OrderPembelian.Status = OrderStatus.Diproses;
                    pembelian.Status = PaymentStatus.Panjar;
                }
                else
                {
                    pembelian.OrderPembelian.Status = OrderStatus.Selesai;
                    pembelian.Status = PaymentStatus.Lunas;
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