﻿using ApsWebApp.Data;
using Microsoft.EntityFrameworkCore;
using ShareModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApsWebApp.Services
{
    public class PengembalianPenjualanService : IPengembalianPenjualanService
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IStockService stockService;

        public PengembalianPenjualanService(ApplicationDbContext db, IStockService _stockService)
        {
            dbContext = db;
            stockService = _stockService;
        }

        public Task<IEnumerable<Penjualanitem>> GetPenjualanByCustomerId(int customerId)
        {
            var result = dbContext.Penjualan
                .Include(x=>x.Items).ThenInclude(x=>x.Penjualan)
                .Include(x=>x.Items).ThenInclude(x=>x.Product).ThenInclude(x=>x.Category)
                .Include(x=>x.Items).ThenInclude(x=>x.Product).ThenInclude(x=>x.Merk)
                .Include(x=>x.Items).ThenInclude(x => x.Product).ThenInclude(x=>x.Units)
                .Where(x => x.Customer.Id == customerId).SelectMany(x=>x.Items).ToList();
            return Task.FromResult(result.OrderByDescending(x=>x.Id) as IEnumerable<Penjualanitem>);
        }

        public async Task<PengembalianPenjualan> Post(PengembalianPenjualan model)  
        {
            dbContext.ChangeTracker.Clear();
            var trans = dbContext.Database.BeginTransaction();
            try
            {
                model.Created = model.Created.ToUniversalTime();
                dbContext.Entry(model.Customer).State = EntityState.Unchanged;
                dbContext.Entry(model.Gudang).State = EntityState.Unchanged;
                foreach (var item in model.Items)
                {
                    dbContext.Entry(item.Penjualan).State = EntityState.Unchanged;
                    dbContext.Entry(item.Product).State = EntityState.Unchanged;
                    dbContext.Entry(item.Unit).State = EntityState.Unchanged;
                }
                dbContext.PengembalianPenjualan.Add(model);
                dbContext.SaveChanges();

                foreach (var item in model.Items)
                {
                    double newStock = item.Quantity * item.Unit.Quantity;
                    var saved = await stockService.AddMovementStock(item.Product.Id, model.Gudang.Id, StockMovementType.IN,
                         ReferenceType.ReturnSale, item.Id, newStock);

                    if (!saved)
                        throw new SystemException("Gagal Menyimpan Stok !");
                }

                dbContext.SaveChanges();
                trans.Commit();
                return await Task.FromResult(model);
            }
            catch (Exception ex)
            {

                trans.Rollback();
                throw new SystemException(ex.Message);
            }
        }

        public async Task<PengembalianPenjualan> Put(int id, PengembalianPenjualan model)
        {
            var trans = dbContext.Database.BeginTransaction();
            try
            {
                var lastData= dbContext.PengembalianPenjualan.Where(x => x.Id == id).Include(x => x.Items)
                                 .FirstOrDefault();
                if (lastData == null)
                    throw new SystemException("Penjualan Not Found  !");
                dbContext.Entry(lastData).CurrentValues.SetValues(model);

                foreach (var item in model.Items)
                {
                    if (item.Id <= 0)
                    {
                        if (item.Product != null)
                        {
                            dbContext.Entry(item.Product).State = EntityState.Unchanged;
                            dbContext.Entry(item.Penjualan).State = EntityState.Unchanged;
                            dbContext.Entry(item.Unit).State = EntityState.Unchanged;
                        }
                        dbContext.PengembalianPenjualanItem.Add(item);
                    }
                    else
                    {
                        var oldItem = lastData.Items.SingleOrDefault(x => x.Id == item.Id);
                        if (item.Product != null)
                        {
                            dbContext.Entry(item.Product).State = EntityState.Unchanged;
                            dbContext.Entry(item.Penjualan).State = EntityState.Unchanged;
                            dbContext.Entry(item.Unit).State = EntityState.Unchanged;
                        }
                        dbContext.Entry(oldItem).CurrentValues.SetValues(item);

                        var stockMovement = await stockService.GetMovementStock(StockMovementType.IN, ReferenceType.ReturnSale, item.Id);
                        //oldItem.Quantity = item.Quantity;
                        //oldItem.Price = item.Price;
                        //oldItem.Discount = item.Discount;
                        //oldItem.Unit = item.Unit;
                        if (stockMovement.Quantity != (oldItem.Quantity * oldItem.Unit.Quantity))
                        {
                            await stockService.UpdateStockMovement(stockMovement, item.Quantity * item.Unit.Quantity);
                        }





                    }
                }


                //remove

                foreach (var item in lastData.Items)
                {
                    var existsDb = model.Items.Where(x => x.Id == item.Id).FirstOrDefault();
                    if (existsDb == null)
                    {
                        dbContext.PengembalianPenjualanItem.Remove(item);
                    }
                }

                dbContext.SaveChanges();
                trans.Commit();
                return model;
            }
            catch (Exception ex)
            {
                trans.Rollback();
                throw new SystemException(ex.Message);
            }
        }

    }
}
