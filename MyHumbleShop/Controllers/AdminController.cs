using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyHumbleShop.Models;
using MyHumbleShop.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MyHumbleShop.Controllers
{
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
        [Authorize]
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
        [Authorize]
        [HttpGet("GetAllUser")]
        public async Task<ActionResult<ServiceResponse<List<Users>>>> GetAllUser()
        {
            var users = await _adminRepo.GetAllUser();
            return Ok(users);
        }
        #endregion
    }
}
