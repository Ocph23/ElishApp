using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using WebClient.Models;
using ShareModels;
using WebClient.Services;

namespace WebClient.Api
{
    [Route("api/[controller]")]
    [ApiController]

    public class UserController : ControllerBase
    {
        private IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }


        [HttpPost("Login")]
        public async Task<IActionResult> Login(UserLogin user)
        {
            try
            {
                var response = await _userService.Authenticate(user);
                if (response == null)
                    return BadRequest(new { message = "Username or password is incorrect" });
                return Ok(response);
            }
            catch (System.Exception ex)
            {
                return BadRequest(new ErrorMessage(ex.Message));
            }
        }

        [ApiAuthorize(Roles = "Administrator, Admin")]
        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterModel user)
        {
            try
            {
                var response = await _userService.Register(user);
                if (response==null)
                    return BadRequest(new { message = "Register User Gagal !" });
                return Ok(response);
            }
            catch (System.Exception ex)
            {
                return BadRequest(new ErrorMessage(ex.Message));
            }
        }



        [ApiAuthorize(Roles ="Administrator, Admin")]
        [HttpPost("AddRoleToUser/{userid}/role")]
        public async Task<IActionResult> AddRole(int userid, string role)
        {
            try
            {
                User user= await _userService.FindUserById(userid);

                if (user == null)
                    throw new System.SystemException("User Not Found !");
                await _userService.AddUserRole(role, user);
                return Ok();
            }
            catch (System.Exception ex)
            {
                return BadRequest(new ErrorMessage(ex.Message));
            }
        }

    }
}