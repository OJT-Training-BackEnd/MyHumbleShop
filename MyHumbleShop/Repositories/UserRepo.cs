using AutoMapper;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using MyHumbleShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using MyHumbleShop.DatabaseSettings;
using MyHumbleShop.Dtos.User;

namespace MyHumbleShop.Repositories
{
    public class UserRepo : IUserRepo
    {
        private readonly IMongoCollection<Users> _user;
        private readonly IMongoCollection<Products> _products;
        private readonly IMongoCollection<Orders> _orders;
        private readonly IMongoCollection<RefreshToken> _refreshToken;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly List<RefreshToken> refreshTokens = new List<RefreshToken>();

        public UserRepo(ITakaTikiDatabaseSettings settings,
                        IMapper mapper,
                        IConfiguration configuration)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _user = database.GetCollection<Users>(settings.UsersCollectionName);
            _products = database.GetCollection<Products>(settings.ProductsCollectionName);
            _refreshToken = database.GetCollection<RefreshToken>(settings.RefreshTokensCollectionName);
            _orders= database.GetCollection<Orders>(settings.OrdersCollectionName);
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<ServiceResponse<string>> AddToCart(string productId, string userId)
        {
            var response = new ServiceResponse<string>();
            var user = await _user.Find(n => n.Id == userId).FirstOrDefaultAsync();
            var userRefresh = _refreshToken.Find(n => n.UserId == userId).FirstOrDefault();

            if (user == null)
            {
                response.Success = false;
                response.Message = "User not found";
                return response;
            }
            if (userRefresh == null)
            {
                response.Success = false;
                response.Message = "Token is expired";
                return response;
            }

            var productExist = await _products.Find(n => n.Id == productId).FirstOrDefaultAsync();
            if (productExist == null)
            {
                response.Success = false;
                response.Message = "Product not found";
                return response;
            }

            if (productExist.Status == false)
            {
                return new ServiceResponse<string>()
                {
                    Success = false,
                    Message = "The Product is sold out!"
                };
            }

            if (user.Cart == null)
            {
                user.Cart = new List<UserCart>();
            }

            foreach (var pro in user.Cart)
            {
                if (pro.ProductId == productId)
                {
                    if (int.Parse(pro.Quantiy) == int.Parse(productExist.Quantity))
                    {
                        return new ServiceResponse<string>
                        {
                            Success = false,
                            Message = "This product's quantity has been maximum!"
                        };
                    }

                    pro.Quantiy = (int.Parse(pro.Quantiy) + 1).ToString();
                    _user.ReplaceOne(n => n.Id == user.Id, user);
                    response.Success = true;
                    response.Message = "Add product existed in cart";
                    return response;
                }
            }

            UserCart cart = new UserCart 
            {
                ProductId = productId,
                Quantiy = "1"
            };
            user.Cart.Add(cart);
            _user.ReplaceOne(n => n.Id == user.Id, user);
            response.Success = true;
            response.Message = "Add new product to cart";
            return response;
        }

        public async Task<ServiceResponse<string>> SaveOrder(string userId, string address, string customerName, string customerPhone)
        {
            var response = new ServiceResponse<string>();
            var user = await _user.Find(n => n.Id == userId).FirstOrDefaultAsync();
            var userRefresh = _refreshToken.Find(n => n.UserId == userId).FirstOrDefault();
            if (user == null)
            {
                response.Success = false;
                response.Message = "User not found";
                return response;
            }
            if (userRefresh == null)
            {
                response.Success = false;
                response.Message = "Token is expired";
                return response;
            }
            if (user.Cart == null)
            {
                response.Success = false;
                response.Message = "You have no item in the cart";
                return response;
            }

            int totalPrice = 0;
            foreach (var orderDetail in user.Cart)
            {
                var product = await _products.Find(n => n.Id == orderDetail.ProductId).FirstOrDefaultAsync();
                totalPrice += int.Parse(product.Price) * int.Parse(orderDetail.Quantiy);
            }

            List<OrderDetail> orderDetails = new List<OrderDetail>();
            foreach (var userCart in user.Cart)
            {
                var product = await _products.Find(n => n.Id == userCart.ProductId).FirstOrDefaultAsync();
                product.Quantity = (int.Parse(product.Quantity) - int.Parse(userCart.Quantiy)).ToString();
                if (int.Parse(product.Quantity) == 0)
                {
                    product.Status = false;
                }
                _products.ReplaceOne(n => n.Id == product.Id, product);

                OrderDetail odt = new OrderDetail
                {
                    ProductId = userCart.ProductId,
                    ProductName = product.ProductName,
                    ProductPrice = product.Price,
                    Quantiy = userCart.Quantiy
                };
                orderDetails.Add(odt);
            }

            Orders order = new Orders
            {
                CustomerId = userId,
                DateOrder = DateTime.Now,
                TotalPrice = totalPrice.ToString(),
                OrderDetails = orderDetails,
                ShippingAddress = address,
                CustomerName = customerName,
                CustomerPhone = customerPhone
            };
            _orders.InsertOne(order);
            user.Cart = null;
            _user.ReplaceOne(n => n.Id == user.Id, user);
            response.Success = true;
            response.Message = "Order is being prepared";
            response.Data = order;
            return response;
        }

        public async Task<ServiceResponse<List<UserCart>>> ViewCart(string userId)
        {
            var response = new ServiceResponse<List<UserCart>>();
            var user = await _user.Find(n => n.Id == userId).FirstOrDefaultAsync();
            if (user == null)
            {
                response.Success = false;
                response.Message = "User not found";
                return response;
            }

            if (user.Cart == null)
            {
                response.Success = false;
                response.Message = "You have no item in the cart";
                return response;
            }

            response.Data = user.Cart;
            response.Success = true;
            response.Message = "Here is your cart";
            return response;
        }
        public async Task<ServiceResponse<UserProfileDto>> GetUserProfile(string id)
        {
            var serviceResponses = new ServiceResponse<UserProfileDto>();
            var dbUser = await _user.Find(s => s.Id == id).FirstOrDefaultAsync();
            var displayUser = _mapper.Map<UserProfileDto>(dbUser);
            serviceResponses.Data = displayUser;
            serviceResponses.Message = "This is your profile";
            return serviceResponses;
        }
       
    }
}
