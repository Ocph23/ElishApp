using ShareModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElishAppDesktop
{
    public class CategoryService : ICategoryService
    {
        private string controller = "/api/category";
        private IUserService userService;

        public async Task<bool> Delete(int id)
        {
            try
            {
                using (var res = new RestService())
                {
                   var response= await res.DeleteAsync($"{controller}/{id}");
                    if (!response.IsSuccessStatusCode)
                        await res.Error(response);
                    return  await response.GetResult<bool>();
                }
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public async Task<Category> Get(int id)
        {
            try
            {
                using (var res = new RestService())
                {
                    var response = await res.GetAsync($"{controller}/{id}");
                    if (!response.IsSuccessStatusCode)
                        await res.Error(response);
                    return await response.GetResult<Category>();
                }
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public async Task<IEnumerable<Category>> Get()
        {
            try
            {
                using (var res = new RestService())
                {
                    var response = await res.GetAsync($"{controller}");
                    if (!response.IsSuccessStatusCode)
                        await res.Error(response);
                    return await response.GetResult<IEnumerable<Category>>();
                }
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public async Task<Category> Post(Category value)
        {

            try
            {
                using (var res = new RestService())
                {
                    var response = await res.PostAsync($"{controller}", res.GenerateHttpContent(value));
                    if (!response.IsSuccessStatusCode)
                        await res.Error(response);
                    return await response.GetResult<Category>();
                }
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public async Task<bool> Update(int id, Category value)
        {
            try
            {
                using (var res = new RestService())
                {
                    var response = await res.PutAsync($"{controller}/{id}", res.GenerateHttpContent(value));
                    if (!response.IsSuccessStatusCode)
                        await res.Error(response);
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
