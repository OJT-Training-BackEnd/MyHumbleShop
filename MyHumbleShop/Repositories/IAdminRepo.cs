using MyHumbleShop.Dtos.User;
using MyHumbleShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyHumbleShop.Repositories
{
    public interface IAdminRepo
    {
        Task<ServiceResponse<List<Users>>> UnabledAccount(string id);
        Task<ServiceResponse<Users>> GetDetailsUser(string id);
        Task<ServiceResponse<List<Users>>> GetAllUser();
        Task<ServiceResponse<ShipperDTO>> CreateShipper(ShipperDTO shipper);
    }
}
