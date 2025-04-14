using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShareModels
{
    public interface IPengembalianPenjualanService
    {
        Task<IEnumerable<Penjualanitem>> GetPenjualanByCustomerId(int customerId);
        Task<PengembalianPenjualan> Post(PengembalianPenjualan model);
        Task<PengembalianPenjualan> Put(int id, PengembalianPenjualan model);

    }








}
