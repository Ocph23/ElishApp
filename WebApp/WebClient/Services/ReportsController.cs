using Microsoft.AspNetCore.Mvc;
using ShareModels;
using ShareModels.ModelViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebClient.Services
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private IProductService _service;

        public ReportsController(IProductService service)
        {
            _service = service;
        }

        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<ReportsController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<ReportsController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ReportsController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
