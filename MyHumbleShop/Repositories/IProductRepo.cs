using MyHumbleShop.Dtos.Product;
using MyHumbleShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyHumbleShop.Repositories
{
    public interface IProductRepo
    {
        Task<ServiceResponse<List<Products>>> AddProduct(Products products);

        Task<ServiceResponse<List<EditProductDto>>> EditProduct(EditProductDto editProductDto, string id);

        Task<ServiceResponse<List<Products>>> DeleteProduct(string id);

        Task<ServiceResponse<List<Products>>> GetProductDetails(string id);

        Task<ServiceResponse<List<Products>>> GetProductAll();

        Task<List<Products>> ListProduct();

        Task<ServiceResponse<List<Products>>> ReadFileExcelProduct();
    }
}
