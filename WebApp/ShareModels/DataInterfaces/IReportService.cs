using ShareModels.Reports;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShareModels
{
    public interface IReportService
    {
        Task<IEnumerable<PiutangData>> GetPiutang();
        Task<IEnumerable<PiutangData>> GetUtang();
    }








}
