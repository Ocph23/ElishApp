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
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService customerService;

        public CustomerController(ICustomerService _customerService)
        {
            customerService = _customerService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var result = await customerService.Get();
                return Ok(result);
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
                return Ok(await customerService.Get(id));
            }
            catch (Exception ex)
            {
                return Ok(new ErrorMessage(ex.Message));
            }
        }

        // POST api/<UnitController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Customer value)
        {
            try
            {
               return Ok(await customerService.Post(value));
            }
            catch (Exception ex)
            {
                return Ok(new ErrorMessage(ex.Message));
            }
        }

        // PUT api/<UnitController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Customer value)
        {
            try
            {
                return Ok(await customerService.Update(id, value));
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
                return Ok(await customerService.Delete(id));
            }
            catch (Exception ex)
            {
                return Ok(new ErrorMessage(ex.Message));
            }
        }
    }
}
