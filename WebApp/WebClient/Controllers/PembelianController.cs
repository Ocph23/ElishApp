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
    public class PembelianController : ControllerBase
    {
        private IPembelianService service;

        public PembelianController(IPembelianService _service)
        {
            service = _service;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                return Ok(await service.GetPembelians());
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
                var result = await service.GetPembelian(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(new ErrorMessage(ex.Message));
            }
        }


        [HttpGet("BySupplierId/{id}")]
        public async Task<IActionResult> GetPembelianBySupplierId(int id)
        {
            try
            {
                return Ok(await service.GetPembeliansBySupplierId(id));
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
                return Ok(await service.CreatePembelian(orderid));
            }
            catch (Exception ex)
            {
                return Ok(new ErrorMessage(ex.Message));
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id,  Pembelian pembelian)
        {
            try
            {
                return Ok(await service.UpdatePembelian(id, pembelian));
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

                bool deleted = await service.DeletePembelian(id);
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
