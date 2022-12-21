using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.Collections.Generic;

namespace Pustok.Models
{
    public class AppUser:IdentityUser
    {
        public string Fullname { get; set; }
        public List<BasketItem> BasketItems { get; set; } = new List<BasketItem>();

        public string ConnectionId { get; set; }
        public DateTime LastConnectedAt { get; set; }

    }
}
