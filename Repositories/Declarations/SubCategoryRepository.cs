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
    public class SubcategoryRepository : ISubcategoryRepository
    {
        private readonly AppDbContext dbContext;

        public SubcategoryRepository(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<bool> CreateSubcategoryAsync(SubCategory subcategory)
        {
            using var connection = dbContext.Database.GetDbConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@SubcategoryName", subcategory.SubCategoryName);
            parameters.Add("@CategoryId", subcategory.CategoryId);
            parameters.Add("@SubCategoryImage", subcategory.SubCategoryImage);
            var result = await connection.QuerySingleOrDefaultAsync<SubCategory>("sp_CreateSubcategory", parameters, commandType: CommandType.StoredProcedure);

            return result != null;
        }

        public async Task<bool> DeleteSubcategoryAsync(int subcategoryId)
        {
            using var connection = dbContext.Database.GetDbConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@SubcategoryId", subcategoryId);

            var rowsAffected = await connection.ExecuteAsync(
                "sp_DeleteSubcategory",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return rowsAffected > 0;
        }

        public async Task<List<SubCategory>> GetAllSubcategoriesAsync()
        {
            using var connection = dbContext.Database.GetDbConnection();
            var result = await connection.QueryAsync<SubCategory>(
                "sp_SubcategoryList",
                commandType: CommandType.StoredProcedure
            );

            return result.AsList();
        }

        public async Task<SubCategory> GetSubcategoryByIdAsync(int subcategoryId)
        {
            using var connection = dbContext.Database.GetDbConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@SubcategoryId", subcategoryId);

            return await connection.QuerySingleOrDefaultAsync<SubCategory>(
                "sp_GetSubcategoryById",
                parameters,
                commandType: CommandType.StoredProcedure);
        }

        public async Task<bool> UpdateSubcategoryAsync(SubCategory subcategory)
        {
            using var connection = dbContext.Database.GetDbConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@SubcategoryId", subcategory.SubCategoryId);
            parameters.Add("@SubcategoryName", subcategory.SubCategoryName);
            parameters.Add("@SubCategoryImage", subcategory.SubCategoryImage);
            parameters.Add("@CategoryId", subcategory.CategoryId);
            var result = await connection.QuerySingleOrDefaultAsync<SubCategory>("sp_UpdateSubcategory", parameters, commandType: CommandType.StoredProcedure);
            return result != null;
        }
    }
}
