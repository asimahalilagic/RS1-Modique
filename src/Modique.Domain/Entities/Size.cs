using System;

namespace Modique.Domain.Entities
{
    public class Size
    {
        public int SizeId { get; set; }
        public string Code { get; set; } = ""; 
        public string? Description { get; set; }
    }
}
