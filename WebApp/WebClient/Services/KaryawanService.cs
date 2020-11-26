using ShareModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebClient.Services
{
    public interface IKaryawanService : IService<Karyawan>
    {
    }
    public class KaryawanService : IKaryawanService
    {

        private OcphDbContext dbContext;
        private IUserService userService;

        public KaryawanService(OcphDbContext db, IUserService _userService)
        {
            dbContext = db;
            userService = _userService;
        }
        public Task<bool> Delete(int id)
        {
            try
            {
                var existsModel = dbContext.Karyawans.Where(x => x.Id == id).FirstOrDefault();
                if (existsModel == null)
                    throw new SystemException("Data Not Found !");

                var deleted = dbContext.Karyawans.Delete(x => x.Id == id);
                if (deleted)
                    throw new SystemException("Data Not Deleted !");
                return Task.FromResult(deleted);
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public Task<Karyawan> Get(int id)
        {
            var result = dbContext.Karyawans.Where(x => x.Id == id).FirstOrDefault();
            return Task.FromResult(result);
        }

        public Task<IEnumerable<Karyawan>> Get()
        {
            var results = dbContext.Karyawans.Select().ToList();
            return Task.FromResult(results.AsEnumerable());
        }

        public async Task<Karyawan> Post(Karyawan value)
        {

            try
            {
                Karyawan result = await userService.RegisterKaryawan(value);
                if (result == null)
                    throw new SystemException("Data Not Saved !");
                return result;
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public Task<bool> Update(int id, Karyawan value)
        {
            var existsModel = dbContext.Karyawans.Where(x => x.Id == id).FirstOrDefault();
            if (existsModel == null)
                throw new SystemException("Data Not Found !");

            var updated = dbContext.Karyawans.Update(x => new { x.Address, x.Email, x.Name, x.Telepon },
                value, x => x.Id == id);
            if (!updated)
                throw new SystemException("Data Not Saved !");
            return Task.FromResult(updated);
        }

    }
}
