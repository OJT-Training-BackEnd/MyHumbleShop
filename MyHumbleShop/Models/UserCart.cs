﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyHumbleShop.Models
{
    public class UserCart
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string ProductId { get; set; }
        public string Quantiy { get; set; }
    }
}
