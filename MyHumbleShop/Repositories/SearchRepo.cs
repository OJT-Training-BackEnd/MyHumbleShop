using AutoMapper;
using MongoDB.Driver;
using MyHumbleShop.DatabaseSettings;
using MyHumbleShop.Dtos.Product;
using MyHumbleShop.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyHumbleShop.Repositories
{
    public class SearchRepo : ISearchRepo
    {
        private readonly IMongoCollection<Categories> _category;
        private readonly IMongoCollection<Products> _product;
        private readonly IMapper _mapper;


        public SearchRepo(ITakaTikiDatabaseSettings settings,
                          IMapper mapper)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _category = database.GetCollection<Categories>(settings.CategoriesCollectionName);
            _product = database.GetCollection<Products>(settings.ProductsCollectionName);
            _mapper = mapper;
        }

        public async Task<ServiceResponse<List<ProductByCategoryDto>>> SearchByCategory(string category)
        {
            

            if (string.IsNullOrEmpty(category))
                return new ServiceResponse<List<ProductByCategoryDto>>()
                {
                    Message = "This category is not exist",
                    Success = false
                };
            

            var categoryName = _category.Find(x => x.Id.Equals(category)).FirstOrDefault();
            var listProductByCategory =  _product.Find(product => product.Category.Equals(category)).ToList();
            if (listProductByCategory.Count == 0)
                return new ServiceResponse<List<ProductByCategoryDto>>()
                {
                    Message = "This category is not exist",
                    Success = false
                };
            
            
            //var products = _mapper.Map<Products>(listProductByCategory);
            var displayProducts =  _mapper.Map<List<ProductByCategoryDto>>(listProductByCategory);


            return new ServiceResponse<List<ProductByCategoryDto>>()
            {
                Message = $"List product by category {categoryName.Name}",
                Data = displayProducts,
                Success = true
            };
        }

        public async Task<ServiceResponse<List<Products>>> SearchByProductName(string productName)
        {
            if (string.IsNullOrEmpty(productName))
                return new ServiceResponse<List<Products>>()
                {
                    Message = "Please enter product name",
                    Success = false
                };

            // Create indexes
            var indexKeysDefinition = Builders<Products>.IndexKeys.Text(x => x.ProductName);
            await _product.Indexes.CreateOneAsync(new CreateIndexModel<Products>(indexKeysDefinition));
            
            // Search full text
            var productListSearchFullTextName = _product.Find(Builders<Products>.Filter.Text(productName)).ToList();
            if (productListSearchFullTextName.Count == 0)
                return new ServiceResponse<List<Products>>()
                {
                    Message = "This product is not exist",
                    Success = false
                };

            
            return new ServiceResponse<List<Products>>()
            {
                Message = "List product by name",
                Success = true,
                Data = productListSearchFullTextName
            };
        }

    }
}
