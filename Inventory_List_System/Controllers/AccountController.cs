using Inventory_List_System.Helpers;
using Inventory_List_System.Models.Database;
using Inventory_List_System.Respositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Inventory_List_System.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserRepository _userRepository;
        public AccountController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "Username and password are required.";
                ViewBag.Username = username;
                return View();
            }

            username = username.Trim();

            if (username.Length < 3)
            {
                ViewBag.Error = "Username must be at least 3 characters.";
                ViewBag.Username = username;
                return View();
            }

            if (password.Length < 6)
            {
                ViewBag.Error = "Password must be at least 6 characters.";
                ViewBag.Username = username;
                return View();
            }

            if (_userRepository.UsernameExists(username))
            {
                ViewBag.Error = "Username already exists.";
                ViewBag.Username = username;
                return View();
            }

            User user = new User
            {
                Username = username,
                PasswordHash = SecurityHelpers.HashPassword(password)
            };

            _userRepository.AddUser(user);
            TempData["Success"] = "Registration successful. You can now log in.";
            return RedirectToAction("Login");
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "Username and password are required.";
                ViewBag.Username = username;
                return View();
            }

            username = username.Trim();

            var user = _userRepository.GetByUsername(username);

            if (user == null)
            {
                ViewBag.Error = "Username does not exist.";
                ViewBag.Username = username;
                return View();
            }

            bool isPasswordValid = SecurityHelpers.VerifyPassword(password, user.PasswordHash);

            if (!isPasswordValid)
            {
                ViewBag.Error = "Password is incorrect.";
                ViewBag.Username = username;
                return View();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal
            );

            TempData["Success"] = "Login successful.";
            return RedirectToAction("Index", "Inventory");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            TempData["Success"] = "You have been logged out.";
            return RedirectToAction("Login");
        }

        public IActionResult AccessDenied()
        {
            TempData["Error"] = "Access denied. Please log in with the correct account.";
            return RedirectToAction("Login");
        }
    }
}
