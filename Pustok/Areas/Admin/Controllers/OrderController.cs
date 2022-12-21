using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Pustok.Areas.Admin.ViewModels;
using Pustok.DAL;
using Pustok.Enums;
using Pustok.Hubs;
using Pustok.Models;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Pustok.Areas.Admin.Controllers
{
    [Authorize(Roles = "SuperAdmin, Admin, Editor")]
    [Area("Admin")]

    public class OrderController : Controller
    {
        private readonly PustokDbContext _context;
        private readonly IHubContext<ChatHub> _hubContext;

        public OrderController(PustokDbContext context, IHubContext<ChatHub> hubContext)
        {
            _context = context;
            this._hubContext = hubContext;
        }
        public IActionResult Index(int page = 1)
        {
            var query = _context.Orders
            .Include(x => x.OrderItems).ThenInclude(x => x.Book)
            .Include(x => x.AppUser);

            var model = PaginatedList<Order>.Create(query, page, 10);

            return View(model);
        }
        public IActionResult Edit(int id)
        {
            var model = _context.Orders.Include(x=> x.AppUser).FirstOrDefault(x => x.Id == id);
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Accept(int id)
        {
            var order = _context.Orders.Include(x => x.AppUser).FirstOrDefault(x => x.Id == id);
            if (order == null) return NotFound();
            order.OrderStatus = OrderStatus.Accepted;
            _context.SaveChanges();

            if(order.AppUser !=null && order.AppUser.ConnectionId != null)
            {
            await _hubContext.Clients.Client(order.AppUser.ConnectionId).SendAsync("OrderStatus", true);
            }

            return RedirectToAction("index");
        }
        [HttpPost]
        public async Task<IActionResult> Reject(int id)
        {
            var order = _context.Orders.Include(x=> x.AppUser).FirstOrDefault(x => x.Id == id);
            if (order == null) return NotFound();

            order.OrderStatus = OrderStatus.Rejected;
            _context.SaveChanges();


            if (order.AppUser != null && order.AppUser.ConnectionId != null)
            {
                await _hubContext.Clients.Client(order.AppUser.ConnectionId).SendAsync("OrderStatus", false);
            }

            return RedirectToAction("index");
        }
    }
}
