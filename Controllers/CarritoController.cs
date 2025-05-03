using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VentaZapatillas.Data;
using VentaZapatillas.Models;
using VentaZapatillas.Service;
using Microsoft.AspNetCore.Authorization;

namespace VentaZapatillas.Controllers
{
    public class CarritoController : Controller
    {
        private readonly CarritoService _carritoService;
        private readonly ILogger<CarritoController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CarritoController(
            CarritoService carritoService,
            ILogger<CarritoController> logger,
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _carritoService = carritoService;
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var carrito = _carritoService.GetCarrito();
            return View(carrito);
        }

        [HttpPost]
        public IActionResult Agregar(int id, string nombre, decimal precio)
        {
            if (id <= 0 || string.IsNullOrWhiteSpace(nombre) || precio <= 0)
            {
                TempData["ErrorMessage"] = "⚠️ Datos del producto inválidos.";
                return RedirectToAction(nameof(Index));
            }

            // En producción, verifica que el producto exista en la base de datos para evitar manipulación del cliente.
            var nuevoItem = new CarritoItem
            {
                ProductoId = id,
                Nombre = nombre,
                Precio = precio,
                Cantidad = 1
            };

            _carritoService.AddItem(nuevoItem);
            TempData["SuccessMessage"] = $"✅ Producto '{nombre}' agregado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Limpiar()
        {
            _carritoService.Clear();
            TempData["InfoMessage"] = "🗑️ Carrito vaciado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Finaliza el pedido y lo guarda en la base de datos.
        /// </summary>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> FinalizarPedido()
        {
            var carrito = _carritoService.GetCarrito();

            if (!carrito.Any())
            {
                TempData["ErrorMessage"] = "⚠️ El carrito está vacío. No se puede finalizar el pedido.";
                return RedirectToAction(nameof(Index));
            }

            var usuario = await _userManager.GetUserAsync(User);
            if (usuario == null)
            {
                TempData["ErrorMessage"] = "⚠️ Debes iniciar sesión para finalizar el pedido.";
                return RedirectToAction("Login", "Account");
            }

            var pedido = new Pedido
            {
                UsuarioId = usuario.Id,
                FechaPedido = DateTime.UtcNow,
                Total = carrito.Sum(i => i.Precio * i.Cantidad),
                Detalles = carrito.Select(i => new DetallePedido
                {
                    ProductoId = i.ProductoId,
                    NombreProducto = i.Nombre,
                    PrecioUnitario = i.Precio,
                    Cantidad = i.Cantidad
                }).ToList()
            };

            try
            {
                _context.Pedidos.Add(pedido);
                await _context.SaveChangesAsync();

                _carritoService.Clear();
                TempData["SuccessMessage"] = "✅ Pedido realizado correctamente.";

                return RedirectToAction(nameof(Confirmacion));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al guardar el pedido: {Mensaje}", ex.InnerException?.Message ?? ex.Message);
                TempData["ErrorMessage"] = "❌ Ocurrió un error al procesar tu pedido.";
                return RedirectToAction(nameof(Index));
            }
        }

        [Authorize]
        public IActionResult Confirmacion()
        {
            return View();
        }
    }
}
