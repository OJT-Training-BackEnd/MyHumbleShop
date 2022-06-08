using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyHumbleShop.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyHumbleShop.Models;
using Microsoft.AspNetCore.Authorization;
using TikiFake.Dtos.User;
using System.Security.Claims;

namespace MyHumbleShop.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepo _authRepo;


        [AllowAnonymous]
        [HttpPost("Login")]

        public async Task<ActionResult<ServiceResponse<List<UserRegisterDto>>>> Login(string username, string password)
        {
            var response = await _authRepo.Login(username, password);
            if (!response.Success)
                return BadRequest(response);
            return Ok(response);
        }

            public AuthController (IAuthRepo authRepo)
            {
                _authRepo = authRepo;
            }

            [HttpPost("Register")]
            public async Task<ActionResult<ServiceResponse<UserRegisterDto>>> Register(UserRegisterDto userDto)
            {
                var res = await _authRepo.Register(userDto, userDto.Password);
                if (!res.Success)
                    return BadRequest(res);
                return Ok(res);
            }
        
        [Authorize]
        [HttpDelete("logout")]
        public async Task<IActionResult> Logout()
        {
            string rawUserId = HttpContext.User.FindFirstValue("Id");

            if (!Int32.TryParse(rawUserId, out int userId))
            {
                return Unauthorized();
            }

            await _authRepo.DeleteAll(userId);

            return NoContent();
        }
    }
}
