using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShareModels
{
    public interface IUserService   : IUserAuthentification
    {
        Task<User> FindUserById(int id);
        Task<string> AuthenticateUSerProvider(User user);
        Task<string> GenerateToken(User user);
        Task<User> Register(RegisterModel user);
        Task AddUserRole(string v, User admin);
        Task<Customer> RegisterCustomer(Customer value);
        Task<Karyawan> RegisterKaryawan(Karyawan value);
        Task<User> FindUserByUserName(string userName);
        Task<User> FindUserByEmail(string email);
        Task<IEnumerable<User>> GetUsers();
    }








}
