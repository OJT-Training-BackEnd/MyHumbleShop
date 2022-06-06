using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyHumbleShop.Repositories;
using MyHumbleShop.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace MyHumbleShop.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepo _userRepo;
        public UserController(IUserRepo userRepo)
        {
            _userRepo = userRepo;
        }

        [HttpPut("addToCart")]
        public async Task<ActionResult<ServiceResponse<List<Users>>>> addToCart(string productId)
        { 
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            return Ok(await _userRepo.AddToCart(productId, userId));
        }

        [HttpGet("viewCart")]
        public async Task<ActionResult<ServiceResponse<List<UserCart>>>> viewCart()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            return Ok(await _userRepo.ViewCart(userId));
        }

        [HttpPost("saveOrder")]
        public async Task<ActionResult<ServiceResponse<Orders>>> saveOrder(string address, string name, string phone)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            return Ok(await _userRepo.SaveOrder(userId, address, name, phone));
        }
    }
}
