using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using ECommerceAPI.Data;
using ECommerceAPI.Models;
using ECommerceAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommerceAPI.Repositories.Declarations
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext dbContext;

        public UserRepository(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<Users> CreateUserAsync(Users user)
        {
            using var connection = dbContext.Database.GetDbConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@UserName", user.UserName);
            parameters.Add("@Password", user.Password);
            parameters.Add("@EmailId", user.EmailId);
            parameters.Add("@Address", user.Address);
            parameters.Add("@ProfilePicture", user.ProfilePicture);
            parameters.Add("@Role", 2);

            var result = await connection.QuerySingleOrDefaultAsync<Users>(
                "sp_CreateUser",
                parameters,
                commandType: CommandType.StoredProcedure);

            return result;
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            using var connection = dbContext.Database.GetDbConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@UserId", userId);

            var rowsAffected = await connection.ExecuteAsync(
                "sp_DeleteUser",
                parameters,
                commandType: CommandType.StoredProcedure);

            return rowsAffected > 0;
        }

        public async Task<Users> GetUserByIdAsync(int userId)
        {
            using var connection = dbContext.Database.GetDbConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@UserId", userId);

            return await connection.QuerySingleOrDefaultAsync<Users>(
                "sp_GetUserById",
                parameters,
                commandType: CommandType.StoredProcedure);
        }

        public async Task<Users> UpdateUserAsync(Users user)
        {
            using var connection = dbContext.Database.GetDbConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@UserId", user.UserId);
            parameters.Add("@UserName", user.UserName);
            parameters.Add("@Password", user.Password);
            parameters.Add("@EmailId", user.EmailId);
            parameters.Add("@Address", user.Address);
            parameters.Add("@ProfilePicture", user.ProfilePicture);
            parameters.Add("@Role", 2);

            var result = await connection.QuerySingleOrDefaultAsync<Users>(
                "sp_UpdateUser",
                parameters,
                commandType: CommandType.StoredProcedure);

            return result;
        }

        public async Task<Users> ValidateUserLoginAsync(string emailId, string password)
        {
            using var connection = dbContext.Database.GetDbConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@EmailId", emailId);
            parameters.Add("@Password", password);

            return await connection.QuerySingleOrDefaultAsync<Users>(
                "sp_ValidateUserLogin",
                parameters,
                commandType: CommandType.StoredProcedure);
        }
    }
}
