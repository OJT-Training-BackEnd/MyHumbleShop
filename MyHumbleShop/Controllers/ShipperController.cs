using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyHumbleShop.Dtos.User;
using MyHumbleShop.Models;
using MyHumbleShop.Repositories;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MyHumbleShop.Controllers
{
    [Authorize(Roles = "SHIPPER")]
    [Route("api/[controller]")]
    [ApiController]
    public class ShipperController : ControllerBase
    {
        private readonly IShipperRepo _shipper;

        public ShipperController(IShipperRepo shipper)
        {
            _shipper = shipper;
        }

        [HttpGet("ViewProfile")]
        public async Task<ActionResult<ServiceResponse<ShipperDTO>>> ViewProfile()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var res = await _shipper.ViewProfile(userId);
            if (!res.Success)
                return BadRequest(res);
            return Ok(res);
        }

        [HttpPut("ChangeOrderStatus")]
        public async Task<ActionResult<ServiceResponse<Orders>>> ChangeOrderStatus(string orderId)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var res = await _shipper.ChangeStatusOrder(userId, orderId);
            if (!res.Success)
                return BadRequest(res);
            return Ok(res);
        }
    }
}
