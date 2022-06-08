using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyHumbleShop.Dtos.Product
{
    public class EditProductDto
    {
        public string ProductName { get; set; }
        public string Description { get; set; }
        public string Price { get; set; }
        public string Quantity { get; set; }

        public bool Status { get; set; }

        public string Category { get; set; }
    }
}
