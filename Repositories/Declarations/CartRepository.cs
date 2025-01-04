using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using ECommerceAPI.Data;
using ECommerceAPI.Models;
using ECommerceAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommerceAPI.Repositories.Declarations
{
    public class CartRepository : ICartRepository
    {
        private readonly AppDbContext dbContext;

        public CartRepository(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<bool> AddToCartAsync(Cart cart)
        {
            using var connection = dbContext.Database.GetDbConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@ProductId", cart.ProductId);
            parameters.Add("@Quantity", cart.Quantity);
            parameters.Add("@UserId", cart.UserId);

            var result = await connection.QuerySingleOrDefaultAsync<Category>("sp_AddCart", parameters, commandType: CommandType.StoredProcedure);

            return result != null;
        }

        public async Task<List<Cart>> GetCartsByUserIdAsync(int userId)
        {
            using var connection = dbContext.Database.GetDbConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@UserId", userId);

            var result = await connection.QueryAsync<Cart>(
                "sp_GetCartByUser",parameters,
                commandType: CommandType.StoredProcedure
            );

            return result.ToList();
        }

        public async Task<bool> RemoveFromCartAsync(int cartId,int Userid)
        {
            using var connection = dbContext.Database.GetDbConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@CartId", cartId);
            parameters.Add("@UserId", Userid);

            var rowsAffected = await connection.ExecuteAsync(
                "sp_RemoveCartById",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return rowsAffected > 0;
        }
    }
}
