using MyHumbleShop.Models;
using System.Threading.Tasks;

namespace MyHumbleShop.Repositories
{
    public interface IShipperRepo
    {
        Task<ServiceResponse<Users>> ViewProfile(string id);
        Task<ServiceResponse<Orders>> ChangeStatusOrder(string id, string orderId);
    }
}
