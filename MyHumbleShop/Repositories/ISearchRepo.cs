using MyHumbleShop.Dtos.Product;
using MyHumbleShop.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyHumbleShop.Repositories
{
    public interface ISearchRepo
    {
        Task<ServiceResponse<List<ProductByCategoryDto>>> SearchByCategory(string category);

        Task<ServiceResponse<List<Products>>> SearchByProductName(string productnName);
    }
}
