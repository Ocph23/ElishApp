using Microsoft.AspNetCore.Mvc;
using ShareModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApsWebApp.Services;
using ClosedXML.Excel;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ApsWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderPembelianController : ControllerBase
    {
        private IPembelianService service;
        private IEmailService emailService;
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _env;

        public OrderPembelianController(IPembelianService _service, IEmailService _emailService, 
            Microsoft.AspNetCore.Hosting.IHostingEnvironment env)
        {
            service = _service;
            emailService = _emailService;
            _env = env;
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


        [HttpGet("sendtosupplier/{id}")]
        public async Task<ActionResult> SendToSupplierOrderPembelian(int id)
        {
            try
            {

              

                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                var logo = _env.ContentRootPath + "/images/apslogo.png";
                var pembelian = await service.GetOrder(id);
                var content = Helper.CreateFileOrderPembelian(contentType, pembelian, logo);
                var base64 = Convert.ToBase64String(content);
                await emailService.SendEmailAsync(new Dictionary<string, string> { { pembelian.Supplier.Email, pembelian.Supplier.Nama } }, $"Order Pembelian : {pembelian.Nomor}",
                    string.IsNullOrEmpty(pembelian.Discription) ? "" : pembelian.Discription ?? "", new List<Attachment>() { new Attachment { ContentType = contentType, 
                        Filename = $"Order Pembelian No : {pembelian.Nomor}.xlsx", Base64Content = base64 } });
                return Ok(base64);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

    }
}
