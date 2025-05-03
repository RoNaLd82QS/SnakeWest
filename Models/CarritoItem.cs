using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VentaZapatillas.Models
{
    public class CarritoItem
    {
        public int ProductoId { get; set; }
        public string Nombre { get; set; } = "";
        public decimal Precio { get; set; }
        public int Cantidad { get; set; }
    }
}