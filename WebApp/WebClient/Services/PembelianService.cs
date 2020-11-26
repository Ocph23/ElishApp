using Microsoft.AspNetCore.Http;
using ShareModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebClient.Services
{
    public interface IPembelianService  
    {
       Task<Orderpembelian> CreateOrder(Orderpembelian order);
        Task<IEnumerable<Orderpembelian>> GetOrdersBySupplierId(int supplierId);
        Task<IEnumerable<Orderpembelian>> GetOrders();
        Task<Orderpembelian> GetOrder(int id);
        Task<Orderpembelian> UpdateOrder(int orderId, Orderpembelian order);
        Task<bool> DeleteOrder(int id);
        //Pembelian
        Task<Pembelian> CreatePembelian(int orderid);
        Task<Pembelian> UpdatePembelian(int pembelianId, Pembelian order);
        Task<Pembelian> GetPembelian(int id);
        Task<IEnumerable<Pembelian>> GetPembelians();
        Task<IEnumerable<Pembelian>> GetPembeliansBySupplierId(int id);
        Task<bool> DeletePembelian(int id);
    }

    public class PembelianService : IPembelianService
    {
        private OcphDbContext dbContext;
        private IHttpContextAccessor auth;

        public PembelianService(OcphDbContext db, IHttpContextAccessor httpContextAccessor)
        {
            dbContext = db;
            auth = httpContextAccessor;
        }
        public Task<Pembelian> CreatePembelian(int orderid)
        {
            var trans = dbContext.BeginTransaction();
            try
            {
                var lastOrder = (from a in dbContext.OrderPembelians.Where(x => x.Id == orderid)
                                 join b in dbContext.OrderPembelianItems.Where(x => x.OrderPembelianId == orderid) on a.Id equals b.OrderPembelianId
                                 into itemGroup
                                 from b in itemGroup.DefaultIfEmpty()
                                 select new Orderpembelian
                                 {
                                     Discount = a.Discount,
                                     OrderDate = a.OrderDate,
                                     SupplierId = a.SupplierId,
                                     Id = a.Id,
                                     Items = itemGroup.ToList()
                                 }).FirstOrDefault();

                if (lastOrder == null)
                    throw new SystemException("Order Tidak Ditemukan !");


                var userid = auth.HttpContext.User.UserId();
                var pembelian = new Pembelian { UserId=userid.Value, OrderPembelianId = orderid, PayDeadLine = DateTime.Now, CreatedDate = DateTime.Now, Items = new List<PembelianItem>() };

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

                trans.Rollback();
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
                                      CreatedDate=a.CreatedDate,
                                       PayDeadLine=a.PayDeadLine, UserId=a.UserId,
                                     Id = a.Id,
                                     Items = itemGroup.ToList()
                                 }).FirstOrDefault();


                if (lastOrder == null)
                    throw new SystemException("Pembelian Not Found  !");

                 var updated = dbContext.Pembelians.Update(x => new { x.CreatedDate, x.PayDeadLine, x.UserId}, order, x => x.Id == order.Id);

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
                trans.Rollback();
                throw new SystemException(ex.Message);
            }
        }

        public Task<Pembelian> GetPembelian(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Pembelian>> GetPembelians()
        {
            var orders = (from a in dbContext.Pembelians.Select()
                          join b in dbContext.PembelianItems.Select() on a.Id equals b.PembelianId
                          into itemGroup
                          from b in itemGroup.DefaultIfEmpty()
                          select new Pembelian
                          {
                              CreatedDate = a.CreatedDate,
                              OrderPembelianId = a.OrderPembelianId,
                              PayDeadLine = a.PayDeadLine,
                              UserId = a.UserId,
                              Id = a.Id,
                              Items = itemGroup.ToList()
                          });

            return Task.FromResult(orders.AsEnumerable());
        }

        public Task<IEnumerable<Pembelian>> GetPembeliansBySupplierId(int id)
        {
            var orders = (  from o in dbContext.OrderPembelians.Where(x=>x.SupplierId==id)
                            join a in dbContext.Pembelians.Select() on o.Id equals a.OrderPembelianId
                            join b in dbContext.PembelianItems.Select() on a.Id equals b.PembelianId
                            into itemGroup
                            from b in itemGroup.DefaultIfEmpty()
                            select new Pembelian
                              {
                                  CreatedDate = a.CreatedDate,
                                  OrderPembelianId = a.OrderPembelianId,
                                  PayDeadLine = a.PayDeadLine,
                                  UserId = a.UserId,
                                  Id = a.Id,
                                  Items = itemGroup.ToList()
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
                throw new SystemException(ex.Message);
            }
        }
        //Order
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
                    if(item.Id<=0)
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
                var deleted = dbContext.OrderPembelians.Delete(x => x.Id == id);
                if (!deleted)
                    throw new SystemException("Order Not Deleted !");
                return Task.FromResult(deleted);
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public Task<Orderpembelian> GetOrder(int id)
        {
            var lastOrder = (from a in dbContext.OrderPembelians.Where(x => x.Id == id)
                             join b in dbContext.OrderPembelianItems.Where(x => x.OrderPembelianId == id) on a.Id equals b.OrderPembelianId
                             into itemGroup
                             from b in itemGroup.DefaultIfEmpty()
                             select new Orderpembelian
                             {
                                 Discount = a.Discount,
                                 OrderDate = a.OrderDate,
                                 SupplierId = a.SupplierId,
                                 Id = a.Id,
                                 Items = itemGroup.ToList()
                             }).FirstOrDefault();
            return Task.FromResult(lastOrder);
        }

        public Task<IEnumerable<Orderpembelian>> GetOrders()
        {
            var orders = (from a in dbContext.OrderPembelians.Select()
                             join b in dbContext.OrderPembelianItems.Select() on a.Id equals b.OrderPembelianId
                             into itemGroup
                             from b in itemGroup.DefaultIfEmpty()
                             select new Orderpembelian
                             {
                                 Discount = a.Discount,
                                 OrderDate = a.OrderDate,
                                 SupplierId = a.SupplierId,
                                 Id = a.Id,
                                 Items = itemGroup.ToList()
                             });
            return Task.FromResult(orders.AsEnumerable());
        }

        public Task<IEnumerable<Orderpembelian>> GetOrdersBySupplierId(int supplierId)
        {
            var orders = (from a in dbContext.OrderPembelians.Where(x=>x.SupplierId==supplierId)
                          join b in dbContext.OrderPembelianItems.Select() on a.Id equals b.OrderPembelianId
                          into itemGroup
                          from b in itemGroup.DefaultIfEmpty()
                          select new Orderpembelian
                          {
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
                var lastOrder = (from a in dbContext.OrderPembelians.Where(x => x.Id ==id)
                                 join b in dbContext.OrderPembelianItems.Where(x => x.OrderPembelianId == id) on a.Id equals b.OrderPembelianId
                                 into itemGroup
                                 from b in itemGroup.DefaultIfEmpty()
                                 select new Orderpembelian
                                 {
                                     Discount = a.Discount,
                                     OrderDate = a.OrderDate,
                                     SupplierId = a.SupplierId,
                                     Id = a.Id,
                                     Items = itemGroup.ToList()
                                 }).FirstOrDefault();


                if(lastOrder==null)
                    throw new SystemException("Order Not Found  !");

                var updated = dbContext.OrderPembelians.Update(x=> new { x.Discount, x.OrderDate },order,x=>x.Id==order.Id);

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

                if(order.Items.Count != lastOrder.Items.Count)
                {
                    foreach (var item in lastOrder.Items)
                    {
                        var existsDb = order.Items.Where(x => x.Id == item.Id).FirstOrDefault();
                        if(existsDb==null)
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

       
    }
}
