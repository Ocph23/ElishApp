using Microsoft.EntityFrameworkCore;
using ShareModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace WebClient.Services
{
  
    public class CustomerService : ICustomerService
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IUserService userService;
        //public ObservableCollection<Customer> CustomerCollection { get; set; }

        public CustomerService(ApplicationDbContext db, IUserService _userService)
        {
            dbContext = db;
            userService = _userService;
           // CustomerCollection = new ObservableCollection<Customer>();
        }

        public async Task<bool> Delete(int id)
        {
            try
            {
                var existsModel = dbContext.Customer.Where(x => x.Id == id).FirstOrDefault();
                if (existsModel == null)
                    throw new SystemException("Data Not Found !");

                dbContext.Customer.Remove(existsModel);
               await dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public Task<Customer> Get(int id)
        {
            var customer = dbContext.Customer.Where(x => x.Id == id).Include(x=>x.Karyawan).FirstOrDefault();
            return Task.FromResult(customer);
        }

        public Task<IEnumerable<Customer>> Get()
        {
            var customer = dbContext.Customer;
            return Task.FromResult(customer.AsEnumerable());
        }


        public async Task<Customer> Post(Customer value)
        {
            try
            {
                Customer result = await userService.RegisterCustomer(value);
                if (result == null)
                    throw new SystemException("Data Not Saved !");
                return result;
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public async Task<bool> Update(int id, Customer value)
        {
            try
            {
                var existsModel = dbContext.Customer.Where(x => x.Id == id).FirstOrDefault();
                if (existsModel == null)
                    throw new SystemException("Data Not Found !");
                dbContext.Entry(existsModel).CurrentValues.SetValues(value);
                await dbContext.SaveChangesAsync();
              return true;
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }


           
        }

        public Task<IEnumerable<Customer>> GetBySales(int id)
        {
            var customers = dbContext.Customer.Where(x => x.KaryawanId== id);
            return Task.FromResult(customers.AsEnumerable());
        }

        public async Task<bool> UpdateLocation(Customer cust)
        {
            try
            {
                var existsModel = dbContext.Customer.Where(x => x.Id == cust.Id).FirstOrDefault();
                if (existsModel == null)
                    throw new SystemException("Data Not Found !");

                existsModel.Location = cust.Location;
                await dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }
    }
}
