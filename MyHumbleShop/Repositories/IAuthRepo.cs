using System.Threading.Tasks;
using TikiFake.Dtos.User;

namespace MyHumbleShop.Repositories
{
    public interface IAuthRepo
    {
        Task<ServiceResponse<string>> Register(UserRegisterDto user, string password);
        bool UserExists(string username);
    }
}
