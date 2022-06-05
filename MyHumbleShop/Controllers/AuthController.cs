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
using TikiFake.Models;
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
        [AllowAnonymous]
        [HttpPost("Register")]
        public async Task<ActionResult<ServiceResponse<UserRegisterDto>>> Register(UserRegisterDto userDto)
        {
            var res = await _authRepo.Register(userDto, userDto.Password);
            if (!res.Success)
                return BadRequest(res);
            return Ok(res);
        }
        [AllowAnonymous]
        [HttpPost("Renew")]
        public async Task<ActionResult<ServiceResponse<List<Users>>>> RenewToken(TokenModel model)
        {
            var response = await _authRepo.RenewToken(model);
            if (!response.Success)
                return BadRequest(response);
            return Ok(response);
        }

        [Authorize]
        [HttpDelete("Logout")]
        public async Task<ActionResult<ServiceResponse<List<Users>>>> Logout()
        {
            string rawUserId = HttpContext.User.FindFirstValue("_id");

            if (rawUserId == null)
            {
                return Unauthorized();
            }


            var result = await _authRepo.Logout(rawUserId);

            return Ok(result);
        }
    }
}
