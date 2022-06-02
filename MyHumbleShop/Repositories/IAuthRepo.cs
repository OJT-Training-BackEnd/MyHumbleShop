using MyHumbleShop.Controllers;
using MyHumbleShop.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
namespace MyHumbleShop.Repositories
{
    public interface IAuthRepo
    {
        Task<ServiceResponse<List<Users>>> Get();

        Task<ServiceResponse<List<Users>>> Get(string id);

        Task<ServiceResponse<string>> Login(string username, string password);
    }
}
