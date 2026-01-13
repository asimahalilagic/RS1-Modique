using System;
using System.Collections.Generic;

namespace Modique.Domain.Entities
{
    public class City
    {
        public int CityId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int CountryId { get; set; }
        public Country? Country { get; set; }

        public ICollection<Address> Addresses { get; set; } = new List<Address>();
    }
}
