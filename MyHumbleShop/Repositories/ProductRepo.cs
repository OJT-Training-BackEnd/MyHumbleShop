using AutoMapper;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using MyHumbleShop.DatabaseSettings;
using MyHumbleShop.Dtos.Product;
using MyHumbleShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyHumbleShop.Repositories
{
    public class ProductRepo : IProductRepo
    {
        private readonly IMongoCollection<Products> _products;

        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public ProductRepo(ITakaTikiDatabaseSettings settings,
                        IMapper mapper,
                        IConfiguration configuration)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _products = database.GetCollection<Products>(settings.ProductsCollectionName);
            _mapper = mapper;
            _configuration = configuration;
        }
        public async Task<ServiceResponse<List<Products>>> AddProduct(Products products)
        {
            var serviceResponses = new ServiceResponse<List<Products>>();
            
            if(products == null)
                return new ServiceResponse<List<Products>>()
                    {
                        Success = false,
                        Message = "Please input product"
                    };

            

            var product = await _products.Find(n => n.ProductName == products.ProductName).FirstOrDefaultAsync();
            if (product != null)
                return new ServiceResponse<List<Products>>()
                    {
                        Success = false,
                        Message = "Product already exist"
                    };
               
            
           
            _products.InsertOne(products);

            return new ServiceResponse<List<Products>>()
            {
                Data = products,
                Success = true,
                Message = "Success"
            };
        }

        public async Task<ServiceResponse<List<Products>>> DeleteProduct(string id)
        {
            var dbUser = await _products.Find(s => s.Id == id).FirstOrDefaultAsync();
            if(dbUser == null)
            {
                return new ServiceResponse<List<Products>>()
                {
                    Success = false,
                    Message = "Delete product fail"
                };
                
            }

            if (dbUser.Status == false)
            {
                return new ServiceResponse<List<Products>>()
                {
                    Success = false,
                    Message = "Product Already deleted"
                };
            }

            dbUser.Status = false;
            await _products.ReplaceOneAsync(s => s.Id == id, dbUser);
            return new ServiceResponse<List<Products>>()
            {
                Success = true,
                Message = "Delete product Success"
            };
            


        }

        public async Task<ServiceResponse<List<EditProductDto>>> EditProduct(EditProductDto editProductDto, string id)
        {
            var dbUser = await _products.Find(s => s.Id == id).FirstOrDefaultAsync();
            if(dbUser == null)
            {
                return new ServiceResponse<List<EditProductDto>>()
                {
                    Success = false,
                    Message = "Product does not exist"
                };
            }
            Products productMapper = _mapper.Map<Products>(editProductDto);
            productMapper.Id = dbUser.Id;
            await _products.ReplaceOneAsync(s => s.Id == id, productMapper);

            return new ServiceResponse<List<EditProductDto>>()
            {
                Data = productMapper,
                Success = true,
                Message = "Update product Success"
            };

        }

        public async Task<ServiceResponse<List<Products>>> GetProductAll()
        {
            var serviceResponses = new ServiceResponse<List<Products>>();
            var dbUser = await _products.Find(s => true).ToListAsync();
            serviceResponses.Data = dbUser.ToList();
            serviceResponses.Message = "Get product all success";
            serviceResponses.Success = true;
            return serviceResponses;
        }

        public async Task<ServiceResponse<List<Products>>> GetProductDetails(string id)
        {
            var serviceResponses = new ServiceResponse<List<Products>>();
            var dbUser = await _products.Find(s => s.Id == id).FirstOrDefaultAsync();
            if(dbUser == null)
            {
                return new ServiceResponse<List<Products>>()
                {
                    
                    Success = false,
                    Message = "Get product details fail"
                };
            }
            serviceResponses.Data = dbUser;
            serviceResponses.Message = "Get product details success";
            serviceResponses.Success = true;
            return serviceResponses;
        }
    }
}
