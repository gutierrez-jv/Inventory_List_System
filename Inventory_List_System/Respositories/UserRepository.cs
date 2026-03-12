using Inventory_List_System.Models.Database;
using Microsoft.AspNetCore.Mvc;

namespace Inventory_List_System.Respositories
{
    public class UserRepository : IUserRepostitory
    {
        private readonly InventoryDbContext _context;
        public UserRepository(InventoryDbContext context)
        {
            _context = context;
        }
        public void AddUser(User user)
        {
            // Adds users and saves the changes to the database
            _context.Users.Add(user);
            _context.SaveChanges();
        }

        public User GetByUsername(string username)
        {
            // Retrieves a user by their username, throws an exception if the user is not found
            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user == null) throw new Exception("User not found");
            return user;
        }

        public bool UsernameExists(string username)
        {
            // Checks if a username exists in the database, returns true if it does, false otherwise
            if (_context.Users.Any(u => u.Username == username)) {
                return true;
            }
            return false;
        }

        public User ValidateUser(string username, string password)
        {
            throw new NotImplementedException();
        }
    }
}
