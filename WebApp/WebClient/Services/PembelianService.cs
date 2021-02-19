using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Ocph.DAL;
using ShareModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebClient.Services
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
        public async Task<Pembelian> CreatePembelian(int orderid)
        {
            var trans = dbContext.Database.BeginTransaction();
            try
            {
                var lastOrder = dbContext.Orderpembelian.Where(x => x.Id == orderid).Include(x => x.Supplier).Include(x => x.Items).FirstOrDefault();

                if (lastOrder == null)
                    throw new SystemException("Order Tidak Ditemukan !");

                var pembelian = new Pembelian { Discount=lastOrder.Discount, OrderPembelianId = orderid, PayDeadLine = DateTime.Now,     
                    CreatedDate = DateTime.Now, Items = new List<PembelianItem>() };

                foreach (var item in lastOrder.Items)
                {
                    var data = new PembelianItem
                    {
                        PembelianId = pembelian.Id,
                        Amount = item.Amount,
                        Price = item.Price,
                        Product = item.Product,
                        ProductId = item.ProductId,
                        UnitId = item.UnitId,
                        Unit = item.Unit
                    };

                    pembelian.Items.Add(data);

                }

                dbContext.Pembelian.Add(pembelian);

                await dbContext.SaveChangesAsync();


                trans.Commit();

                return pembelian;
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
      
        public async Task<Pembelian> UpdatePembelian(int pembelianId, Pembelian order)
        {
            var trans = dbContext.Database.BeginTransaction();
            try
            {

                var lastOrder = await GetPembelian(pembelianId);

                if (lastOrder == null)
                    throw new SystemException("Pembelian Not Found  !");

                dbContext.Entry(lastOrder).CurrentValues.SetValues(order);

                foreach (var item in order.Items)
                {
                    if (item.Id <= 0)
                    {
                        item.PembelianId = order.Id;
                        dbContext.PembelianItem.Add(item);
                    }
                    else
                    {
                        var oldItem = dbContext.PembelianItem.SingleOrDefault(x => x.Id == item.Id);
                        if(oldItem==null)
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

                await dbContext.SaveChangesAsync();
                trans.Commit();
                return order;
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
                var pembelians =dbContext.Pembelian.Where(x => x.Id == id)
                                   .Include(x => x.Items).ThenInclude(x => x.Product).ThenInclude(x => x.Units)
                                   .Include(x => x.OrderPembelian).ThenInclude(x => x.Supplier);
                return Task.FromResult(pembelians.FirstOrDefault());
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new SystemException(ex.Message);
            }
        }                               

        public Task<IEnumerable<Pembelian>> GetPembelians()
        {
            try
            {
                var pembelians = dbContext.Pembelian
                                   .Include(x => x.Items).ThenInclude(x => x.Product).ThenInclude(x => x.Units)
                                   .Include(x => x.OrderPembelian).ThenInclude(x => x.Supplier).AsNoTracking();
                return Task.FromResult(pembelians.AsEnumerable());
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new SystemException(ex.Message);
            }
        }

        public Task<IEnumerable<Pembelian>> GetPembeliansBySupplierId(int id)
        {
             var pembelians = dbContext.Pembelian
                                   .Include(x => x.Items).ThenInclude(x => x.Product).ThenInclude(x => x.Units)
                                   .Include(x => x.OrderPembelian).ThenInclude(x => x.Supplier).AsNoTracking(); 
                return Task.FromResult(pembelians.Where(x=>x.OrderPembelian.SupplierId==id).AsEnumerable());
        }


        public async Task<bool> DeletePembelian(int id)
        {
            try
            {
                var oldData  = dbContext.Pembelian.SingleOrDefault(x => x.Id == id);
                if (oldData==null)
                    throw new SystemException("Pembelian Not Found !");

                dbContext.Pembelian.Remove(oldData);

                await dbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new SystemException(ex.Message);
            }
        }
        #endregion

        #region Orders
        public async Task<Orderpembelian> CreateOrder(Orderpembelian order)
        {
            try
            {

                if (order.Supplier != null)
                    dbContext.Entry(order.Supplier).State = EntityState.Unchanged;
                foreach (var item in order.Items)
                {
                    dbContext.Entry(item.Product).State = EntityState.Unchanged;
                    dbContext.Entry(item.Unit).State = EntityState.Unchanged;
                }
                dbContext.Orderpembelian.Add(order);
                await dbContext.SaveChangesAsync();
                return order;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new SystemException(ex.Message);
            }
        }

        public async Task<bool> DeleteOrder(int id)
        {
            try
            {
                var deleted = dbContext.Orderpembelian.SingleOrDefault(x => x.Id == id);
                if (deleted==null)
                    throw new SystemException("Order Not Found !");
                dbContext.Orderpembelian.Remove(deleted);
                await dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new SystemException(ex.Message);
            }
        }

        public Task<Orderpembelian> GetOrder(int id)
        {
            var lastOrder = dbContext.Orderpembelian.Where(x => x.Id == id)
                            .Include(x => x.Supplier)
                            .Include(x => x.Items).ThenInclude(x => x.Product).ThenInclude(x => x.Units);             
            return Task.FromResult(lastOrder.FirstOrDefault());
        }

        public Task<IEnumerable<Orderpembelian>> GetOrders()
        {
            var lastOrder = dbContext.Orderpembelian
                           .Include(x => x.Supplier)
                           .Include(x => x.Items).ThenInclude(x => x.Product).ThenInclude(x => x.Units).AsNoTracking();
            return Task.FromResult(lastOrder.AsEnumerable());
        }

        public Task<IEnumerable<Orderpembelian>> GetOrdersBySupplierId(int supplierId)
        {
            var lastOrder = dbContext.Orderpembelian.Where(x=>x.SupplierId==supplierId)
                            .Include(x => x.Supplier)
                            .Include(x => x.Items).ThenInclude(x => x.Product).ThenInclude(x => x.Units).AsNoTracking();
            return Task.FromResult(lastOrder.AsEnumerable());
        }



        public async Task<Orderpembelian> UpdateOrder(int id, Orderpembelian order)
        {
            try
            {


                var lastOrder = dbContext.Orderpembelian.Where(x=>x.Id==id).Include(x => x.Items).AsNoTracking().FirstOrDefault();


                if (lastOrder == null)
                    throw new SystemException("Order Not Found  !");


                lastOrder.Discount = order.Discount;
                lastOrder.OrderDate = order.OrderDate;
                lastOrder.Status = order.Status;
                lastOrder.SupplierId = order.SupplierId;
                foreach (var item in order.Items)
                {
                    if (item.Id <= 0)
                    {
                        dbContext.Entry(item.Unit).State = EntityState.Unchanged;
                        dbContext.Entry(item.Product).State = EntityState.Unchanged;
                        dbContext.OrderpembelianItem.Add(item);
                    }
                    else
                    {
                       var updatedItem = dbContext.OrderpembelianItem.AsNoTracking().SingleOrDefault(x => x.Id == item.Id);
                        if (updatedItem==null)
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
                            dbContext.OrderpembelianItem.Remove(existsDb);
                        }
                    }
                }

                

                await dbContext.SaveChangesAsync();
                return order;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new SystemException(ex.Message);
            }
        }


        #endregion


        #region Pembayaran
        public async Task<Pembayaranpembelian> CreatePembayaran(int pembelianId, Pembayaranpembelian pembayaran, bool forced)
        {
            var trans = dbContext.Database.BeginTransaction();
            try
            {
                var pembelian = dbContext.Pembelian.Where(x => x.Id == pembelianId)
                    .Include(x => x.Items)
                    .Include(x => x.OrderPembelian)
                    .Include(x => x.Pembayaranpembelian).AsNoTracking().FirstOrDefault();



                if (pembelian == null)
                    throw new SystemException("Pembelian Tidak Ditemukan");

                var totalInvoice = pembelian.Items.Sum(x => x.Total) - (pembelian.Items.Sum(x => x.Total)*(pembelian.Discount/100)) ;
                double totalBayar = 0;
                if (pembelian.Pembayaranpembelian != null)
                {
                   totalBayar= pembelian.Pembayaranpembelian.Sum(x => x.PayValue);
                }

                var sisa = totalInvoice - totalBayar - pembayaran.PayValue;

                if (sisa < 0 && !forced)
                    throw new SystemException("Pembayaran Anda Melebihi Tagihan Invoice !");


                var status = sisa > 0 ? PaymentStatus.DownPayment:  PaymentStatus.PaidOff;

                pembelian.Status = status;
                pembelian.OrderPembelian.Status = OrderStatus.Complete;
               
                pembelian.Pembayaranpembelian.Add(pembayaran);

                var result = await dbContext.SaveChangesAsync();
                
                if (result <=0)
                    throw new SystemException("Pembayaran Gagal !");
                
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

        public Task<IEnumerable<Pembayaranpembelian>> GetPembayaran(int pembelianId)
        {
            var pembayarans = dbContext.Pembayaranpembelian.Where(x => x.PembelianId == pembelianId).AsNoTracking();
            return Task.FromResult(pembayarans.AsEnumerable());
        }
        #endregion


    }
}