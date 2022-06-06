using MyHumbleShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyHumbleShop.Repositories
{
    public interface IUserRepo
    {
        Task<ServiceResponse<string>> AddToCart(string productId, string userId);
        Task<ServiceResponse<List<UserCart>>> ViewCart(string userId);
    }
}
