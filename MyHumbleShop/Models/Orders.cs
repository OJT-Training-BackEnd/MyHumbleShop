using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
namespace MyHumbleShop.Models
{
    public class Orders
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonElement("customerID")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string CustomerId { get; set; }
        [BsonElement("status")]
        public string Status { get; set; } = OrderStatus.PREPARING.ToString();
        [BsonElement("date")]
        public DateTime DateOrder { get; set; }
        [BsonElement("order detail")]
        public List<OrderDetail> OrderDetails { get; set; }
        [BsonElement("shipping address")]
        public string ShippingAddress { get; set; }
        [BsonElement("customer name")]
        public string CustomerName { get; set; }
        [BsonElement("customer phone")]
        public string CustomerPhone { get; set; }
    }
}
