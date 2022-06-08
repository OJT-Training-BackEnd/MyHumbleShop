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
        private readonly IProductRepo _productRepo;
        public UserController(IUserRepo userRepo, IProductRepo productRepo)
        {
            _userRepo = userRepo;
            _productRepo = productRepo;
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
        [Authorize]

        //Get User by id
        [HttpGet("GetProfile")]
        public async Task<ActionResult<ServiceResponse<Users>>> Get()
        {
            string rawUserId = HttpContext.User.FindFirstValue("_id");

            return Ok(await _userRepo.GetUserProfile(rawUserId));
        }
       
    }
}
