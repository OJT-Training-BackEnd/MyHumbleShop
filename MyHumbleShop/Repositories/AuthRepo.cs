using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using MongoDB.Driver;
using MyHumbleShop.DatabaseSettings;
using MyHumbleShop.Dtos.User;
using MyHumbleShop.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using MyHumbleShop.DatabaseSettings;
using MyHumbleShop.Dtos.User;
using MyHumbleShop.Models;

namespace MyHumbleShop.Repositories
{
    public class AuthRepo : IAuthRepo
    {
        private readonly IMongoCollection<Users> _user;
        private readonly IMongoCollection<RefreshToken> _refreshToken;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly List<RefreshToken> refreshTokens = new List<RefreshToken>();

        public AuthRepo(ITakaTikiDatabaseSettings settings,
                        IMapper mapper,
                        IConfiguration configuration)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _user = database.GetCollection<Users>(settings.UsersCollectionName);
            _refreshToken = database.GetCollection<RefreshToken>(settings.RefreshTokensCollectionName);
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
        public string PasswordEncryption(string password)
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
        private TokenModel CreateToken(Users user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim("_id", user.Id.ToString()),
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

            var accessToken = tokenHandler.WriteToken(token);
            var refreshToken = GenerateRefreshToken();

            var refreshTokenEntity = new RefreshToken
            {
                RTokenId = Guid.NewGuid(),
                JwtId = token.Id,
                UserId = user.Id,
                Token = refreshToken,
                isUsed = false,
                isRevoked = false,
                IssuedAt = DateTime.Now,
                ExpiredAt = DateTime.Now
            };
            _refreshToken.InsertOne(refreshTokenEntity);
            return new TokenModel
            { 
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
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
            if (user.Status == false)
            {
                response.Success = false;
                response.Message = "Account has been disabled";
                return response;
            }
            if (user == null)
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

       
        private Users getuserbyId(string id)
        {
            var user = _user.Find(s => s.Id.Equals(id)).FirstOrDefault();

            return user;
        }
        private string GenerateRefreshToken()
        {
            var random = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(random);
                return Convert.ToBase64String(random);
            }
        }
        public async Task<ServiceResponse<string>> RenewToken(TokenModel model)
        {
            var response = new ServiceResponse<string>();
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));
            var tokenValidateParam = new TokenValidationParameters
            {
                //ký vào token 
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                //tự cấp token 
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero,
                ValidateLifetime = false //ko kiem tra token het han
            };
            try
            {
                //check 1 : AccessToken Valid format
                var tokenInVerification = tokenHandler.ValidateToken(model.AccessToken, tokenValidateParam, out var validatedToken);
                //check 2: check alg
                if (validatedToken is JwtSecurityToken jwtSecurityToken)
                {
                    var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);
                    if (!result)
                    {
                        response.Success = false;
                        response.Message = "Invalid token";
                        return response;
                    }
                }
                //check 3: check accessToken expire?
                var utcExpireDate = long.Parse(tokenInVerification.Claims.FirstOrDefault(
                    x => x.Type == JwtRegisteredClaimNames.Exp).Value);

                var expireDate = ConvertUnixTimeToDateTime(utcExpireDate);
                if (expireDate > DateTime.UtcNow)
                {
                    response.Success = false;
                    response.Message = "Access token has not yet expired";
                    return response;
                }
                //check 4: check refreshtoken exist in DB
                var storedToken = _refreshToken.Find(x => x.Token == model.RefreshToken).FirstOrDefault();
                if (storedToken == null)
                {
                    response.Success = false;
                    response.Message = "Refresh token does not exist";
                    return response;
                }
                //check 5 : check refresh token is used/revoked?
                if (storedToken.isUsed)
                {
                    response.Success = false;
                    response.Message = "Refresh token has been used";
                    return response;
                }
                if (storedToken.isRevoked)
                {
                    response.Success = false;
                    response.Message = "Refresh token has been revoked";
                    return response;
                }

                //check 6: AccessToken id == JwtId in RefreshToke
                var jti = tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti).Value;
                if (storedToken.JwtId != jti)
                {
                    response.Success = false;
                    response.Message = "Token doesn't match";
                    return response;
                }

                //Update token is used
                storedToken.isRevoked = true;
                storedToken.isUsed = true;
                _refreshToken.ReplaceOne(n => n.Token == model.RefreshToken, storedToken);
                //create new token 
                var user = _user.Find(n => n.Id == storedToken.UserId).FirstOrDefault();
                var token = CreateToken(user);

                response.Success = true;
                response.Message = "Renew token success";
                response.Data = token;
                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Something went wrong";
                return response;
            }
        }

        private DateTime ConvertUnixTimeToDateTime(long utcExpireDate)
        {
            var dateTimeInterval = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTimeInterval.AddSeconds(utcExpireDate).ToUniversalTime();
            return dateTimeInterval;
        }

        public async Task<ServiceResponse<string>> Logout(string userId)
        {
            var response = new ServiceResponse<string>();

            var refresh = _refreshToken.Find(p => p.UserId.Equals(userId)).FirstOrDefault();
            if(refresh != null)
            {
                _refreshToken.DeleteOne(p => p.UserId.Equals(userId));
                response.Success = true;
                response.Message = "Success";
                return response;
            }
            return response;
           

                  
            /*if (result == null) 
            {
                
                response.Success = true;
                response.Message = "Success";

                return response;

            }
            else
            {
                response.Success = false;
                response.Message = "Fail";
                return response;
            }*/

        }
       
        public Task<ServiceResponse<List<Users>>> Get(string id)
        {
            throw new NotImplementedException();
        }
    }
}
