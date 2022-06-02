using AutoMapper;
using MyHumbleShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TikiFake.Dtos.User;

namespace MyHumbleShop
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Users, UserRegisterDto>();
            CreateMap<UserRegisterDto, Users>();
        }
    }
}
