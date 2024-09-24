using Entities;

namespace DAL.Interfaces
{
    public interface IUsersDAL
    {
        Task<bool> AddUserAsync(User user);
        Task<bool> UpdateUserAsync(User user);
        Task<User?> GetAsync(string login);
        Task<bool> DeleteAsync(string login);
        Task<bool> ExistsAsync(string login);
    }
}
