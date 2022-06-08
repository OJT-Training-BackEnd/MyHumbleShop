using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyHumbleShop.Models
{
    public class RefreshToken
    {

        public Guid Id { get; set; }

        public int UserId { get; set; }

        public Users Users { get; set; }

        public string Token { get; set; }

        public string JwtId { get; set; }

        public bool IsUsed { get; set; }

        public bool IsRevoked { get; set; }

        public DateTime IssuedAt { get; set; }

        public DateTime ExpiredAt { get; set; }
    }
}
