using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VentaZapatillas.Data;
using VentaZapatillas.Models;

namespace VentaZapatillas.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin
        public async Task<IActionResult> Index()
        {
            var zapatillas = await _context.Zapatillas.ToListAsync();

            // 📊 Datos para el dashboard
            ViewBag.TotalZapatillas = zapatillas.Count;
            ViewBag.TotalStock = zapatillas.Sum(z => z.Stock);
            ViewBag.UltimaZapatilla = zapatillas.OrderByDescending(z => z.Id).FirstOrDefault();

            return View(zapatillas);
        }
    }
}

