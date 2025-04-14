using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShareModels
{
    public interface ICustomerService : IService<Customer>{
        //ObservableCollection<Customer> CustomerCollection { get; set; }

        Task<IEnumerable<Customer>> GetBySales(int id);
        Task<bool> UpdateLocation(Customer cust);
    }








}
