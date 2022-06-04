using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyHumbleShop
{
    public class ServiceResponse <T>
    {
        public object Data { get; set; }
        public bool Success { get; set; } = true;
        public string Message { get; set; }
    }
}
