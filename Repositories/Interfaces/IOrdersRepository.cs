using ECommerceAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ECommerceAPI.Repositories.Interfaces
{
    public interface IOrdersRepository
    {
        Task<bool> CreateOrdersAsync(Orders Orders);
        Task<Orders> GetOrdersByIdAsync(int OrderId);
        Task<bool> DeleteOrderAsync(int OrderId);
        Task<List<Orders>> GetAllOrdersAsync();
    }
}
