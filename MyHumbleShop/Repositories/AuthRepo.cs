using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using MyHumbleShop.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TikiFake.DatabaseSettings;

namespace MyHumbleShop.Repositories
{
    public class AuthRepo
    {
        private readonly IMongoCollection<Users> _user;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public AuthRepo(ITakaTikiDatabaseSettings settings,
                        IMapper mapper,
                        IConfiguration configuration)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _user = database.GetCollection<Users>(settings.UsersCollectionName);
            _mapper = mapper;
            _configuration = configuration;
        }

        private string CreateToken(Users user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = System.DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public async Task<ServiceResponse<string>> Login(string username, string password)
        {
            var response = new ServiceResponse<string>();

            if (string.IsNullOrEmpty(username))
            {
                response.Success = false;
                response.Message = "Please enter your password!";
                return response;
            }

            if (string.IsNullOrEmpty(password))
            {
                response.Success = false;
                response.Message = "Please enter your message!";
                return response;
            }

            return response;


        }


    }
}
