using ECommerceAPI.Models;
using System.Threading.Tasks;

namespace ECommerceAPI.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<Users> CreateUserAsync(Users user);
        Task<Users> GetUserByIdAsync(int userId);
        Task<Users> UpdateUserAsync(Users user);
        Task<bool> DeleteUserAsync(int userId);
        Task<Users> ValidateUserLoginAsync(string emailId, string password);
    }
}
