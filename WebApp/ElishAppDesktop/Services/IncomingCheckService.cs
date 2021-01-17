using ShareModels;
using ShareModels.ModelViews;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

namespace ElishAppDesktop.Services
{
    public class IncomingCheckService : IIncommingService
    {
       readonly string controller = "/api/IncomingCheck";
       private PembelianModel list;

        public ObservableCollection<Pembelian> Pembelians { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public ObservableCollection<IncomingItem> Datas { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Pembelian PembelianSelected { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public async Task<PembelianModel> CreateNew(int pembelianid)
        {
            try
            {
                using var res = new RestService();
                var response = await res.GetAsync($"{controller}/{pembelianid}");
                if (!response.IsSuccessStatusCode)
                    await res.Error(response);
                list = await response.GetResult<PembelianModel>();
                return list;

            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }

        }

        public Task Invoike(IncomingItem data)
        {
            throw new NotImplementedException();
        }

        public async Task<PembelianModel> Load(bool force = false)
        {
            if (force || list == null )
            {
                try
                {
                    using var res = new RestService();
                    var response = await res.GetAsync($"{controller}");
                    if (!response.IsSuccessStatusCode)
                        await res.Error(response);
                    return await response.GetResult<PembelianModel>();
                }
                catch (Exception ex)
                {
                    throw new SystemException(ex.Message);
                }
            }

            return list;
        }

        public Task Save()
        {
            throw new NotImplementedException();
        }
    }
}
