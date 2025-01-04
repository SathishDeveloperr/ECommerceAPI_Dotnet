using System.Collections.Generic;
using ECommerceAPI.Models;

namespace ECommerceAPI.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int CategoryId { get; set; }
        public int SubCategoryId { get; set; }
        public List<ProductImage> ProductImages { get; set; }
        public float ActualPrice { get; set; }
        public float SellPrice { get; set; }
        public float Discount { get; set; }
        public int Quantity { get; set; }
    }

    public class ProductImage
    {
        public string ImageUrl { get; set; }
        public int ProductId { get; set; }
    }
}
