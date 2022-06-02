using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Collections.Generic;

namespace MyHumbleShop.Models
{
    public class Users
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonElement("username")]
        public string Username { get; set; }
        [BsonElement("password")]
        public string Password { get; set; }
        [BsonElement("email")]
        public string Email { get; set; }
        [BsonElement("phone")]
        public string Phone { get; set; }
        [BsonElement("status")]
        public bool Status { get; set; } = true;
        [BsonElement("role")]
        public string Role { get; set; } = Roles.CUSTOMER.ToString();
        [BsonElement("cart")]
        public List<UserCart> Cart { get; set; }
    }
}
