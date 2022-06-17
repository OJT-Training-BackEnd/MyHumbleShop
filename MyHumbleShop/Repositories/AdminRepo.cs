using AutoMapper;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using MyHumbleShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyHumbleShop.DatabaseSettings;
using MyHumbleShop.Dtos.User;

namespace MyHumbleShop.Repositories
{
    public class AdminRepo: IAdminRepo
    {
        private readonly IMongoCollection<Users> _users;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IAuthRepo _authRepo;

        public AdminRepo(ITakaTikiDatabaseSettings settings,
                        IMapper mapper,
                        IConfiguration configuration,
                        IAuthRepo auth)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _users = database.GetCollection<Users>(settings.UsersCollectionName);
            _mapper = mapper;
            _authRepo = auth;
            _configuration = configuration;
        }

        public async Task<ServiceResponse<List<Users>>> UnabledAccount(string id)
        {
            var serviceResponses = new ServiceResponse<List<Users>>();
            var dbUser = await _users.Find(s => s.Id == id).FirstOrDefaultAsync();
            if (dbUser != null)
            {
                dbUser.Status = false;
                _users.ReplaceOne(n => n.Id == id, dbUser);

                serviceResponses.Success = true;
                serviceResponses.Message = "Ok";
                serviceResponses.Data = dbUser;
                return serviceResponses;
            }
            serviceResponses.Message = "Fail";
            serviceResponses.Data = dbUser;
            return serviceResponses;
        }

        public async Task<ServiceResponse<Users>> GetDetailsUser(string id)
        {
            var serviceResponses = new ServiceResponse<Users>();
            var dbUser = await _users.Find(s => s.Id == id).FirstOrDefaultAsync();
            serviceResponses.Data = dbUser;

            return serviceResponses;
        }

        public async Task<ServiceResponse<List<Users>>> GetAllUser()
        {

            var serviceResponses = new ServiceResponse<List<Users>>();
            var dbUser = await _users.Find(s => s.Role == "SHIPPER").ToListAsync();

            serviceResponses.Data = dbUser.ToList();
            return serviceResponses;
        }

        public async Task<ServiceResponse<ShipperDTO>> CreateShipper(ShipperDTO shipper)
        {
            if (_authRepo.UserExists(shipper.Username))
                return new ServiceResponse<ShipperDTO>()
                {
                    Success = false,
                    Message = "Shipper's username already existed!",
                    Data = null
                };

            if (shipper.Phone == null)
                return new ServiceResponse<ShipperDTO>()
                {
                    Success = false,
                    Message = "Shipper must have phone!",
                    Data = null
                };

            if (shipper.CardId == null)
                return new ServiceResponse<ShipperDTO>()
                {
                    Success = false,
                    Message = "Shipper must have identity card!",
                    Data = null
                };

            var passwordEncode = _authRepo.PasswordEncryption(shipper.Password);
            shipper.Password = passwordEncode;

            Users user = _mapper.Map<Users>(shipper);
            user.Role = Roles.SHIPPER.ToString();

            _users.InsertOne(user);

            return new ServiceResponse<ShipperDTO>()
            {
                Data = user,
                Message = "Register shipper successed!",
                Success = true
            };
        }


    }
}
