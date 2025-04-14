using ApsWebApp.Data;
using ApsWebApp.Validations;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using ShareModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApsWebApp.Services
{

    public class KaryawanService : IKaryawanService
    {

        private readonly ApplicationDbContext dbContext;
        private readonly IUserService userService;
        private readonly IValidator<Karyawan> karyawanValidator;

        public KaryawanService(ApplicationDbContext db, IUserService _userService, IValidator<Karyawan> _karyawanValidator)
        {
            dbContext = db;
            userService = _userService;
            karyawanValidator = _karyawanValidator;
        }
        public Task<bool> Delete(int id)
        {
            try
            {
                var existsModel = dbContext.Karyawan.Where(x => x.Id == id).FirstOrDefault();
                if (existsModel == null)
                    throw new SystemException("Data Not Found !");

                dbContext.Karyawan.Remove(existsModel);
                dbContext.SaveChanges();
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public Task<Karyawan> Get(int id)
        {
            var result = dbContext.Karyawan.Where(x => x.Id == id)
                .Include(x => x.User).ThenInclude(x=>x.Roles).ThenInclude(x=>x.Role).FirstOrDefault();
            return Task.FromResult(result);
        }

        public Task<IEnumerable<Karyawan>> Get()
        {
            var results = dbContext.Karyawan;
            return Task.FromResult(results.AsEnumerable());
        }

        public Task<IEnumerable<Karyawan>> GetSales()
        {
            var result = dbContext.Karyawan
                .Include(x => x.User).ThenInclude(x => x.Roles).ThenInclude(x => x.Role).AsEnumerable();
            return Task.FromResult(result.AsEnumerable());
        }

        public async Task<Karyawan> Post(Karyawan value)
        {

            try
            {
                var validateResult = karyawanValidator.Validate(value);
                if (!validateResult.IsValid)
                    throw new SystemException(Helper.GetErrorString(validateResult.Errors));
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

        public Task<bool> RemoveRole(int id)
        {
            try
            {
                var userrole = dbContext.Userrole.FirstOrDefault(x => x.Id == id);
                if(userrole != null)
                {
                    dbContext.Userrole.Remove(userrole);
                    dbContext.SaveChanges();
                    return Task.FromResult(true);
                }
                throw new SystemException("User Role Not Found !");

            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public Task<bool> Update(int id, Karyawan value)
        {
            try
            {
                var validateResult = karyawanValidator.Validate(value);
                if (!validateResult.IsValid)
                    throw new SystemException(Helper.GetErrorString(validateResult.Errors));
                var existsModel = dbContext.Karyawan.Where(x => x.Id == id).FirstOrDefault();
                if (existsModel == null)
                    throw new SystemException("Data Not Found !");

                dbContext.Entry(existsModel).CurrentValues.SetValues(value);
                dbContext.SaveChanges();
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }



        public Task<bool> UpdateUser(User user)
        {
            try
            {
                var result = dbContext.User.Where(x => x.Id == user.Id).FirstOrDefault();
                result.UserName = user.UserName;
                dbContext.SaveChanges();
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {

                throw new SystemException(ex.Message);
            }
        }

    }
}
