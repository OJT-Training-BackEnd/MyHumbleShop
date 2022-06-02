using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyHumbleShop.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TikiFake.Dtos.User;

namespace MyHumbleShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepo _authRepo;
        public AuthController(IAuthRepo authRepo)
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
    }
}
