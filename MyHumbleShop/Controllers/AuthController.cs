using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyHumbleShop.Repositories;
using MyHumbleShop.Models;
using Microsoft.AspNetCore.Authorization;
using TikiFake.Dtos.User;

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
    }
}
