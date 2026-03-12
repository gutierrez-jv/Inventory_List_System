using Inventory_List_System.Models.Database;
using Microsoft.AspNetCore.Mvc;
using System.Security.Permissions;
using Inventory_List_System.Helpers;

namespace Inventory_List_System.Respositories
{
    public class UserRepository : IUserRepository
    {
        private readonly InventoryDbContext _context;
        public UserRepository(InventoryDbContext context)
        {
            _context = context;
        }
        public void AddUser(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
        }

        public User? GetByUsername(string username)
        {
            return _context.Users.FirstOrDefault(u => u.Username == username);
        }

        public bool UsernameExists(string username)
        {
            if (_context.Users.Any(u => u.Username == username)) {
                return true;
            }
            return false;
        }

        public User? ValidateUser(string username, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user == null) return null;

            bool isPasswordValid = SecurityHelpers.VerifyPassword(password, user.PasswordHash);

            if (!isPasswordValid) return null;
            return user;
        }
    }
}
