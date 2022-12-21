using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pustok.DAL;
using Pustok.Models;
using System.Data;
using System.Linq;

namespace Pustok.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "SuperAdmin, Admin, Editor")]

    public class UserController : Controller
    {
        private readonly PustokDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public UserController(PustokDbContext context,UserManager<AppUser> userManager)
        {
            this._context = context;
            this._userManager = userManager;
        }
        public IActionResult Index()
        {
            var users = _userManager.Users.ToList();

            return View(users);
        }
    }
}
