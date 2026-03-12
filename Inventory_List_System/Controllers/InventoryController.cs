using Inventory_List_System.Models.Database;
using Inventory_List_System.Respositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Inventory_List_System.Controllers
{
    [Authorize]
    public class InventoryController : Controller
    {
        private readonly IInventoryRepository _inventoryRepository;
        public InventoryController(IInventoryRepository inventoryRepository)
        {
            _inventoryRepository = inventoryRepository;
        }
        public IActionResult Index(string search, int page = 1)
        {
            //CODE SNIPPET FOR PAGINATION
            int pageSize = 10;

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            int parsedUserId = int.Parse(userId);

            var items = _inventoryRepository.GetItemsByUser(parsedUserId, search, page, pageSize);

            ViewBag.TotalItems = _inventoryRepository.GetTotalItems(parsedUserId, search);
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.Search = search;

            return View(items);
        }

        [HttpPost]
        public IActionResult Create(string itemName, int quantity, decimal price)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            InventoryItem item = new InventoryItem
            {
                ItemName = itemName,
                Quantity = quantity,
                Price = price,
                DateAdded = DateTime.Now,
                UserId = int.Parse(userId)
            };

            _inventoryRepository.AddItem(item);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            _inventoryRepository.DeleteItem(id);
            return RedirectToAction("Index");
        }
    }
}
