using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyHumbleShop.Dtos.User;
using MyHumbleShop.Models;
using MyHumbleShop.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MyHumbleShop.Controllers
{
    [Authorize(Roles = "ADMIN")]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminRepo _adminRepo;
        public AdminController(IAdminRepo adminRepo)
        {
            _adminRepo = adminRepo;
        }
        #region UnableAccount
        
        [HttpPut("UnableAccount/{id}")]
        public async Task<ActionResult<ServiceResponse<Users>>> UnableAccount(string id)
        {
            string rawUserId = HttpContext.User.FindFirstValue("_id");
            if (id != rawUserId)
            {
                return Ok(await _adminRepo.UnabledAccount(id));
            }
            else
            {
                return Unauthorized();
            }
        }
        #endregion
        #region GetAllUser
        [HttpGet("GetAllUser")]
        public async Task<ActionResult<ServiceResponse<List<Users>>>> GetAllUser()
        {
            var users = await _adminRepo.GetAllUser();
            return Ok(users);
        }
        #endregion

        #region
        [HttpPost("CreateShipper")]
        public async Task<ActionResult<ServiceResponse<ShipperDTO>>> CreateShipper(string username, string password, string cardId, string phone, string email)
        {
            var res = await _adminRepo.CreateShipper(new ShipperDTO
            {
                Username = username,
                Password = password,
                CardId = cardId,
                Phone = phone,
                Email = email
            });

            if (!res.Success)
            {
                return BadRequest(res);
            }
            return Ok(res);
        }
        #endregion
    }
}
