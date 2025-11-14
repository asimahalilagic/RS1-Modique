using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modique.Domain.Entities
{
    public class ReturnRequest
    {
        public int ReturnRequestId { get; set; }
        public int OrderId { get; set; }
        public Order? Order { get; set; }

        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
        public string Reason { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending"; 
    }
}
