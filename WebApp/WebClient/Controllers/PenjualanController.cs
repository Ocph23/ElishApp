using Microsoft.AspNetCore.Mvc;
using ShareModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebClient.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebClient.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PenjualanController : ControllerBase
    {
        private readonly IPenjualanService service;

        public PenjualanController(IPenjualanService _service)
        {
            service = _service;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                return Ok(await service.GetPenjualans());
            }
            catch (Exception ex)
            {
                return Ok(new ErrorMessage(ex.Message));
            }
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var result = await service.GetPenjualan(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(new ErrorMessage(ex.Message));
            }
        }


        [HttpGet("ByCustomerId/{id}")]
        public async Task<IActionResult> GetPenjualanByCustomerId(int id)
        {
            try
            {
                return Ok(await service.GetPenjualansByCustomerId(id));
            }
            catch (Exception ex)
            {
                return Ok(new ErrorMessage(ex.Message));
            }
        }

        [HttpGet("BySalesId/{id}")]
        public async Task<IActionResult> GetPenjualanBySalesId(int id)
        {
            try
            {
                return Ok(await service.GetPenjualansBySalesId(id));
            }
            catch (Exception ex)
            {
                return Ok(new ErrorMessage(ex.Message));
            }
        }


        [ApiAuthorize]
        [HttpPost("{orderid}")]
        public async Task<IActionResult> Post(int orderid)
        {
            try
            {
                return Ok(await service.CreatePenjualan(orderid));
            }
            catch (Exception ex)
            {
                return Ok(new ErrorMessage(ex.Message));
            }
        }


        [HttpPost("order")]
        public async Task<IActionResult> PostOrder(Orderpenjualan order)
        {
            try
            {
                var result = await service.CreateOrder(order);
                if (result != null)
                    return Ok(result);
                throw new SystemException("Order Not Created !");
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorMessage(ex.Message));
            }
        }

        [HttpGet("order")]
        public async Task<IActionResult> GetOrders()
        {
            try
            {
                var result = await service.GetOrders();
                if (result != null)
                    return Ok(result);
                throw new SystemException("Order Not Created !");
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorMessage(ex.Message));
            }
        }

        [HttpGet("order/{id}")]
        public async Task<IActionResult> GetOrder(int id)
        {
            try
            {
                var result = await service.GetOrder(id);
                if (result != null)
                    return Ok(result);
                throw new SystemException("Order Not Created !");
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorMessage(ex.Message));
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, Penjualan penjualan)
        {
            try
            {
                return Ok(await service.UpdatePenjualan(id, penjualan));
            }
            catch (Exception ex)
            {
                return Ok(new ErrorMessage(ex.Message));
            }
        }


        [HttpDelete("{id}")]

        public async Task<IActionResult> Delete(int id)
        {
            try
            {

                bool deleted = await service.DeletePenjualan(id);
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
