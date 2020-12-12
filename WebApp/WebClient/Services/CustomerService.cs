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
        private readonly OcphDbContext dbContext;
        private readonly IUserService userService;
        public ObservableCollection<Customer> CustomerCollection { get; set; }

        public CustomerService(OcphDbContext db, IUserService _userService)
        {
            dbContext = db;
            userService = _userService;
            CustomerCollection = new ObservableCollection<Customer>();
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
            Customer customer;
            if (CustomerCollection != null || CustomerCollection.Count > 0)
            {
                customer = CustomerCollection.Where(x => x.Id == id).FirstOrDefault();
            }
            else
            {
                customer = dbContext.Customers.Where(x => x.Id == id).FirstOrDefault();
            }
            return Task.FromResult(customer);
        }

        public Task<IEnumerable<Customer>> Get()
        {
            if(CustomerCollection==null || CustomerCollection.Count<=0)
            {
                var data=dbContext.Customers.Select().ToList();
                if (data != null)
                {
                    CustomerCollection = new ObservableCollection<Customer>(data);
                }
            }
            return Task.FromResult(CustomerCollection.AsEnumerable());
        }


        public async Task<Customer> Post(Customer value)
        {
            try
            {
                Customer result = await userService.RegisterCustomer(value);
                if (result == null)
                    throw new SystemException("Data Not Saved !");
                if (CustomerCollection != null)
                    CustomerCollection.Add(result);
                return result;
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public Task<bool> Update(int id, Customer value)
        {
            var existsModel = dbContext.Customers.Where(x => x.Id == id).FirstOrDefault();
            if (existsModel == null)
                throw new SystemException("Data Not Found !");

            var updated = dbContext.Customers.Update(x => new { x.ContactName, x.Email,x.Address, x.Name, x.NPWP, x.Telepon },
                value, x => x.Id == id);
            if (!updated)
                throw new SystemException("Data Not Saved !");

            var existOnModel = CustomerCollection.Where(x => x.Id == id).FirstOrDefault();
            if (existOnModel!=null)
            {
                existOnModel.Address = value.Address;
                existOnModel.ContactName = value.ContactName;
                existOnModel.Email = value.Email;
                existOnModel.Name = value.Name;
                existOnModel.NPWP = value.NPWP;
                existOnModel.Telepon = value.Telepon;
                existOnModel.UserId = value.UserId;
            }

            return Task.FromResult(updated);
        }

    }
}
