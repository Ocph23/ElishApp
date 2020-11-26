using Microsoft.AspNetCore.Mvc;
using ShareModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase, IBaseController<Category>
    {
        private OcphDbContext dbContext;

        public CategoryController(OcphDbContext db)
        {
            dbContext = db;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                return Ok(await Task.FromResult( dbContext.Categories.Select().ToList()));
            }
            catch (Exception ex)
            {
                return Ok(new ErrorMessage(ex.Message));
            }
        }

        // GET api/<UnitController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                return Ok(await Task.FromResult(dbContext.Categories.Where(x=>x.Id==id).FirstOrDefault()));
            }
            catch (Exception ex)
            {
                return Ok(new ErrorMessage(ex.Message));
            }
        }

        // POST api/<UnitController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Category value)
        {
            try
            {
                value.Id = dbContext.Categories.InsertAndGetLastID(value);
                if (value.Id <= 0)
                    throw new SystemException("Data Not Saved !");
                return Ok(await Task.FromResult(value));
            }
            catch (Exception ex)
            {
                return Ok(new ErrorMessage(ex.Message));
            }
        }

        // PUT api/<UnitController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Category value)
        {
            try
            {
                var existsModel = dbContext.Categories.Where(x => x.Id == id).FirstOrDefault();
                if (existsModel == null)
                    throw new SystemException("Data Not Found !");

                var updated = dbContext.Categories.Update(x => new { x.Name, x.Description }, value, x => x.Id == id);
                if (updated)
                    throw new SystemException("Data Not Saved !");
                return Ok(await Task.FromResult(updated));
            }
            catch (Exception ex)
            {
                return Ok(new ErrorMessage(ex.Message));
            }
        }

        // DELETE api/<UnitController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var existsModel = dbContext.Categories.Where(x => x.Id == id).FirstOrDefault();
                if (existsModel == null)
                    throw new SystemException("Data Not Found !");

                var deleted = dbContext.Categories.Delete(x => x.Id == id);
                if (deleted)
                    throw new SystemException("Data Not Deleted !");
                return Ok(await Task.FromResult(deleted));
            }
            catch (Exception ex)
            {
                return Ok(new ErrorMessage(ex.Message));
            }
        }

    }
}
