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
    public class WhishListRepository : IWhishlistRepository
    {
        private readonly AppDbContext dbContext;

        public WhishListRepository(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<bool> AddToWhishListAsync(WhishList whishList)
        {
            using var connection = dbContext.Database.GetDbConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@ProductId", whishList.ProductId);
            parameters.Add("@UserId", whishList.UserId);

            var result = await connection.QuerySingleOrDefaultAsync<WhishList>(
                "sp_AddWhishList",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result != null;
        }

        public async Task<List<WhishList>> GetWhishListsByUserIdAsync(int userId)
        {
            using var connection = dbContext.Database.GetDbConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@UserId", userId);

            var result = await connection.QueryAsync<WhishList>(
                "sp_GetWhishListByUser",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result.ToList();
        }

        public async Task<bool> RemoveFromWhishListAsync(int whishListId, int userId)
        {
            using var connection = dbContext.Database.GetDbConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@WhishListId", whishListId);
            parameters.Add("@UserId", userId);

            var rowsAffected = await connection.ExecuteAsync(
                "sp_RemoveWhishListById",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return rowsAffected > 0;
        }
    }
}
