using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MyHumbleShop.Models
{
    public class Ratings
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("userId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; }

        [BsonElement("star")]
        public string Star { get; set; }

        [BsonElement("content")]
        public string Content { get; set; }
    }
}
