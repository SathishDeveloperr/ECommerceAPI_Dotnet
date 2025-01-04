using ECommerceAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ECommerceAPI.Repositories.Interfaces
{
    public interface IProductRepository
    {
        Task<bool> CreateProductAsync(Product Product);
        Task<Product> GetProductByIdAsync(int ProductId);
        Task<bool> UpdateProductAsync(Product Product);
        Task<bool> DeleteProductAsync(int ProductId);
        Task<List<Product>> GetAllProductsAsync();
    }
}
