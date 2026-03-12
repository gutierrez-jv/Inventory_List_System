using Inventory_List_System.Models.Database;
using Microsoft.AspNetCore.Mvc;

namespace Inventory_List_System.Respositories
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly InventoryDbContext _context;
        public InventoryRepository(InventoryDbContext context)
        {
            _context = context;
        }
        public void AddItem(InventoryItem item)
        {
            _context.InventoryItems.Add(item);
            _context.SaveChanges();
        }

        public void DeleteItem(int id)
        {
            _context.Remove(_context.InventoryItems.FirstOrDefault(i => i.Id == id) ?? throw new Exception("Item not found"));
            _context.SaveChanges();
        }

        public InventoryItem GetItemById(int id)
        {
            return _context.InventoryItems.FirstOrDefault(i => i.Id == id) ?? throw new Exception("Item not found");
        }

        public List<InventoryItem> GetItemsByUser(int userId, string search, int page, int pageSize)
        {
            var query = _context.InventoryItems.Where(i => i.UserId == userId);
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(i => i.ItemName.Contains(search));
            }
            return query
                .OrderByDescending(i => i.DateAdded)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }

        public int GetTotalItems(int userId, string search)
        {
            var query = _context.InventoryItems.Where(i => i.UserId == userId);

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(i => i.ItemName.Contains(search));
            }

            return query.Count();
        }

        public void UpdateItem(InventoryItem item)
        {
            _context.InventoryItems.Update(item);
            _context.SaveChanges();
        }
    }
}