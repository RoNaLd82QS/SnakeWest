using VentaZapatillas.Helpers;
using VentaZapatillas.Models;

namespace VentaZapatillas.Service
{
    public class CarritoService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string SessionKey = "Carrito";

        public CarritoService(IHttpContextAccessor accessor)
        {
            _httpContextAccessor = accessor;
        }

        public List<CarritoItem> GetCarrito()
        {
            var session = _httpContextAccessor.HttpContext!.Session;
            var carrito = session.GetObjectFromJson<List<CarritoItem>>(SessionKey);
            return carrito ?? new List<CarritoItem>();
        }

        public void AddItem(CarritoItem item)
        {
            var carrito = GetCarrito();
            var existente = carrito.FirstOrDefault(p => p.ProductoId == item.ProductoId);

            if (existente != null)
                existente.Cantidad += item.Cantidad;
            else
                carrito.Add(item);

            _httpContextAccessor.HttpContext!.Session.SetObjectAsJson(SessionKey, carrito);
        }

        public void Clear()
        {
            _httpContextAccessor.HttpContext!.Session.Remove(SessionKey);
        }
    }
}
