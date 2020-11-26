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
    public class UnitController : ControllerBase
    {
        private OcphDbContext dbContext;

        public UnitController(OcphDbContext db)
        {
            dbContext = db;
        }

        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                return Ok(dbContext.Units.Select().ToList());
            }
            catch (Exception ex)
            {
                return Ok(new ErrorMessage(ex.Message));
            }
        }

        // GET api/<UnitController>/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                return Ok(dbContext.Units.Where(x=>x.Id==id).FirstOrDefault());
            }
            catch (Exception ex)
            {
                return Ok(new ErrorMessage(ex.Message));
            }
        }

        // POST api/<UnitController>
        [HttpPost]
        public IActionResult Post([FromBody] Unit value)
        {
            try
            {
                value.Id = dbContext.Units.InsertAndGetLastID(value);
                if (value.Id <= 0)
                    throw new SystemException("Data Not Saved !");
                return Ok(value);
            }
            catch (Exception ex)
            {
                return Ok(new ErrorMessage(ex.Message));
            }
        }

        // PUT api/<UnitController>/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Unit value)
        {
            try
            {
                var existsModel = dbContext.Units.Where(x => x.Id == id).FirstOrDefault();
                if (existsModel == null)
                    throw new SystemException("Data Not Found !");

                var updated = dbContext.Units.Update(x => new { x.Name, x.Amount }, value, x => x.Id == id);
                if (updated)
                    throw new SystemException("Data Not Saved !");
                return Ok(updated);
            }
            catch (Exception ex)
            {
                return Ok(new ErrorMessage(ex.Message));
            }
        }

        // DELETE api/<UnitController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var existsModel = dbContext.Units.Where(x => x.Id == id).FirstOrDefault();
                if (existsModel == null)
                    throw new SystemException("Data Not Found !");

                var deleted = dbContext.Units.Delete(x => x.Id == id);
                if (deleted)
                    throw new SystemException("Data Not Deleted !");
                return Ok(deleted);
            }
            catch (Exception ex)
            {
                return Ok(new ErrorMessage(ex.Message));
            }
        }
    }
}
