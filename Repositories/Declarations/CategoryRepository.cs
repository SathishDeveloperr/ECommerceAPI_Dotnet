using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using ECommerceAPI.Data;
using ECommerceAPI.Models;
using ECommerceAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommerceAPI.Repositories.Declarations
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext dbContext;
        public CategoryRepository(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<bool> CreateCategoryAsync(Category category)
        {
            using var connection = dbContext.Database.GetDbConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@CategoryName", category.CategoryName);
            parameters.Add("@CategoryImage", category.CategoryImage);



            var result = await connection.QuerySingleOrDefaultAsync<Category>("sp_CreateCategory", parameters, commandType: CommandType.StoredProcedure);

            return result != null;
        }


        public async Task<bool> DeleteCategoryAsync(int CategoryId)
        {
            using var connection = dbContext.Database.GetDbConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@CategoryId", CategoryId);

            var rowsAffected = await connection.ExecuteAsync(
                "sp_DeleteCategory",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return rowsAffected > 0;
        }

        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            using var connection = dbContext.Database.GetDbConnection();
            var result = await connection.QueryAsync<Category>(
                "sp_CategoryList", 
                commandType: CommandType.StoredProcedure
            );

            return result.AsList();
        }

        public async Task<Category> GetCategoryByIdAsync(int CategoryId)
        {
            using var connection = dbContext.Database.GetDbConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@CategoryId", CategoryId);

            return await connection.QuerySingleOrDefaultAsync<Category>(
                "sp_GetCategoryById",
                parameters,
                commandType: CommandType.StoredProcedure);
        }

        public async Task<bool> UpdateCategoryAsync(Category Category)
        {
            using var connection = dbContext.Database.GetDbConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@CategoryId", Category.CategoryId);
            parameters.Add("@CategoryName", Category.CategoryName);
            parameters.Add("@CategoryImage", Category.CategoryImage);
            var result = await connection.QuerySingleOrDefaultAsync<Category>("sp_UpdateCategory", parameters, commandType: CommandType.StoredProcedure);
            return result != null;
        }

    }
}
