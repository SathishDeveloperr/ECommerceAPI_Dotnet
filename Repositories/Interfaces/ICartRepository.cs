using ECommerceAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ECommerceAPI.Repositories.Interfaces
{
    public interface ICartRepository
    {
        Task<bool> AddToCartAsync(Cart cart);
        Task<List<Cart>> GetCartsByUserIdAsync(int userId);
        Task<bool> RemoveFromCartAsync(int cartId, int Userid);
    }
}
