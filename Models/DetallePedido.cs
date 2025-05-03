using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VentaZapatillas.Models
{
    public class DetallePedido
    {
        public int Id { get; set; }

        public int PedidoId { get; set; }
        public Pedido? Pedido { get; set; }

        public int ProductoId { get; set; }
        public string NombreProducto { get; set; } = "";
        public decimal PrecioUnitario { get; set; }
        public int Cantidad { get; set; }
    }
}