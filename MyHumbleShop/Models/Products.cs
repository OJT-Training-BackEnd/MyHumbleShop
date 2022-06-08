using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyHumbleShop.Models
{
    public class Products
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonElement("productName")]
        public string ProductName { get; set; }
        [BsonElement("description")]
        public string Description { get; set; }
        [BsonElement("price")]
        public string Price { get; set; }
        [BsonElement("status")]
        public bool Status { get; set; } = true;
        [BsonElement("quantity")]
        public string Quantity { get; set; }
        [BsonElement("categoryID")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Category { get; set; }
    }
}
