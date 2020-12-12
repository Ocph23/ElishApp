using Microsoft.AspNetCore.Mvc;
using ShareModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebClient.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebClient.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase, IBaseController<Product>
    {
        private IProductService service;

        public ProductController(IProductService _service)
        {
            service = _service;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                return Ok( await service.Get());
            }
            catch (Exception ex)
            {
                return Ok(new ErrorMessage(ex.Message));
            }
        }

        [HttpGet("stock")]
        public async Task<IActionResult> GetStock()
        {
            try
            {
                return Ok(await service.GetProductStock());
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
                var result = await service.Get(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(new ErrorMessage(ex.Message));
            }
        }

        

        // POST api/<UnitController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Product value)
        {
            try
            {
                return Ok(await service.Post(value));
            }
            catch (Exception ex)
            {
                return Ok(new ErrorMessage(ex.Message));
            }
        }


        // POST api/<UnitController>
        [HttpPost("AddUnit/{productId}")]
        public async Task<IActionResult> AddUnit(int productId, [FromBody] Unit value)
        {
            try
            {
                return Ok(await service.AddUnit(productId, value));
            }
            catch (Exception ex)
            {
                return Ok(new ErrorMessage(ex.Message));
            }
        }



        // POST api/<UnitController>
        [HttpPut("UpdateUnit/{unitId}")]
        public async Task<IActionResult> UpdateUnit(int unitId, [FromBody] Unit value)
        {
            try
            {
                return Ok(await service.UpdateUnit(unitId, value));
            }
            catch (Exception ex)
            {
                return Ok(new ErrorMessage(ex.Message));
            }
        }




        // PUT api/<UnitController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Product value)
        {
            try
            {
                var result = await service.Update(id, value);
                return Ok(result);
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
                bool deleted = await service.Delete(id);
                return Ok(deleted);
            }
            catch (Exception ex)
            {
                return Ok(new ErrorMessage(ex.Message));
            }
        }
    }
}
