using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShareModels
{
    public interface IKaryawanService : IService<Karyawan>
    {
        Task<bool> RemoveRole(int id);
        Task<IEnumerable<Karyawan>> GetSales();
    }








}
