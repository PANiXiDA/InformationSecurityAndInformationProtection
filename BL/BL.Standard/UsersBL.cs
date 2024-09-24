using BL.Interfaces;
using DAL.Interfaces;
using Entities;
using System.Security.Cryptography;
using System.Text;

namespace BL.Standard
{
    public class UsersBL : IUsersBL
    {
        private readonly IUsersDAL _usersDal;

        public UsersBL(IUsersDAL usersDal)
        {
            _usersDal = usersDal ?? throw new ArgumentNullException(nameof(usersDal));
        }

        public async Task<bool> RegisterUserAsync(User user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (await _usersDal.ExistsAsync(user.Login))
                throw new InvalidOperationException($"User with login {user.Login} already exists.");

            var salt = GenerateSalt();
            user.Sault = salt;
            user.Password = HashPassword(user.Password, salt);

            return await _usersDal.AddUserAsync(user);
        }

        public async Task<bool> AuthenticateUserAsync(string login, string password)
        {
            if (string.IsNullOrEmpty(login)) throw new ArgumentNullException(nameof(login));
            if (string.IsNullOrEmpty(password)) throw new ArgumentNullException(nameof(password));

            var user = await _usersDal.GetAsync(login);
            if (user == null) return false;

            var hashedPassword = HashPassword(password, user.Sault);
            return hashedPassword == user.Password;
        }

        public async Task<User?> GetUserByLoginAsync(string login)
        {
            if (string.IsNullOrEmpty(login)) throw new ArgumentNullException(nameof(login));

            return await _usersDal.GetAsync(login);
        }

        public async Task<bool> DeleteUserAsync(string login)
        {
            if (string.IsNullOrEmpty(login)) throw new ArgumentNullException(nameof(login));

            return await _usersDal.DeleteAsync(login);
        }

        public async Task<bool> UpdateUserPasswordAsync(string login, string newPassword)
        {
            if (string.IsNullOrEmpty(login)) throw new ArgumentNullException(nameof(login));
            if (string.IsNullOrEmpty(newPassword)) throw new ArgumentNullException(nameof(newPassword));

            var user = await _usersDal.GetAsync(login);
            if (user == null) return false;

            var salt = GenerateSalt();
            user.Password = HashPassword(newPassword, salt);
            user.Sault = salt;

            return await _usersDal.UpdateUserAsync(user);
        }

        private string GenerateSalt()
        {
            var saltBytes = new byte[16];
            RandomNumberGenerator.Fill(saltBytes);
            return Convert.ToBase64String(saltBytes);
        }

        private string HashPassword(string password, string salt)
        {
            using (var sha256 = SHA256.Create())
            {
                var saltedPassword = $"{salt}{password}";
                var saltedPasswordBytes = Encoding.UTF8.GetBytes(saltedPassword);
                var hashBytes = sha256.ComputeHash(saltedPasswordBytes);
                return Convert.ToBase64String(hashBytes);
            }
        }
    }
}
