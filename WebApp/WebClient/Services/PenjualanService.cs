using Microsoft.AspNetCore.Http;
using ShareModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebClient.Services
{
    public interface IPenjualanService
    {
       Task<Orderpenjualan> CreateOrder(Orderpenjualan order);
        Task<IEnumerable<Orderpenjualan>> GetOrdersBySalesId(int supplierId);
        Task<IEnumerable<Orderpenjualan>> GetOrdersByCustomerId(int supplierId);
        Task<IEnumerable<Orderpenjualan>> GetOrders();
        Task<Orderpenjualan> GetOrder(int id);
        Task<Orderpenjualan> UpdateOrder(int orderId, Orderpenjualan order);
        Task<bool> DeleteOrder(int id);
        //Pembelian

        Task<Penjualan> CreatePenjualan(int orderid);
        Task<Penjualan> UpdatePenjualan(int pembelianId, Penjualan order);
        Task<Penjualan> GetPenjualan(int id);
        Task<IEnumerable<Penjualan>> GetPenjualans();
        Task<IEnumerable<Penjualan>> GetPenjualansByCustomerId(int id);
        Task<IEnumerable<Penjualan>> GetPenjualansBySalesId(int id);
        Task<bool> DeletePenjualan(int id);
    }


    public class PenjualanService : IPenjualanService
    {
        private OcphDbContext dbContext;
        private IHttpContextAccessor auth;

        public PenjualanService(OcphDbContext db, IHttpContextAccessor httpContextAccessor)
        {
            dbContext = db;
            auth = httpContextAccessor;
        }
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
                                     SalesId=a.SalesId,
                                     Items = itemGroup.ToList()
                                 }).FirstOrDefault();

                if (lastOrder == null)
                    throw new SystemException("Order Tidak Ditemukan !");


                var userid = auth.HttpContext.User.UserId();
                var penjualan = new Penjualan { UserId=userid.Value, OrderPenjualanId = orderid, PayDeadLine = DateTime.Now, CreateDate = DateTime.Now, Items = new List<Penjualanitem>() };

                penjualan.Id = dbContext.Penjualans.InsertAndGetLastID(penjualan);

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
                                      CreateDate=a.CreateDate,      Discount=a.Discount, OrderPenjualanId=a.OrderPenjualanId, 
                                     Payment=a.Payment,
                                       PayDeadLine=a.PayDeadLine, UserId=a.UserId,
                                     Id = a.Id,
                                     Items = itemGroup.ToList()
                                 }).FirstOrDefault();


                if (lastOrder == null)
                    throw new SystemException("Penjualan Not Found  !");

                 var updated = dbContext.Penjualans.Update(x => new { x.CreateDate, x.PayDeadLine, x.UserId}, order, x => x.Id == order.Id);

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
                          join b in dbContext.PenjualanItems.Select() on a.Id equals b.PenjualanId
                          into itemGroup
                          from b in itemGroup.DefaultIfEmpty()
                          select new Penjualan
                          {
                              CreateDate = a.CreateDate,
                              OrderPenjualanId = a.OrderPenjualanId,
                              PayDeadLine = a.PayDeadLine,     Discount=a.Discount, Payment=a.Payment,
                              UserId = a.UserId,
                              Id = a.Id,
                              Items = itemGroup.ToList()
                          });

            return Task.FromResult(orders.AsEnumerable());
        }
        public Task<Penjualan> GetPenjualan(int id)
        {
            var orders = (from a in dbContext.Penjualans.Where(x => x.Id == id)
                          join b in dbContext.PenjualanItems.Select() on a.Id equals b.PenjualanId
                          into itemGroup
                          from b in itemGroup.DefaultIfEmpty()
                          select new Penjualan
                          {
                              Discount = a.Discount,
                              CreateDate = a.CreateDate,
                              OrderPenjualanId = a.OrderPenjualanId,
                              PayDeadLine = a.PayDeadLine,
                              Payment = a.Payment,
                              UserId = a.UserId,
                              Id = a.Id,
                              Items = itemGroup.ToList()
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
                              UserId = a.UserId,
                              Id = a.Id,
                              Items = itemGroup.ToList()
                          });

            return Task.FromResult(orders.AsEnumerable());
        }

        public Task<IEnumerable<Penjualan>> GetPenjualansByCustomerId(int id)
        {
            var orders = (  from o in dbContext.OrderPenjualans.Where(x=>x.CustomerId==id)
                            join a in dbContext.Penjualans.Select() on o.Id equals a.OrderPenjualanId
                            join b in dbContext.PenjualanItems.Select() on a.Id equals b. PenjualanId
                            into itemGroup
                            from b in itemGroup.DefaultIfEmpty()
                            select new Penjualan
                              {
                                  CreateDate = a.CreateDate,
                                  OrderPenjualanId= a.OrderPenjualanId,
                                  PayDeadLine = a.PayDeadLine,
                                  UserId = a.UserId,
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
        //Order
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
                    item.OrderPenjualanId= order.Id;
                    item.Id = dbContext.OrderPenjualanItems.InsertAndGetLastID(item);
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
                             join b in dbContext.OrderPenjualanItems.Where(x => x.OrderPenjualanId == id) on a.Id equals b.OrderPenjualanId
                             into itemGroup
                             from b in itemGroup.DefaultIfEmpty()
                             select new Orderpenjualan
                             {
                                 Discount = a.Discount,
                                 OrderDate = a.OrderDate,
                                 CustomerId = a.CustomerId,
                                 SalesId=a.SalesId,
                                 Id = a.Id,
                                 Items = itemGroup.ToList()
                             }).FirstOrDefault();
            return Task.FromResult(lastOrder);
        }

        public Task<IEnumerable<Orderpenjualan>> GetOrders()
        {
            var orders = (from a in dbContext.OrderPenjualans.Select()
                             join b in dbContext.OrderPenjualanItems.Select() on a.Id equals b.OrderPenjualanId
                             into itemGroup
                             from b in itemGroup.DefaultIfEmpty()
                             select new Orderpenjualan
                             {
                                 Discount = a.Discount,
                                 OrderDate = a.OrderDate,
                                 CustomerId = a.CustomerId,
                                   SalesId=a.SalesId,
                                 Id = a.Id,
                                 Items = itemGroup.ToList()
                             });
            return Task.FromResult(orders.AsEnumerable());
        }

        public Task<IEnumerable<Orderpenjualan>> GetOrdersByCustomerId(int customerId)
        {
            var orders = (from a in dbContext.OrderPenjualans.Where(x=>x.CustomerId== customerId)
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
            var orders = (from a in dbContext.OrderPenjualans.Where(x=>x.SalesId==salesId)
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
                var lastOrder = (from a in dbContext.OrderPenjualans.Where(x => x.Id ==id)
                                 join b in dbContext.OrderPenjualanItems.Where(x => x.OrderPenjualanId == id) on a.Id equals b.OrderPenjualanId
                                 into itemGroup
                                 from b in itemGroup.DefaultIfEmpty()
                                 select new Orderpenjualan
                                 {
                                     Discount = a.Discount,                 
                                     OrderDate = a.OrderDate,
                                     CustomerId= a.CustomerId,
                                     Id = a.Id,
                                     SalesId=a.SalesId,       
                                     Items = itemGroup.ToList()
                                 }).FirstOrDefault();


                if(lastOrder==null)
                    throw new SystemException("Order Not Found  !");

                var updated = dbContext.OrderPenjualans.Update(x=> new { x.Discount, x.OrderDate },order,x=>x.Id==order.Id);

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
