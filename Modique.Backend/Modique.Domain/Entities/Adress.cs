using System;

namespace Modique.Domain.Entities
{
    public class Address
    {
        public int AddressId { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }

        public int CityId { get; set; }
        public City? City { get; set; }
        public string Street { get; set; } = string.Empty;
        public string HouseNumber { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
    }
}
