using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SalesInventoryManagement.Domain.Entities
{
    public enum OrderStatus
    {
        Pending,
        Confirmed,
        Shipped,
        Cancelled
    }

    public class Order : BaseEntity
    {
        public int CustomerId { get; set; }
        public Customer Customer { get; set; } = null!;

        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public string? PaymentIntentId { get; set; }
        public string? PaymentStatus { get; set; }
    }
}
