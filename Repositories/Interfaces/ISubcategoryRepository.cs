using ECommerceAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ECommerceAPI.Repositories.Interfaces
{
    public interface ISubcategoryRepository
    {
        Task<bool> CreateSubcategoryAsync(SubCategory subcategory);
        Task<SubCategory> GetSubcategoryByIdAsync(int subcategoryId);
        Task<bool> UpdateSubcategoryAsync(SubCategory subcategory);
        Task<bool> DeleteSubcategoryAsync(int subcategoryId);
        Task<List<SubCategory>> GetAllSubcategoriesAsync();
    }
}
