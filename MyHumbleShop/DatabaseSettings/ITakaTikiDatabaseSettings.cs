using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TikiFake.DatabaseSettings
{
     public interface ITakaTikiDatabaseSettings
    {
        public string UsersCollectionName { get; set; }
        public string CategoriesCollectionName { get; set; }
        public string OrdersCollectionName { get; set; }
        public string ProductsCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}
