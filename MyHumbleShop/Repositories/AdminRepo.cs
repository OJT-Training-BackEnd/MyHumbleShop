using AutoMapper;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using MyHumbleShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyHumbleShop.DatabaseSettings;

namespace MyHumbleShop.Repositories
{
    public class AdminRepo:IAdminRepo
    {
        private readonly IMongoCollection<Users> _users;

        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public AdminRepo(ITakaTikiDatabaseSettings settings,
                        IMapper mapper,
                        IConfiguration configuration)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _users = database.GetCollection<Users>(settings.UsersCollectionName);
            _mapper = mapper;
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
            var dbUser = await _users.Find(s => s.Role == "CUSTOMER").ToListAsync();

            serviceResponses.Data = dbUser.ToList();
            return serviceResponses;
        }
    }
}
