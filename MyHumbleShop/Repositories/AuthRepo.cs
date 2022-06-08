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
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TikiFake.DatabaseSettings;
using TikiFake.Dtos.User;

namespace MyHumbleShop.Repositories
{
    public class AuthRepo : IAuthRepo
    {
        private readonly IMongoCollection<Users> _user;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly List<RefreshToken> _refreshTokens = new List<RefreshToken>();

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
        public async Task<ServiceResponse<string>> Register(UserRegisterDto userDto, string password)
        {
            var response = new ServiceResponse<string>();
            if (UserExists(userDto.Username))
            {
                response.Success = false;
                response.Message = "User already exists.";
                return response;
            }
            var passwordEncode = PasswordEncryption(password);
            userDto.Password = passwordEncode;

            Users user = _mapper.Map<Users>(userDto);
            _user.InsertOne(user);
 
            response.Data = CreateToken(user);
            response.Message = "Register sucessed";
            return response;
        }
        private string PasswordEncryption(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                //send a sample text to hash 
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                //get the hashed string
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }
        public bool UserExists(string username)
        {
            var user = _user.Find(n => n.Username == username).FirstOrDefault();
            if (user == null)
                return false;
            return true;
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
                response.Message = "Please enter your username!";
                return response;
            }

            if (string.IsNullOrEmpty(password))
            {
                response.Success = false;
                response.Message = "Please enter your password!";
                return response;
            }

            var user = _user.Find(s => s.Username.Equals(username)).FirstOrDefault();

            if(user == null)
            {
                response.Success = false;
                response.Message = "User does not exist!";
                return response;
            }

            var accPassword = PasswordEncryption(password);
            if (accPassword != user.Password)
            {
                response.Message = "Incorrect password";
                return response;
            }

            if (user != null)
            {
                response.Success = true;
                response.Message = "Login succesfully!";
                response.Data = CreateToken(user);
                return response;
            }

            return response;


        }

        public Task<ServiceResponse<List<Users>>> Get()
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<List<Users>>> Get(string id)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAll(int userId)
        {
            _refreshTokens.RemoveAll(p => p.UserId == userId);

            return Task.CompletedTask;
        }
    }
    
}
