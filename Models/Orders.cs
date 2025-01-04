using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace ECommerceAPI.Models
{
    public class Orders
    {
        public int OrderId {get;set;}
        public int UserId {get;set;}
        public int ProductId { get;set;}
        public string OrderDate { get; set;}
        public string Address { get; set;}
        public int Status { get; set;}
        public int Quantity { get; set;}
    }
}
