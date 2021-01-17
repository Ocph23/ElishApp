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
        public ObservableCollection<Customer> CustomerCollection { get; set; }

        public CustomerService(ApplicationDbContext db, IUserService _userService)
        {
            dbContext = db;
            userService = _userService;
            CustomerCollection = new ObservableCollection<Customer>();
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
            Customer customer;
            if (CustomerCollection != null || CustomerCollection.Count > 0)
            {
                customer = CustomerCollection.Where(x => x.Id == id).FirstOrDefault();
            }
            else
            {
                customer = dbContext.Customer.Where(x => x.Id == id).FirstOrDefault();
            }
            return Task.FromResult(customer);
        }

        public Task<IEnumerable<Customer>> Get()
        {
            if(CustomerCollection==null || CustomerCollection.Count<=0)
            {
                var data=dbContext.Customer;
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

        public async Task<bool> Update(int id, Customer value)
        {
            var existsModel = dbContext.Customer.Where(x => x.Id == id).FirstOrDefault();
            if (existsModel == null)
                throw new SystemException("Data Not Found !");


            dbContext.Entry(existsModel).CurrentValues.SetValues(value);

            await dbContext.SaveChangesAsync();

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

            return true;
        }

    }
}
