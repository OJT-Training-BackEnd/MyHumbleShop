using AutoMapper;
using MyHumbleShop.Dtos.Product;
using MyHumbleShop.Dtos.User;
using MyHumbleShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyHumbleShop.Dtos.User;

namespace MyHumbleShop
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Users, UserRegisterDto>();
            CreateMap<UserRegisterDto, Users>();
            CreateMap<Products, ProductByCategoryDto>();
            CreateMap<ProductByCategoryDto, Products>();
            CreateMap<Products,EditProductDto>();
            CreateMap<EditProductDto, Products>();
            CreateMap<Users, UserProfileDto>();
            CreateMap<UserProfileDto, Users>();
            CreateMap<Users, ShipperDTO>();
            CreateMap<ShipperDTO, Users>();
        }
    }
}
