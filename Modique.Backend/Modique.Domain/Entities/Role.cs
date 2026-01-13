using System;
using System.Collections.Generic;

namespace Modique.Domain.Entities
{
    public class Role
    {
        public int RoleId { get; set; }
        public string Name { get; set; } = "";
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
