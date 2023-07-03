using ApsMobileApp.Models;
using ApsMobileApp.Services;
using ShareModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApsMobileApp.Services
{
    public class KaryawanService : IKaryawanService
    {
        private readonly ElishDbStore localDb;
        private string controller = "/api/karyawan";
        private IEnumerable<Karyawan> karyawans;


        public KaryawanService(ElishDbStore _localDb)
        {
            localDb = _localDb;
        }
        public async Task<bool> Delete(int id)
        {
            try
            {
                using (var res = new RestService())
                {
                   var response= await res.DeleteAsync($"{controller}/{id}");
                    if (!response.IsSuccessStatusCode)
                        throw new SystemException(await res.Error(response));
                    return  await response.GetResult<bool>();
                }
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public async Task<Karyawan> Get(int id)
        {
            try
            {
                using (var res = new RestService())
                {
                    var response = await res.GetAsync($"{controller}/{id}");
                    if (!response.IsSuccessStatusCode)
                        throw new SystemException(await res.Error(response));
                    return await response.GetResult<Karyawan>();
                }
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public async Task<IEnumerable<Karyawan>> Get()
        {
            try
            {
                if (karyawans == null)
                {
                    var connection = Helper.CheckInterNetConnection();
                    
                    if (connection.Item1)
                    {
                        using var res = new RestService();
                        var response = await res.GetAsync($"{controller}");
                        if (!response.IsSuccessStatusCode)
                            throw new SystemException(await res.Error(response));
                        var datas = await response.GetResult<IEnumerable<Karyawan>>();
                        _ = localDb.Save<SqlDataModelKaryawan, Karyawan>(datas);
                        karyawans = datas;
                    }
                    else
                    {
                        var datas = await localDb.GetAsync<SqlDataModelKaryawan, Karyawan>();
                        karyawans = datas;
                    }

                }
                return karyawans;
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public async Task<IEnumerable<Karyawan>> GetSales()
        {
            try
            {
                if (karyawans == null)
                {
                    var connection = Helper.CheckInterNetConnection();
                  
                    if (connection.Item1)
                    {
                        using var res = new RestService();
                        var response = await res.GetAsync($"{controller}/sales");
                        if (!response.IsSuccessStatusCode)
                            throw new SystemException(await res.Error(response));
                        var datas = await response.GetResult<IEnumerable<Karyawan>>();
                        _ =  localDb.Save<SqlDataModelKaryawan, Karyawan>(datas);
                        karyawans = datas;
                    }
                    else
                    {
                        var datas = await localDb.GetAsync<SqlDataModelKaryawan, Karyawan>();
                        karyawans = datas;
                    }

                }
                return karyawans;
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public async Task<Karyawan> Post(Karyawan value)
        {

            try
            {
                using (var res = new RestService())
                {
                    var response = await res.PostAsync($"{controller}", res.GenerateHttpContent(value));
                    if (!response.IsSuccessStatusCode)
                        throw new SystemException(await res.Error(response));
                    return await response.GetResult<Karyawan>();
                }
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public Task<bool> RemoveRole(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> Update(int id, Karyawan value)
        {
            try
            {
                using (var res = new RestService())
                {
                    var response = await res.PutAsync($"{controller}/{id}", res.GenerateHttpContent(value));
                    if (!response.IsSuccessStatusCode)
                        throw new SystemException(await res.Error(response));
                    return await response.GetResult<bool>();
                }
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

    }
}
