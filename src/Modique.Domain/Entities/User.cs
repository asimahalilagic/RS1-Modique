using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modique.Domain.Entities
{
    public class User
    {
        public int UserId { get; set; }
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Email { get; set; } = "";
        public string PasswordHash { get; set; } = "";
        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;
        public bool Active { get; set; } = true;

        public int RoleId { get; set; }
        public Role? Role { get; set; }
    }
}
