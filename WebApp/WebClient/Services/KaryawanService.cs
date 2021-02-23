using Microsoft.EntityFrameworkCore;
using ShareModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebClient.Services
{
   
    public class KaryawanService : IKaryawanService
    {

        private readonly ApplicationDbContext dbContext;
        private readonly IUserService userService;

        public KaryawanService(ApplicationDbContext db, IUserService _userService)
        {
            dbContext = db;
            userService = _userService;
        }
        public async Task<bool> Delete(int id)
        {
            try
            {
                var existsModel = dbContext.Karyawan.Where(x => x.Id == id).FirstOrDefault();
                if (existsModel == null)
                    throw new SystemException("Data Not Found !");

                dbContext.Karyawan.Remove(existsModel);
                await dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public Task<Karyawan> Get(int id)
        {
            var result = dbContext.Karyawan.Where(x => x.Id == id)
                .Include(x=>x.User).FirstOrDefault();
            return Task.FromResult(result);
        }

        public Task<IEnumerable<Karyawan>> Get()
        {
            var results = dbContext.Karyawan;
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

        public async Task<bool> Update(int id, Karyawan value)
        {
            try
            {
                var existsModel = dbContext.Karyawan.Where(x => x.Id == id).FirstOrDefault();
                if (existsModel == null)
                    throw new SystemException("Data Not Found !");

                dbContext.Entry(existsModel).CurrentValues.SetValues(value);
                await dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }



        public async  Task<bool> UpdateUser(User user) {
            var result = dbContext.User.Where(x => x.Id == user.Id).FirstOrDefault();
            result.UserName = user.UserName;
           await dbContext.SaveChangesAsync();
            return true;
        }

    }
}
