using Microsoft.AspNetCore.Http;
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
        private readonly OcphDbContext dbContext;
    //    private readonly IHttpContextAccessor auth;
        private readonly ILogger _logger;

        public PembelianService(ILogger<PembelianService> logger, OcphDbContext db)
        {
            dbContext = db;
       //     auth = httpContextAccessor;
            _logger = logger;
        }

        #region Pembelian
        public Task<Pembelian> CreatePembelian(int orderid)
        {
            var trans = dbContext.BeginTransaction();
            try
            {
                var lastOrder = (from a in dbContext.OrderPembelians.Where(x => x.Id == orderid)
                                 join s in dbContext.Suppliers.Select() on a.SupplierId equals s.Id
                                 join b in dbContext.OrderPembelianItems.Where(x => x.OrderPembelianId == orderid) on a.Id equals b.OrderPembelianId
                                 into itemGroup
                                 from b in itemGroup.DefaultIfEmpty()
                                 select new Orderpembelian
                                 {
                                     Supplier = s,
                                     Discount = a.Discount,
                                     OrderDate = a.OrderDate,
                                     SupplierId = a.SupplierId,
                                     Id = a.Id,
                                     Items = itemGroup.ToList()
                                 }).FirstOrDefault();

                if (lastOrder == null)
                    throw new SystemException("Order Tidak Ditemukan !");

                var pembelian = new Pembelian { Discount=lastOrder.Discount, OrderPembelianId = orderid, PayDeadLine = DateTime.Now,     
                    CreatedDate = DateTime.Now, Items = new List<PembelianItem>() };

                pembelian.Id = dbContext.Pembelians.InsertAndGetLastID(pembelian);

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


                    data.Id = dbContext.PembelianItems.InsertAndGetLastID(data);

                    if (data.Id <= 0)
                        throw new SystemException("Item Pembelian Not Saved !");
                    pembelian.Items.Add(data);

                }

                trans.Commit();

                return Task.FromResult(pembelian);
            }
            catch (Exception ex)
            {

                try
                {
                    _logger.LogError(ex.Message);
                    trans.Rollback();
                }
                catch (System.Exception exx)
                {
                    throw new SystemException(exx.Message);
                }
                throw new SystemException(ex.Message);
            }
        }
      
        public Task<Pembelian> UpdatePembelian(int pembelianId, Pembelian order)
        {
            var trans = dbContext.BeginTransaction();
            try
            {
                var lastOrder = (from a in dbContext.Pembelians.Where(x => x.Id == pembelianId)
                                 join b in dbContext.PembelianItems.Where(x => x.PembelianId == pembelianId) on a.Id equals b.PembelianId
                                 into itemGroup
                                 from b in itemGroup.DefaultIfEmpty()
                                 select new Pembelian
                                 {                
                                     Discount=a.Discount, InvoiceNumber=a.InvoiceNumber,  OrderPembelianId=a.OrderPembelianId, 
                                     CreatedDate = a.CreatedDate,
                                     PayDeadLine = a.PayDeadLine,
                                     Id = a.Id,
                                     Items = itemGroup.ToList()
                                 }).FirstOrDefault();


                if (lastOrder == null)
                    throw new SystemException("Pembelian Not Found  !");

                var updated = dbContext.Pembelians.Update(x => new { x.InvoiceNumber,  x.CreatedDate, x.PayDeadLine, x.Discount, }, order, x => x.Id == order.Id);

                if (!updated)
                    throw new SystemException("Pembelian Not Updated !");

                foreach (var item in order.Items)
                {
                    if (item.Id <= 0)
                    {
                        item.PembelianId = order.Id;
                        item.Id = dbContext.PembelianItems.InsertAndGetLastID(item);
                        if (item.Id <= 0)
                            throw new SystemException("Pembelian Item Not Added !");
                    }
                    else
                    {
                        updated = dbContext.PembelianItems.Update(x => new { x.Amount, x.Price, x.UnitId }, item, x => x.Id == item.Id);
                        if (!updated)
                            throw new SystemException("Pembelian Item Not Updated !");
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
                _logger.LogError(ex.Message);
                trans.Rollback();
                throw new SystemException(ex.Message);
            }
        }

        public Task<Pembelian> GetPembelian(int id)
        {
            try
            {
                var pembelians = (from a in dbContext.Pembelians.Where(x => x.Id == id)
                                  join o in dbContext.OrderPembelians.Select() on a.OrderPembelianId equals o.Id
                                  join s in dbContext.Suppliers.Select() on o.SupplierId equals s.Id
                                  join b in dbContext.PembelianItems.Select() on a.Id equals b.PembelianId
                                  into itemGroup
                                  from b in itemGroup.DefaultIfEmpty()
                                  select new Pembelian
                                  {
                                      Supplier = s,
                                      InvoiceNumber = a.InvoiceNumber,
                                      Discount = a.Discount,
                                      Status = a.Status,
                                      OrderPembelian = new Orderpembelian { Id = o.Id, Supplier = s, OrderDate = o.OrderDate, Discount = o.Discount },
                                      CreatedDate = a.CreatedDate,
                                      OrderPembelianId = a.OrderPembelianId,
                                      PayDeadLine = a.PayDeadLine,
                                      Id = a.Id,
                                      Items = (from i in itemGroup
                                               join p in dbContext.Products.Select() on i.ProductId equals p.Id
                                               join uss in dbContext.Units.Select() on p.Id equals uss.ProductId into units
                                               select new PembelianItem
                                               {
                                                   Id = i.Id,
                                                   PembelianId = i.PembelianId,
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
                var pembelians = (from a in dbContext.Pembelians.Select()
                                  join o in dbContext.OrderPembelians.Select() on a.OrderPembelianId equals o.Id
                                  join s in dbContext.Suppliers.Select() on o.SupplierId equals s.Id
                                  join b in dbContext.PembelianItems.Select() on a.Id equals b.PembelianId into itemGroup
                                  select new Pembelian
                                  {
                                      Supplier = s, 
                                      InvoiceNumber=a.InvoiceNumber,
                                      Discount = a.Discount,
                                      Status = a.Status,
                                      OrderPembelian = new Orderpembelian { Id = o.Id, Supplier = s, OrderDate = o.OrderDate, Discount = o.Discount },
                                      CreatedDate = a.CreatedDate,
                                      OrderPembelianId = a.OrderPembelianId,
                                      PayDeadLine = a.PayDeadLine,
                                      Id = a.Id,
                                      Items = (from i in itemGroup
                                               join p in dbContext.Products.Select() on i.ProductId equals p.Id
                                               join uss in dbContext.Units.Select() on p.Id equals uss.ProductId into units
                                               select new PembelianItem
                                               {
                                                   Id = i.Id,
                                                   PembelianId = i.PembelianId,
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
            var orders = (from o in dbContext.OrderPembelians.Where(x => x.SupplierId == id)
                          from a in dbContext.Pembelians.Select()
                          join s in dbContext.Suppliers.Select() on o.SupplierId equals s.Id
                          join b in dbContext.PembelianItems.Select() on a.Id equals b.PembelianId
                          into itemGroup
                          from b in itemGroup.DefaultIfEmpty()
                          select new Pembelian
                          {
                              Supplier = s,             
                              InvoiceNumber=a.InvoiceNumber,
                              Discount = a.Discount,
                              Status = a.Status,
                              OrderPembelian = new Orderpembelian { Id = o.Id, Supplier = s, OrderDate = o.OrderDate, Discount = o.Discount },
                              CreatedDate = a.CreatedDate,
                              OrderPembelianId = a.OrderPembelianId,
                              PayDeadLine = a.PayDeadLine,
                              Id = a.Id,
                              Items = (from i in itemGroup
                                       join p in dbContext.Products.Select() on i.ProductId equals p.Id
                                       join uss in dbContext.Units.Select() on p.Id equals uss.ProductId into units
                                       select new PembelianItem
                                       {
                                           Id = i.Id,
                                           PembelianId = i.PembelianId,
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
            return Task.FromResult(orders.AsEnumerable());
        }


        public Task<bool> DeletePembelian(int id)
        {
            try
            {
                var deleted = dbContext.Pembelians.Delete(x => x.Id == id);
                if (!deleted)
                    throw new SystemException("Pembelian Not Deleted !");
                return Task.FromResult(deleted);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new SystemException(ex.Message);
            }
        }
        #endregion

        #region Orders
        public Task<Orderpembelian> CreateOrder(Orderpembelian order)
        {
            var trans = dbContext.BeginTransaction();

            try
            {

                order.Id = dbContext.OrderPembelians.InsertAndGetLastID(order);

                if (order.Id <= 0)
                    throw new SystemException("Order Not Created !");

                foreach (var item in order.Items)
                {
                    item.OrderPembelianId = order.Id;
                    item.Id = dbContext.OrderPembelianItems.InsertAndGetLastID(item);
                    if (item.Id <= 0)
                        throw new SystemException("Order Item Not Added !");
                }

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

        public Task<bool> DeleteOrder(int id)
        {
            try
            {
                var deleted = dbContext.OrderPembelians.Delete(x => x.Id == id);
                if (!deleted)
                    throw new SystemException("Order Not Deleted !");
                return Task.FromResult(deleted);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new SystemException(ex.Message);
            }
        }

        public Task<Orderpembelian> GetOrder(int id)
        {
            var lastOrder = (from a in dbContext.OrderPembelians.Where(x => x.Id == id)
                             join s in dbContext.Suppliers.Select() on a.SupplierId equals s.Id
                             join b in dbContext.OrderPembelianItems.Where(x => x.OrderPembelianId == id)
                             on a.Id equals b.OrderPembelianId into itemGroup
                             select new Orderpembelian
                             {
                                 Supplier = s,
                                 Discount = a.Discount,
                                 OrderDate = a.OrderDate,
                                 SupplierId = a.SupplierId,
                                 Status = a.Status,
                                 Id = a.Id,
                                 Items = (from i in itemGroup
                                          join p in dbContext.Products.Select() on i.ProductId equals p.Id
                                          join uss in dbContext.Units.Select() on p.Id equals uss.ProductId into units
                                          select new OrderpembelianItem
                                          {
                                              Id = i.Id,
                                              OrderPembelianId = i.OrderPembelianId,
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


                             }).FirstOrDefault();
            return Task.FromResult(lastOrder);
        }

        public Task<IEnumerable<Orderpembelian>> GetOrders()
        {
            var orders = (from a in dbContext.OrderPembelians.Select()
                             join s in dbContext.Suppliers.Select() on a.SupplierId equals s.Id
                             join b in dbContext.OrderPembelianItems.Select()
                             on a.Id equals b.OrderPembelianId into itemGroup
                             select new Orderpembelian
                             {
                                 Supplier = s,
                                 Discount = a.Discount,
                                 OrderDate = a.OrderDate,
                                 SupplierId = a.SupplierId,
                                 Status = a.Status,
                                 Id = a.Id,
                                 Items = (from i in itemGroup
                                          join p in dbContext.Products.Select() on i.ProductId equals p.Id
                                          join uss in dbContext.Units.Select() on p.Id equals uss.ProductId into units
                                          select new OrderpembelianItem
                                          {
                                              Id = i.Id,
                                              OrderPembelianId = i.OrderPembelianId,
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
            return Task.FromResult(orders.AsEnumerable());
        }

        public Task<IEnumerable<Orderpembelian>> GetOrdersBySupplierId(int supplierId)
        {
            var orders = (from a in dbContext.OrderPembelians.Where(x => x.SupplierId == supplierId)
                          join s in dbContext.Suppliers.Select() on a.SupplierId equals s.Id
                          join b in dbContext.OrderPembelianItems.Select() on a.Id equals b.OrderPembelianId
                          into itemGroup
                          from b in itemGroup.DefaultIfEmpty()
                          select new Orderpembelian
                          {
                              Supplier = s,
                              Status = a.Status,
                              Discount = a.Discount,
                              OrderDate = a.OrderDate,
                              SupplierId = a.SupplierId,
                              Id = a.Id,
                              Items = itemGroup.ToList()
                          });
            return Task.FromResult(orders.AsEnumerable());
        }



        public Task<Orderpembelian> UpdateOrder(int id, Orderpembelian order)
        {
            var trans = dbContext.BeginTransaction();
            try
            {
                var lastOrder = (from a in dbContext.OrderPembelians.Where(x => x.Id == id)
                                 join s in dbContext.Suppliers.Select() on a.SupplierId equals s.Id
                                 join b in dbContext.OrderPembelianItems.Where(x => x.OrderPembelianId == id) on a.Id equals b.OrderPembelianId
                                 into itemGroup
                                 from b in itemGroup.DefaultIfEmpty()
                                 select new Orderpembelian
                                 {
                                     Supplier = s,
                                     Status = a.Status,
                                     Discount = a.Discount,
                                     OrderDate = a.OrderDate,
                                     SupplierId = a.SupplierId,
                                     Id = a.Id,
                                     Items = itemGroup.ToList()
                                 }).FirstOrDefault();


                if (lastOrder == null)
                    throw new SystemException("Order Not Found  !");

                var updated = dbContext.OrderPembelians.Update(x => new { x.Discount, x.OrderDate }, order, x => x.Id == order.Id);

                if (!updated)
                    throw new SystemException("Order Not Updated !");

                foreach (var item in order.Items)
                {
                    if (item.Id <= 0)
                    {
                        item.OrderPembelianId = order.Id;
                        item.Id = dbContext.OrderPembelianItems.InsertAndGetLastID(item);
                        if (item.Id <= 0)
                            throw new SystemException("Order Item Not Added !");
                    }
                    else
                    {
                        updated = dbContext.OrderPembelianItems.Update(x => new { x.Amount, x.Price, x.UnitId }, item, x => x.Id == item.Id);
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
                _logger.LogError(ex.Message);
                throw new SystemException(ex.Message);
            }
        }


        #endregion


        #region Pembayaran
        public Task<Pembayaranpembelian> CreatePembayaran(int pembelianId, Pembayaranpembelian pembayaran, bool forced)
        {
            var trans = dbContext.BeginTransaction();
            try
            {
                var pembelian = (from a in  dbContext.Pembelians.Where(x=>x.Id==pembelianId)
                                     join b in dbContext.PembelianItems.Where(x => x.PembelianId == pembelianId).ToList() 
                                     on a.Id equals b.PembelianId into gg
                                     select new Pembelian
                                     {
                                         Id=a.Id, OrderPembelianId=a.OrderPembelianId,
                                         Discount=a.Discount,
                                         Items = (from c in gg select c).ToList()
                                     }  ).FirstOrDefault();



                if (pembelian == null)
                    throw new SystemException("Pembelian Tidak Ditemukan");

                var pembayarans = dbContext.PembayaranPembelians.Where(x => x.PembelianId == pembelianId).ToList();

                var totalInvoice = pembelian.Items.Sum(x => x.Total) - (pembelian.Items.Sum(x => x.Total)*(pembelian.Discount/100)) ;
                double totalBayar = 0;
                if (pembayaran != null)
                {
                   totalBayar= pembayarans.Sum(x => x.PayValue);

                }

                var sisa = totalInvoice - totalBayar - pembayaran.PayValue;

                if (sisa < 0 && !forced)
                    throw new SystemException("Pembayaran Anda Melebihi Tagihan Invoice !");


                var status = sisa > 0 ? PaymentStatus.DownPayment:  PaymentStatus.PaidOff;
                var updatePembelian = dbContext.Pembelians.Update(x => new { x.Status }, new Pembelian { Status = status }, x => x.Id == pembelianId);
                var updateOrder= dbContext.OrderPembelians.Update(x => new { x.Status }, new Orderpembelian { Status =  OrderStatus.Complete}, x => x.Id == pembelian.OrderPembelianId);
               
                if(!updatePembelian || !updateOrder)
                    throw new SystemException("Pembayaran Gagal !");

                var result =  dbContext.PembayaranPembelians.InsertAndGetLastID(pembayaran);
                if (result <=0)
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

        public Task<IEnumerable<Pembayaranpembelian>> GetPembayaran(int pembelianId)
        {
            var pembayarans = dbContext.PembayaranPembelians.Where(x => x.PembelianId == pembelianId);
            return Task.FromResult(pembayarans.AsEnumerable());
        }
        #endregion


    }
}