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
            
            var response = new ServiceResponse<List<ProductByCategoryDto>>();
            var productByCategoryDto = new ProductByCategoryDto();
            if (string.IsNullOrEmpty(category))
            {
                response.Message = "This category is not exist!";
                response.Data = response;
                return response;
            }
            var categoryName = _category.Find(x => x.Id.Equals(category)).FirstOrDefault();
            var listProductByCategory = await _product.FindAsync(product => product.Category.Equals(category));
          
            //var products = _mapper.Map<Products>(listProductByCategory);

            response.Message = $"List product by category {categoryName.Name}";
            response.Data = listProductByCategory.ToList();
            response.Success = true;

            return response;
        }

        public async Task<ServiceResponse<List<Products>>> SearchByProductName(string productName)
        {
            var response = new ServiceResponse<List<Products>>();
            if (string.IsNullOrEmpty(productName))
            {
                response.Message = "This product is not exist!";
                response.Data = response;
                return response;
            }

            // Create indexes
            var indexKeysDefinition = Builders<Products>.IndexKeys.Text(x => x.ProductName);
            await _product.Indexes.CreateOneAsync(new CreateIndexModel<Products>(indexKeysDefinition));
            
            // Search full text
            var productListSearchFullTextName = _product.Find(Builders<Products>.Filter.Text(productName)).ToList();

            response.Message = "List product by name";
            response.Data = productListSearchFullTextName;
            return response;
        }

    }
}
