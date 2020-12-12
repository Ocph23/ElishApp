using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using Ocph.DAL;
using Ocph.DAL.Mapping.MySql;
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
        private readonly OcphDbContext dbContext;
     //   private readonly IHttpContextAccessor auth;
        private readonly ILogger<PenjualanService> _logger;

        public PenjualanService(OcphDbContext db, ILogger<PenjualanService> log)
        {
            dbContext = db;
       //     auth = httpContextAccessor;
            _logger = log;
        }


        #region penjualan
        public Task<Penjualan> CreatePenjualan(int orderid)
        {
            var trans = dbContext.BeginTransaction();
            try
            {
                var lastOrder = (from a in dbContext.OrderPenjualans.Where(x => x.Id == orderid)
                                 join b in dbContext.OrderPenjualanItems.Where(x => x.OrderPenjualanId == orderid) on a.Id equals b.OrderPenjualanId
                                 into itemGroup
                                 from b in itemGroup.DefaultIfEmpty()
                                 select new Orderpenjualan
                                 {
                                     Discount = a.Discount,
                                     OrderDate = a.OrderDate,
                                     CustomerId = a.CustomerId,
                                     Id = a.Id,
                                     SalesId = a.SalesId,
                                     Items = itemGroup.ToList()
                                 }).FirstOrDefault();

                if (lastOrder == null)
                    throw new SystemException("Order Tidak Ditemukan !");


                var penjualan = new Penjualan
                {
                    OrderPenjualanId = orderid,
                    PayDeadLine = DateTime.Now,
                    Discount = lastOrder.Discount,
                    CreateDate = DateTime.Now,
                    Items = new List<Penjualanitem>()
                };

                penjualan.Id = dbContext.Penjualans.InsertAndGetLastID(penjualan);


                 if(lastOrder.Status == OrderStatus.New)
                {
                    var updatedOrder = dbContext.OrderPenjualans.Update(x => new { x.Status }, new Orderpenjualan { Status = OrderStatus.Proccess }, x => x.Id == lastOrder.Id);
                    if (!updatedOrder)
                        throw new SystemException("Data Tidak Berhasil Disimpan");
                }

                foreach (var item in lastOrder.Items)
                {
                    var data = new Penjualanitem
                    {
                        PenjualanId = penjualan.Id,
                        Amount = item.Amount,

                        Price = item.Price,
                        Product = item.Product,
                        ProductId = item.ProductId,
                        UnitId = item.UnitId,
                        Unit = item.Unit
                    };


                    data.Id = dbContext.PenjualanItems.InsertAndGetLastID(data);

                    if (data.Id <= 0)
                        throw new SystemException("Item Pembelian Not Saved !");
                    penjualan.Items.Add(data);

                }

                trans.Commit();

                return Task.FromResult(penjualan);
            }
            catch (Exception ex)
            {

                trans.Rollback();
                throw new SystemException(ex.Message);
            }
        }
        public Task<Penjualan> UpdatePenjualan(int penjualanId, Penjualan order)
        {
            var trans = dbContext.BeginTransaction();
            try
            {
                var lastOrder = (from a in dbContext.Penjualans.Where(x => x.Id == penjualanId)
                                 join b in dbContext.PenjualanItems.Where(x => x.PenjualanId == penjualanId) on a.Id equals b.PenjualanId
                                 into itemGroup
                                 from b in itemGroup.DefaultIfEmpty()
                                 select new Penjualan
                                 {

                                     CreateDate = a.CreateDate,
                                     Discount = a.Discount,
                                     OrderPenjualanId = a.OrderPenjualanId,
                                     Payment = a.Payment,
                                     PayDeadLine = a.PayDeadLine,
                                     Id = a.Id,
                                     Items = itemGroup.ToList()
                                 }).FirstOrDefault();


                if (lastOrder == null)
                    throw new SystemException("Penjualan Not Found  !");


                order.Activity = order.Activity == ActivityStatus.None ? ActivityStatus.Created :order.Activity;

                var updated = dbContext.Penjualans.Update(x => new {x.Payment, x.Discount, x.CreateDate, x.PayDeadLine,x.Activity }, order, x => x.Id == order.Id);

                if (!updated)
                    throw new SystemException("Penjualan Not Updated !");

                foreach (var item in order.Items)
                {
                    if (item.Id <= 0)
                    {
                        item.PenjualanId = order.Id;
                        item.Id = dbContext.PenjualanItems.InsertAndGetLastID(item);
                        if (item.Id <= 0)
                            throw new SystemException("Penjualan Item Not Added !");
                    }
                    else
                    {
                        updated = dbContext.PenjualanItems.Update(x => new { x.Amount, x.Price, x.UnitId }, item, x => x.Id == item.Id);
                        if (!updated)
                            throw new SystemException("Penjualan Item Not Updated !");
                    }
                }


                //remove

                if (order.Items.Count != lastOrder.Items.Count)
                {
                    foreach (var item in lastOrder.Items)
                    {
                        var existsDb = order.Items.Where(x => x.Id == item.Id).FirstOrDefault();
                        if (existsDb == null)
                        {
                            if (!dbContext.PenjualanItems.Delete(x => x.Id == item.Id))
                                throw new SystemException("Order Item Not Removed !");
                        }
                    }
                }

                trans.Commit();
                return Task.FromResult(order);
            }
            catch (Exception ex)
            {
                trans.Rollback();
                throw new SystemException(ex.Message);
            }
        }

        public Task<IEnumerable<Penjualan>> GetPenjualans()
        {
            var orders = (from a in dbContext.Penjualans.Select()
                          join o in dbContext.OrderPenjualans.Select() on a.OrderPenjualanId equals o.Id
                          join c in dbContext.Customers.Select() on o.CustomerId equals c.Id
                          join s in dbContext.Karyawans.Select() on o.SalesId equals s.Id into ss
                          from sa in ss.DefaultIfEmpty()
                          join b in dbContext.PenjualanItems.Select() on a.Id equals b.PenjualanId
                          into itemGroup
                          select new Penjualan
                          {
                              OrderPenjualan = o,
                              Status = a.Status,
                              Customer = c,
                              Sales = ss != null ? sa : null,
                              CreateDate = a.CreateDate,
                              OrderPenjualanId = a.OrderPenjualanId,
                              PayDeadLine = a.PayDeadLine,
                              Discount = a.Discount,
                              Payment = a.Payment,
                              Id = a.Id,
                              Items = (from i in itemGroup
                                       join p in dbContext.Products.Select() on i.ProductId equals p.Id
                                       join uss in dbContext.Units.Select() on p.Id equals uss.ProductId into units
                                       select new Penjualanitem
                                       {
                                           Id = i.Id,
                                           PenjualanId = i.PenjualanId,
                                           ProductId = i.ProductId,
                                           Product = new Product
                                           {
                                               CategoryId = p.CategoryId,
                                               CodeArticle = p.CodeArticle,
                                               CodeName = p.CodeName,
                                               Description = p.Description,
                                               Id = p.Id,
                                               Merk = p.Merk,
                                               Name = p.Name,
                                               Size = p.Size,
                                               Units = (from uu in units select uu).ToList()
                                           },
                                           Amount = i.Amount,
                                           UnitId = i.UnitId,
                                           Unit = units.Where(x => x.Id == i.UnitId).FirstOrDefault(),
                                           Price = i.Price


                                       }).ToList()
                          });

            return Task.FromResult(orders.AsEnumerable());
        }
        public Task<Penjualan> GetPenjualan(int id)
        {
            var orders = (from a in dbContext.Penjualans.Where(x => x.Id == id)
                          join o in dbContext.OrderPenjualans.Select() on a.OrderPenjualanId equals o.Id
                          join c in dbContext.Customers.Select() on o.CustomerId equals c.Id
                          join s in dbContext.Karyawans.Select() on o.SalesId equals s.Id into ss
                          from sa in ss.DefaultIfEmpty()
                          join b in dbContext.PenjualanItems.Select() on a.Id equals b.PenjualanId
                          into itemGroup
                          from b in itemGroup.DefaultIfEmpty()
                          select new Penjualan
                          {
                              OrderPenjualan = o,
                              Status = a.Status,
                              Customer = c,
                              Sales = ss != null ? sa : null,
                              CreateDate = a.CreateDate,
                              OrderPenjualanId = a.OrderPenjualanId,
                              PayDeadLine = a.PayDeadLine,
                              Discount = a.Discount,
                              Payment = a.Payment,
                              Id = a.Id,
                              Items = (from i in itemGroup
                                       join p in dbContext.Products.Select() on i.ProductId equals p.Id
                                       join uss in dbContext.Units.Select() on p.Id equals uss.ProductId into units
                                       select new Penjualanitem
                                       {
                                           Id = i.Id,
                                           PenjualanId = i.PenjualanId,
                                           ProductId = i.ProductId,
                                           Product = new Product
                                           {
                                               CategoryId = p.CategoryId,
                                               CodeArticle = p.CodeArticle,
                                               CodeName = p.CodeName,
                                               Description = p.Description,
                                               Id = p.Id,
                                               Merk = p.Merk,
                                               Name = p.Name,
                                               Size = p.Size,
                                               Units = units.ToList()
                                           },
                                           Amount = i.Amount,
                                           UnitId = i.UnitId,
                                           Unit = units.Where(x => x.Id == i.UnitId).FirstOrDefault(),
                                           Price = i.Price


                                       }).ToList()
                          });

            return Task.FromResult(orders.FirstOrDefault());
        }

        public Task<IEnumerable<Penjualan>> GetPenjualansBySalesId(int id)
        {
            var orders = (from o in dbContext.OrderPenjualans.Where(x => x.SalesId == id)
                          join a in dbContext.Penjualans.Select() on o.Id equals a.OrderPenjualanId
                          join b in dbContext.PenjualanItems.Select() on a.Id equals b.PenjualanId
                          into itemGroup
                          from b in itemGroup.DefaultIfEmpty()
                          select new Penjualan
                          {
                              CreateDate = a.CreateDate,
                              OrderPenjualanId = a.OrderPenjualanId,
                              PayDeadLine = a.PayDeadLine,
                              Id = a.Id,
                              Items = itemGroup.ToList()
                          });

            return Task.FromResult(orders.AsEnumerable());
        }

        public Task<IEnumerable<Penjualan>> GetPenjualansByCustomerId(int id)
        {
            var orders = (from o in dbContext.OrderPenjualans.Where(x => x.CustomerId == id)
                          join a in dbContext.Penjualans.Select() on o.Id equals a.OrderPenjualanId
                          join b in dbContext.PenjualanItems.Select() on a.Id equals b.PenjualanId
                          into itemGroup
                          from b in itemGroup.DefaultIfEmpty()
                          select new Penjualan
                          {
                              CreateDate = a.CreateDate,
                              OrderPenjualanId = a.OrderPenjualanId,
                              PayDeadLine = a.PayDeadLine,
                              Id = a.Id,
                              Items = itemGroup.ToList()
                          });

            return Task.FromResult(orders.AsEnumerable());
        }


        public Task<bool> DeletePenjualan(int id)
        {
            try
            {
                var deleted = dbContext.Penjualans.Delete(x => x.Id == id);
                if (!deleted)
                    throw new SystemException("Penjualan Not Deleted !");
                return Task.FromResult(deleted);
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public Task<IEnumerable<PenjualanViewModel>> GetPenjualans(DateTime startDate, DateTime endDate)
        {
            try
            {
                var command = dbContext.CreateCommand();
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.CommandText = "GetPenjualanByDate";

                command.Parameters.Add(new MySqlParameter("@startDate", startDate));
                command.Parameters.Add(new MySqlParameter("@endDate", endDate));

                var reader = command.ExecuteReader();
                var list = new MappingColumn(new EntityInfo(typeof(PenjualanViewModel))).MappingWithoutInclud<PenjualanViewModel>(reader);
                reader.Close();

                var result = from a in list select a;
                return Task.FromResult(result);
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
            var trans = dbContext.BeginTransaction();

            try
            {

                order.Id = dbContext.OrderPenjualans.InsertAndGetLastID(order);

                if (order.Id <= 0)
                    throw new SystemException("Order Not Created !");

                foreach (var item in order.Items)
                {
                    item.OrderPenjualanId = order.Id;
                    item.Id = dbContext.OrderPenjualanItems.InsertAndGetLastID(item);
                    if (item.Id <= 0)
                        throw new SystemException("Order Item Not Added !");
                }

                trans.Commit();
                return Task.FromResult(order);
            }
            catch (Exception ex)
            {
                trans.Rollback();
                throw new SystemException(ex.Message);
            }
        }

        public Task<bool> DeleteOrder(int id)
        {
            try
            {
                var deleted = dbContext.OrderPenjualans.Delete(x => x.Id == id);
                if (!deleted)
                    throw new SystemException("Order Not Deleted !");
                return Task.FromResult(deleted);
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public Task<Orderpenjualan> GetOrder(int id)
        {
            var lastOrder = (from a in dbContext.OrderPenjualans.Where(x => x.Id == id)
                             join c in dbContext.Customers.Select() on a.CustomerId equals c.Id
                             join s in dbContext.Karyawans.Select() on a.SalesId equals s.Id
                             join b in dbContext.OrderPenjualanItems.Where(x => x.OrderPenjualanId == id) on a.Id equals b.OrderPenjualanId
                             into itemGroup
                             select new Orderpenjualan
                             {                                     
                                 Customer=c, 
                                 Sales=s ,
                                 Status = a.Status,
                                 Discount = a.Discount,
                                 OrderDate = a.OrderDate,
                                 CustomerId = a.CustomerId,
                                 SalesId = a.SalesId,
                                 Id = a.Id,
                                 Items = (from i in itemGroup
                                          join p in dbContext.Products.Select() on i.ProductId equals p.Id
                                          join uss in dbContext.Units.Select() on p.Id equals uss.ProductId into units
                                          select new OrderPenjualanItem
                                          {
                                              Id = i.Id,
                                              OrderPenjualanId = i.OrderPenjualanId,
                                              ProductId = i.ProductId,
                                              Product = new Product
                                              {
                                                  CategoryId = p.CategoryId,
                                                  CodeArticle = p.CodeArticle,
                                                  CodeName = p.CodeName,
                                                  Description = p.Description,
                                                  Id = p.Id,
                                                  Merk = p.Merk,
                                                  Name = p.Name,
                                                  Size = p.Size,
                                                  Units = (from uu in units select uu).ToList()
                                              },
                                              Amount = i.Amount,
                                              UnitId = i.UnitId,
                                              Unit = units.Where(x => x.Id == i.UnitId).FirstOrDefault(),
                                              Price = i.Price


                                          }).ToList()
                             }).FirstOrDefault();
            return Task.FromResult(lastOrder);
        }

        public Task<IEnumerable<Orderpenjualan>> GetOrders()
        {
            var orders = (from a in dbContext.OrderPenjualans.Select()
                          join c in dbContext.Customers.Select() on a.CustomerId equals c.Id
                          join s in dbContext.Karyawans.Select() on a.SalesId equals s.Id into ss
                          from sa in ss.DefaultIfEmpty()
                          join b in dbContext.OrderPenjualanItems.Select() on a.Id equals b.OrderPenjualanId
                          into itemGroup
                          select new Orderpenjualan
                          {
                              Customer = c,
                              Status = a.Status,
                              Sales = sa ?? null,
                              Discount = a.Discount,
                              OrderDate = a.OrderDate,
                              CustomerId = a.CustomerId,
                              SalesId = a.SalesId,
                              Id = a.Id,
                              Items = (from ig in itemGroup select ig).ToList()
                          });
            return Task.FromResult(orders.AsEnumerable());
        }

        public Task<IEnumerable<Orderpenjualan>> GetOrdersByCustomerId(int customerId)
        {
            var orders = (from a in dbContext.OrderPenjualans.Where(x => x.CustomerId == customerId)
                          join b in dbContext.OrderPenjualanItems.Select() on a.Id equals b.OrderPenjualanId
                          into itemGroup
                          from b in itemGroup.DefaultIfEmpty()
                          select new Orderpenjualan
                          {
                              Discount = a.Discount,
                              OrderDate = a.OrderDate,
                              CustomerId = a.CustomerId,
                              SalesId = a.SalesId,
                              Id = a.Id,
                              Items = itemGroup.ToList()
                          });
            return Task.FromResult(orders.AsEnumerable());
        }
        public Task<IEnumerable<Orderpenjualan>> GetOrdersBySalesId(int salesId)
        {
            var orders = (from a in dbContext.OrderPenjualans.Where(x => x.SalesId == salesId)
                          join b in dbContext.OrderPenjualanItems.Select() on a.Id equals b.OrderPenjualanId
                          into itemGroup
                          from b in itemGroup.DefaultIfEmpty()
                          select new Orderpenjualan
                          {
                              Discount = a.Discount,
                              OrderDate = a.OrderDate,
                              CustomerId = a.CustomerId,
                              SalesId = a.SalesId,
                              Id = a.Id,
                              Items = itemGroup.ToList()
                          });
            return Task.FromResult(orders.AsEnumerable());
        }

        public Task<Orderpenjualan> UpdateOrder(int id, Orderpenjualan order)
        {
            var trans = dbContext.BeginTransaction();
            try
            {
                var lastOrder = (from a in dbContext.OrderPenjualans.Where(x => x.Id == id)
                                 join b in dbContext.OrderPenjualanItems.Where(x => x.OrderPenjualanId == id) on a.Id equals b.OrderPenjualanId
                                 into itemGroup
                                 from b in itemGroup.DefaultIfEmpty()
                                 select new Orderpenjualan
                                 {
                                     Discount = a.Discount,
                                     OrderDate = a.OrderDate,
                                     CustomerId = a.CustomerId,
                                     Id = a.Id,
                                     SalesId = a.SalesId,
                                     Items = itemGroup.ToList()
                                 }).FirstOrDefault();


                if (lastOrder == null)
                    throw new SystemException("Order Not Found  !");

                var updated = dbContext.OrderPenjualans.Update(x => new { x.Discount, x.OrderDate, x.SalesId }, order, x => x.Id == order.Id);

                if (!updated)
                    throw new SystemException("Order Not Updated !");

                foreach (var item in order.Items)
                {
                    if (item.Id <= 0)
                    {
                        item.OrderPenjualanId = order.Id;
                        item.Id = dbContext.OrderPenjualanItems.InsertAndGetLastID(item);
                        if (item.Id <= 0)
                            throw new SystemException("Order Item Not Added !");
                    }
                    else
                    {
                        updated = dbContext.OrderPenjualanItems.Update(x => new { x.Amount, x.Price, x.UnitId }, item, x => x.Id == item.Id);
                        if (!updated)
                            throw new SystemException("Order Item Not Updated !");
                    }
                }


                //remove

                if (order.Items.Count != lastOrder.Items.Count)
                {
                    foreach (var item in lastOrder.Items)
                    {
                        var existsDb = order.Items.Where(x => x.Id == item.Id).FirstOrDefault();
                        if (existsDb == null)
                        {
                            if (!dbContext.OrderPembelianItems.Delete(x => x.Id == item.Id))
                                throw new SystemException("Order Item Not Removed !");
                        }
                    }
                }

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
        public Task<Pembayaranpenjualan> CreatePembayaran(int penjualanId, Pembayaranpenjualan pembayaran, bool forced)
        {
            var trans = dbContext.BeginTransaction();
            try
            {
                var penjualan = (from a in dbContext.Penjualans.Where(x => x.Id == penjualanId)
                                 join b in dbContext.PenjualanItems.Where(x => x.PenjualanId == penjualanId)
                                 on a.Id equals b.PenjualanId into gg
                                 select new Penjualan
                                 {
                                     Id = a.Id,
                                     OrderPenjualanId = a.OrderPenjualanId,
                                     Discount = a.Discount,
                                     Items = (from c in gg select c).ToList()
                                 }).FirstOrDefault();



                if (penjualan == null)
                    throw new SystemException("Pembelian Tidak Ditemukan");

                var pembayarans = dbContext.PembayaranPenjualans.Where(x => x.PenjualanId == penjualanId).ToList();

                var totalInvoice = penjualan.Items.Sum(x => x.Total) - (penjualan.Items.Sum(x => x.Total) * (penjualan.Discount / 100));
                double totalBayar = 0;
                if (pembayaran != null)
                {
                    totalBayar = pembayarans.Sum(x => x.PayValue);

                }

                var sisa = totalInvoice - totalBayar - pembayaran.PayValue;

                if (sisa < 0 && !forced)
                    throw new SystemException($"Pembayaran Melebihi Tagihan Invoice ! sisa {sisa}");


                var status = sisa > 0 ? PaymentStatus.DownPayment : PaymentStatus.PaidOff;
                var updatePenjualan = dbContext.Penjualans.Update(x => new { x.Status }, new Penjualan { Status = status }, x => x.Id == penjualanId);
                var updateOrder = dbContext.OrderPenjualans.Update(x => new { x.Status },
                    new Orderpenjualan { Status = OrderStatus.Complete }, x => x.Id == penjualan.OrderPenjualanId);

                if (!updatePenjualan || !updateOrder)
                    throw new SystemException("Pembayaran Gagal !");

                var result = dbContext.PembayaranPenjualans.InsertAndGetLastID(pembayaran);
                if (result <= 0)
                    throw new SystemException("Pembayaran Gagal !");


                trans.Commit();
                pembayaran.Id = result;
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
            var pembayarans = dbContext.PembayaranPenjualans.Where(x => x.PenjualanId == penjualanId);
            return Task.FromResult(pembayarans.AsEnumerable());
        }

      
        #endregion

    }
}
