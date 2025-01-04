using ECommerceAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ECommerceAPI.Repositories.Interfaces
{
    public interface ICategoryRepository
    {
        Task<bool> CreateCategoryAsync(Category Category);
        Task<Category> GetCategoryByIdAsync(int CategoryId);
        Task<bool> UpdateCategoryAsync(Category Category);
        Task<bool> DeleteCategoryAsync(int CategoryId);
        Task<List<Category>> GetAllCategoriesAsync();
    }
}
