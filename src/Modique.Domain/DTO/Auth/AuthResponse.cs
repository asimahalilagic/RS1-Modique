using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modique.Domain.Entities;

namespace Modique.Domain.DTO.Auth
{
    public class AuthResponse
    {
        public string Token { get; set; } = "";
        public DateTime ExpiresAt { get; set; }
        public UserDto User { get; set; }
    }
}
