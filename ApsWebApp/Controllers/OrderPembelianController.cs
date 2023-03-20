using Microsoft.AspNetCore.Mvc;
using ShareModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApsWebApp.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ApsWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderPembelianController : ControllerBase
    {
        private IPembelianService service;

        public OrderPembelianController(IPembelianService _service)
        {
            service = _service;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                return Ok(await service.GetOrders());
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorMessage(ex.Message));
            }
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var result = await service.GetOrder(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorMessage(ex.Message));
            }
        }


        [HttpGet("BySupplierId/{id}")]
        public async Task<IActionResult> GetOrdersBySupplierId(int id)
        {
            try
            {
                return Ok(await service.GetOrdersBySupplierId(id));
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorMessage(ex.Message));
            }
        }


        [HttpPost]
        public async Task<IActionResult> Post(OrderPembelian order)
        {
            try
            {
                return Ok(await service.CreateOrder(order));
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorMessage(ex.Message));
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, OrderPembelian order)
        {
            try
            {
                return Ok(await service.UpdateOrder(id, order));
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorMessage(ex.Message));
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {

                bool deleted = await service.DeleteOrder(id);
                if (deleted)
                    throw new SystemException("Data Not Deleted !");
                return Ok(await Task.FromResult(deleted));
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorMessage(ex.Message));
            }
        }

    }
}
