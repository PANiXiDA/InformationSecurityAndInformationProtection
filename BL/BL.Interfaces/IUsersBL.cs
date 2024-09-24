using Entities;

namespace BL.Interfaces
{
    public interface IUsersBL
    {
        Task<bool> RegisterUserAsync(User user);
        Task<bool> AuthenticateUserAsync(string login, string password);
        Task<User?> GetUserByLoginAsync(string login);
        Task<bool> DeleteUserAsync(string login);
        Task<bool> UpdateUserPasswordAsync(string login, string newPassword);
    }
}
