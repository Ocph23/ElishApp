using Microsoft.AspNetCore.Mvc;
using ShareModels;
using System.Threading.Tasks;
using WebClient.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebClient.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IncomingCheckController : ControllerBase
    {
        private IIncommingService _service;

        public IncomingCheckController(IIncommingService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult>  Get()
        {
           return Ok(await _service.Load(true));
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            return Ok(await _service.CreateNew(id));
        }


    }
}
