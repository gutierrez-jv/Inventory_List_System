using Inventory_List_System.Models.Database;

namespace Inventory_List_System.Respositories
{
    public interface IInventoryRepository
    {
        List<InventoryItem> GetItemsByUser(int userId, string search, int page, int pageSize);

        int GetTotalItems(int userId, string search);

        InventoryItem GetItemById(int id);

        void AddItem(InventoryItem item);

        void UpdateItem(InventoryItem item);

        void DeleteItem(int id);
    }
}
