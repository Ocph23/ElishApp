using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShareModels
{
    public interface IPemindahanService
    {
        Task<IEnumerable<Pemindahan>> Get();
        Task<Pemindahan> Get(int id);
        Task<Pemindahan> Post(Pemindahan model);
        Task<bool> Put(int id, Pemindahan model);
        Task<bool> Delete(int id);
    }








}
