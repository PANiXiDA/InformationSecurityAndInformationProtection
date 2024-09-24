using DAL.DbModels;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL.SQL
{
    public class UsersDAL : IUsersDAL
    {
        private readonly DefaultDbContext _context;

        public UsersDAL(DefaultDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<bool> AddUserAsync(Entities.User user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (await ExistsAsync(user.Login))
                throw new InvalidOperationException($"User with login {user.Login} already exists.");

            var dbUser = new User
            {
                Login = user.Login,
                Password = user.Password,
                Sault = user.Sault
            };

            _context.Users.Add(dbUser);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateUserAsync(Entities.User user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            var dbUser = await _context.Users.FindAsync(user.Login);
            if (dbUser == null)
                throw new InvalidOperationException($"User with login {user.Login} does not exist.");

            dbUser.Password = user.Password;
            dbUser.Sault = user.Sault;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Entities.User?> GetAsync(string login)
        {
            if (string.IsNullOrEmpty(login)) throw new ArgumentNullException(nameof(login));

            var dbUser = await _context.Users.FindAsync(login);
            if (dbUser == null) return null;

            return new Entities.User(dbUser.Login, dbUser.Password)
            {
                Sault = dbUser.Sault
            };
        }

        public async Task<bool> DeleteAsync(string login)
        {
            if (string.IsNullOrEmpty(login)) throw new ArgumentNullException(nameof(login));

            var dbUser = await _context.Users.FindAsync(login);
            if (dbUser == null) return false;

            _context.Users.Remove(dbUser);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(string login)
        {
            if (string.IsNullOrEmpty(login)) throw new ArgumentNullException(nameof(login));

            return await _context.Users.AnyAsync(u => u.Login == login);
        }
    }
}
