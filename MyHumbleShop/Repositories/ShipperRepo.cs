using AutoMapper;
using MongoDB.Driver;
using MyHumbleShop.DatabaseSettings;
using MyHumbleShop.Dtos.User;
using MyHumbleShop.Models;
using System.Threading.Tasks;

namespace MyHumbleShop.Repositories
{
    public class ShipperRepo : IShipperRepo
    {
        private readonly IMongoCollection<Users> _users;
        private readonly IMongoCollection<Orders> _orders;
        private readonly IMapper _mapper;

        public ShipperRepo(ITakaTikiDatabaseSettings settings, IMapper mapper)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _users = database.GetCollection<Users>(settings.UsersCollectionName);
            _orders = database.GetCollection<Orders>(settings.OrdersCollectionName);   
            _mapper = mapper;
        }

        //method nay dung de thay doi trang thai cua don hang
        //moi lan nguoi dung nhap ShipperId va OrderId, don hang se tu nhay trang thai
        // tu Preparing -> Shipping, Shipping -> Received. Neu da Received -> khong doi Status nua.
        public async Task<ServiceResponse<Orders>> ChangeStatusOrder(string ShipperId, string orderId)
        {
            //Check if account doesn't exist
            var user = await _users.Find(n => n.Id == ShipperId).FirstOrDefaultAsync();
            if (user == null)
            {
                return new ServiceResponse<Orders>()
                {
                    Message = "Account doesn't exist!",
                    Success = false
                };
            }

            //Check if order doesn't exist
            var order = await _orders.Find(e => e.Id == orderId).FirstOrDefaultAsync();
            if (order == null)
            {
                return new ServiceResponse<Orders>()
                {
                    Message = "Order doesn't exist!",
                    Success = false
                };
            }

            //Check if order already received by customer
            if (order.Status == OrderStatus.RECEIVED.ToString())
            {
                return new ServiceResponse<Orders>()
                {
                    Message = "Order already received by customer!",
                    Success = false
                };
            }

            //Return when order is changed to SHIPPING Status
            if (order.Status == OrderStatus.PREPARING.ToString())
            {
                order.Status = OrderStatus.SHIPPING.ToString();
                _orders.ReplaceOne(n => n.Id == orderId, order);
                return new ServiceResponse<Orders>()
                {
                    Success = true,
                    Message = "Order is Shipping now!",
                    Data = order
                };
            }

            //Return when order is changed to RECEIVED Status
            order.Status = OrderStatus.RECEIVED.ToString();
            _orders.ReplaceOne(n => n.Id == orderId, order);
            return new ServiceResponse<Orders>()
            {
                Success = true,
                Message = "Order is received successfully!",
                Data = order
            };
        }

        public async Task<ServiceResponse<Users>> ViewProfile(string id)
        {
            var user = await _users.Find(n => n.Id == id).FirstOrDefaultAsync();
            if (user == null)
            {
                return new ServiceResponse<Users>()
                {
                    Message = "Account doesn't exist!",
                    Success = false
                };
            }

            ShipperDTO shipper = _mapper.Map<ShipperDTO>(user);

            return new ServiceResponse<Users>()
            {
                Data = shipper,
                Message = "Here is your account profile!",
                Success = true
            };
        }
    }
}
