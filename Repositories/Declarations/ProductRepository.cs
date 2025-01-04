using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using ECommerceAPI.Data;
using ECommerceAPI.Models;
using ECommerceAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ECommerceAPI.Repositories.Declarations
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext dbContext;

        public ProductRepository(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<bool> CreateProductAsync(Product product)
        {
            try
            {
                using var connection = dbContext.Database.GetDbConnection();
                var parameters = new DynamicParameters();
                parameters.Add("@ProductName", product.ProductName);
                parameters.Add("@CategoryId", product.CategoryId);
                parameters.Add("@SubCategoryId", product.SubCategoryId);
                parameters.Add("@ProdImg", product.ProductImages);
                parameters.Add("@ActualPrice", product.ActualPrice);
                parameters.Add("@SellPrice", product.SellPrice);
                parameters.Add("@Discount", product.Discount);
                parameters.Add("@Quantity", product.Quantity);

                var result = await connection.ExecuteAsync("sp_AddProduct", parameters, commandType: CommandType.StoredProcedure);

                return result > 0;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public async Task<bool> DeleteProductAsync(int ProductId)
        {
            using var connection = dbContext.Database.GetDbConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@ProductId", ProductId);

            var rowsAffected = await connection.ExecuteAsync(
                "sp_DeleteProduct",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return rowsAffected > 0;
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            using var connection = dbContext.Database.GetDbConnection();
            var result = await connection.QueryAsync<Product>(
                "sp_GetAllProducts",
                commandType: CommandType.StoredProcedure
            );

            return result.AsList();
        }

        public async Task<Product> GetProductByIdAsync(int ProductId)
        {
            using var connection = dbContext.Database.GetDbConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@ProductId", ProductId);

            return await connection.QuerySingleOrDefaultAsync<Product>(
                "sp_GetProductById",
                parameters,
                commandType: CommandType.StoredProcedure);
        }

        public async Task<bool> UpdateProductAsync(Product product)
        {
            try
            {
                using var connection = dbContext.Database.GetDbConnection();
                var parameters = new DynamicParameters();
                parameters.Add("@ProductId", product.ProductId);
                parameters.Add("@ProductName", product.ProductName);
                parameters.Add("@CategoryId", product.CategoryId);
                parameters.Add("@SubCategoryId", product.SubCategoryId);
                parameters.Add("@ProdImg", product.ProductImages);
                parameters.Add("@ActualPrice", product.ActualPrice);
                parameters.Add("@SellPrice", product.SellPrice);
                parameters.Add("@Discount", product.Discount);
                parameters.Add("@Quantity", product.Quantity);

                var result = await connection.ExecuteAsync("sp_UpdateProduct", parameters, commandType: CommandType.StoredProcedure);

                return result > 0;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
