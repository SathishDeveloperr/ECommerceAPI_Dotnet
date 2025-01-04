using ECommerceAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ECommerceAPI.Repositories.Interfaces
{
    public interface IWhishlistRepository
    {
        Task<bool> AddToWhishListAsync(WhishList whishList);
        Task<List<WhishList>> GetWhishListsByUserIdAsync(int userId);
        Task<bool> RemoveFromWhishListAsync(int whishListId, int userId);
    }
}
