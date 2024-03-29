﻿using Microsoft.AspNetCore.Mvc;
using ShareModels;
using System;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ApsWebApp.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class PenjualanController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IPenjualanService service;

        public PenjualanController(IPenjualanService _service, IUserService userService)
        {
            _userService = userService;
            service = _service;
        }

        [ApiAuthorize]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                if (User.InRole("Sales") || User.IsInRole("Customer"))
                {
                    var profile = await _userService.Profile();
                    if (profile != null)
                    {
                        if (User.InRole("Sales"))
                        {
                            Karyawan karyawan = profile as Karyawan;
                            return Ok((await service.GetPenjualansBySalesId(karyawan.Id)).ToList());
                        }

                        if (User.InRole("Customer"))
                        {
                            Customer karyawan = profile as Customer;
                            return Ok(await service.GetPenjualansByCustomerId(karyawan.Id));
                        }
                    }
                }
                var result = await service.GetPenjualans();
                return Ok(result.ToList());
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorMessage(ex.Message));
            }
        }

        [ApiAuthorize]
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
                return BadRequest(new ErrorMessage(ex.Message));
            }
        }


        [ApiAuthorize]
        [HttpPost("{orderid}")]
        public async Task<IActionResult> Post(int orderid, Penjualan model)
        {
            try
            {
                return Ok(await service.CreatePenjualan(orderid, model));
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorMessage(ex.Message));
            }
        }

        [ApiAuthorize]
        [HttpPost("order")]
        public async Task<IActionResult> PostOrder(OrderPenjualan order)
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

        [ApiAuthorize]
        [HttpGet("order")]
        public async Task<IActionResult> GetOrders()
        {
            try
            {
                if (User.InRole("Sales") || User.InRole("Customer"))
                
                {
                    var profile = await _userService.Profile();
                    if (profile != null)
                    {

                        if (User.InRole("Sales"))
                        {
                            Karyawan karyawan = profile as Karyawan;
                            return Ok(await service.GetOrdersBySalesId(karyawan.Id));
                        }

                        if (User.InRole("Customer"))
                        {
                            Customer karyawan = profile as Customer;
                            return Ok(await service.GetOrdersByCustomerId(karyawan.Id));
                        }
                    }
                }

                return Ok(await service.GetOrders());

            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorMessage(ex.Message));
            }
        }

        [ApiAuthorize]
        [HttpGet("order/{id}")]
        public async Task<IActionResult> GetOrder(int id)
        {
            try
            {
                var result = await service.GetOrder(id);
                if (result != null)
                    return Ok(result);
                throw new SystemException("Order Not Found !");
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorMessage(ex.Message));
            }
        }


        [ApiAuthorize]
        [HttpGet("pembayaranbypenjualanid/{id}")]
        public async Task<IActionResult> GetPembayaranByPenjualanId(int id)
        {
            try
            {
                var result = await service.GetPembayaran(id);
                if (result != null)
                    return Ok(result);
                throw new SystemException("Order Not Found !");
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorMessage(ex.Message));
            }
        }

        [ApiAuthorize]
        [HttpPost("cretaepembayaran/{id}")]
        public async Task<IActionResult> CreatePembayaran(int id, PembayaranPenjualan model)
        {
            try
            {
                return Ok(await service.CreatePembayaran(id, model, true));
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorMessage(ex.Message));
            }
        }


        [ApiAuthorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, Penjualan penjualan)
        {
            try
            {
                return Ok(await service.UpdatePenjualan(id, penjualan));
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorMessage(ex.Message));
            }
        }

        [ApiAuthorize]
        [HttpPut("order/{id}")]
        public async Task<IActionResult> UpdateOrder(int id, OrderPenjualan penjualan)
        {
            try
            {
                return Ok(await service.UpdateOrder(id, penjualan));
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorMessage(ex.Message));
            }
        }

        [ApiAuthorize]
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
                return BadRequest(new ErrorMessage(ex.Message));
            }
        }


    }
}
