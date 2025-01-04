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
    public class OrdersRepository : IOrdersRepository
    {
        private readonly AppDbContext dbContext;

        public OrdersRepository(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<bool> CreateOrdersAsync(Orders Orders)
        {
            using var connection = dbContext.Database.GetDbConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@ProductId", Orders.ProductId);
            parameters.Add("@Quantity", Orders.Quantity);
            parameters.Add("@UserId", Orders.UserId);
            parameters.Add("@OrderDate", Orders.OrderDate);
            parameters.Add("@Address", Orders.Address);
            parameters.Add("@Status", Orders.Status);

            var result = await connection.QuerySingleOrDefaultAsync<Category>("sp_AddOrder", parameters, commandType: CommandType.StoredProcedure);

            return result != null;
        }

        public async Task<bool> DeleteOrderAsync(int OrderId)
        {
            using var connection = dbContext.Database.GetDbConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@OrderId", OrderId);

            var rowsAffected = await connection.ExecuteAsync(
                "sp_DeleteOrder",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return rowsAffected > 0;
        }

        public async Task<List<Orders>> GetAllOrdersAsync()
        {
            using var connection = dbContext.Database.GetDbConnection();
            var result = await connection.QueryAsync<Orders>(
                "sp_GetAllOrders",
                commandType: CommandType.StoredProcedure
            );

            return result.AsList();
        }

        public async Task<Orders> GetOrdersByIdAsync(int UserId)
        {
            using var connection = dbContext.Database.GetDbConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@UserId", UserId);

            return await connection.QuerySingleOrDefaultAsync<Orders>(
                "sp_GetOrder",
                parameters,
                commandType: CommandType.StoredProcedure);
        }
    }
}
