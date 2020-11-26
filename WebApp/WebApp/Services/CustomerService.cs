using ShareModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApp.Middlewares;

namespace WebApp.Services
{
    public interface ICustomerService :IService<Customer>
    {
    }
    public class CustomerService : ICustomerService
    {

        private OcphDbContext dbContext;
        private IUserService userService;

        public CustomerService(OcphDbContext db, IUserService _userService)
        {
            dbContext = db;
            userService = _userService;
        }
        public Task<bool> Delete(int id)
        {
            try
            {
                var existsModel = dbContext.Customers.Where(x => x.Id == id).FirstOrDefault();
                if (existsModel == null)
                    throw new SystemException("Data Not Found !");

                var deleted = dbContext.Customers.Delete(x => x.Id == id);
                if (deleted)
                    throw new SystemException("Data Not Deleted !");
                return Task.FromResult(deleted);
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public Task<Customer> Get(int id)
        {
            var result = dbContext.Customers.Where(x => x.Id == id).FirstOrDefault();
            return Task.FromResult(result);
        }

        public Task<IEnumerable<Customer>> Get()
        {
            var results = dbContext.Customers.Select().ToList();
            return Task.FromResult(results.AsEnumerable());
        }

       

        public async Task<Customer> Post(Customer value)
        {
            
            try
            {

               Customer result = await userService.RegisterCustomer(value);
                if (result ==null)
                    throw new SystemException("Data Not Saved !");
                return result;
            }
            catch (Exception ex) { 
                throw new SystemException(ex.Message);
            }
        }

        public Task<bool> Update(int id, Customer value)
        {
            var existsModel = dbContext.Customers.Where(x => x.Id == id).FirstOrDefault();
            if (existsModel == null)
                throw new SystemException("Data Not Found !");

            var updated = dbContext.Customers.Update(x => new { x.ContactName, x.Email, x.Name, x.NPWP, x.Telepon},
                value, x => x.Id == id);
            if (!updated)
                throw new SystemException("Data Not Saved !");
            return Task.FromResult(updated);
        }

    }
}
