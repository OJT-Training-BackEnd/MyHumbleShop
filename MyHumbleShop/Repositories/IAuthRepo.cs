using MyHumbleShop.Controllers;
using MyHumbleShop.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
﻿using System.Threading.Tasks;
using TikiFake.Dtos.User;
using TikiFake.Models;

namespace MyHumbleShop.Repositories
{
    public interface IAuthRepo
    {

        Task<ServiceResponse<List<Users>>> Get();

        Task<ServiceResponse<List<Users>>> Get(string id);

        Task<ServiceResponse<string>> Login(string username, string password);

        Task<ServiceResponse<string>> Register(UserRegisterDto user, string password);
        Task<ServiceResponse<string>> RenewToken(TokenModel model);
        bool UserExists(string username);

    }
}
